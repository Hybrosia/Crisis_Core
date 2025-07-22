using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GruntController : MonoBehaviour, IEnemyHealthManager
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject clusterPrefab;
    [SerializeField] private float startMeleeRange, meleeRange, closeRange, viewDistance, joinClusterRadius, avoidPlayerDistance, damage;
    [SerializeField] private float knockbackInitialSpeed, stunTime, angryTime, knockbackSlowdownPerSecond, attackCooldown;
    [SerializeField] private float maxHealth;
    [SerializeField] private LayerMask terrain;

    [HideInInspector] public float lastSawPlayerTime, attackTimer;
    [HideInInspector] public Vector3 lastKnownPlayerPosition;
    [HideInInspector] public GruntState state;
    [HideInInspector] public GruntClusterController cluster;

    private float _currentHealth;
    private float _standardSpeed;
    private float _stunTimer, _angryTimer;
    private NavigationPoint _lastCheckedPoint;
    private List<NavigationPoint> _currentPath;
    
    public static List<GruntController> ActiveGrunts = new List<GruntController>();

    public enum GruntState
    {
        Searching,
        Afraid,
        Stunned,
        Angry
    }
    
    
    private void OnEnable()
    {
        ActiveGrunts.Add(this);
        _standardSpeed = agent.speed;
        StartCoroutine(OnEnableCoroutine());
    }
    
    private void OnDisable() => ActiveGrunts.Remove(this);

    private IEnumerator OnEnableCoroutine()
    {
        yield return null;
        SetSearching();
    }
    
    private void Update()
    {
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (canSeePlayer)
        {
            lastSawPlayerTime = Time.time;
            lastKnownPlayerPosition = playerData.PlayerPos;
        }
        
        if (cluster)
        {
            agent.SetDestination(cluster.transform.position);
            return;
        }
        
        if (state == GruntState.Searching) Searching(canSeePlayer);
        else if (state == GruntState.Afraid) Afraid(canSeePlayer);
        else if (state == GruntState.Stunned) Stunned(canSeePlayer);
        else if (state == GruntState.Angry) Angry(canSeePlayer);
    }

    private void SetSearching()
    {
        state = GruntState.Searching;
        animator.Play("Move");
        agent.isStopped = false;
        agent.speed = _standardSpeed;

        agent.SetDestination(FindNextSearchTarget());
    }

    private void Searching(bool canSeePlayer)
    {
        var closestVisibleGrunt = FindClosestVisibleGrunt();

        if (closestVisibleGrunt)
        {
            if (Vector3.Distance(transform.position, closestVisibleGrunt.transform.position) <= joinClusterRadius  
                && closestVisibleGrunt.state is GruntState.Searching or GruntState.Afraid)
            {
                //Create or join a cluster.
                if (closestVisibleGrunt.cluster) closestVisibleGrunt.cluster.AddGrunt(this);
                else CreateCluster(closestVisibleGrunt);
            }
            else agent.SetDestination(closestVisibleGrunt.transform.position);
        }
        else if (canSeePlayer && Vector3.Distance(transform.position, playerData.PlayerPos) < closeRange) SetAfraid();
        else if (agent.remainingDistance < 0.05f) agent.SetDestination(FindNextSearchTarget());
    }

    private void SetAfraid()
    {
        state = GruntState.Afraid;
        animator.Play("Flee");
        agent.isStopped = false;
        agent.speed = _standardSpeed;

        _currentPath = GetPathAwayFromPlayer();
        agent.SetDestination(_currentPath[0].transform.position);
    }

    private void Afraid(bool canSeePlayer)
    {
        var closestVisibleGrunt = FindClosestVisibleGrunt();
        
        if (closestVisibleGrunt) SetSearching();
        else if (!canSeePlayer && Vector3.Distance(transform.position, lastKnownPlayerPosition) > viewDistance) SetSearching();
        else if (canSeePlayer)
        {
            _currentPath = GetPathAwayFromPlayer();
            agent.SetDestination(_currentPath[0].transform.position);
        }
    }
    
    private void SetStunned(Vector3 knockBackDirection)
    {
        state = GruntState.Stunned;
        animator.Play("Stunned");
        agent.isStopped = false;
        
        agent.Raycast(transform.position + knockbackInitialSpeed * stunTime * knockBackDirection, out var hit);

        agent.SetDestination(hit.position);
        agent.speed = knockbackInitialSpeed;

        _stunTimer = Time.time + stunTime;
    }

    private void Stunned(bool canSeePlayer)
    {
        agent.speed = Mathf.Clamp(agent.speed - Time.deltaTime * knockbackSlowdownPerSecond, 0f, knockbackInitialSpeed);

        if (Time.time > _stunTimer) SetAngry();
        else if (agent.remainingDistance < 0.05f) agent.isStopped = true;
    }
    
    private void SetAngry()
    {
        state = GruntState.Angry;
        animator.Play("Angry");
        agent.isStopped = false;
        agent.speed = _standardSpeed;

        agent.SetDestination(lastKnownPlayerPosition);

        _angryTimer = Time.time + angryTime;
    }

    private void Angry(bool canSeePlayer)
    {
        if (Time.time > _angryTimer) SetSearching();

        agent.SetDestination(lastKnownPlayerPosition);

        if (!canSeePlayer) return;
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > startMeleeRange) return;
        if (Time.time < attackTimer) return;

        attackTimer = Time.time + attackCooldown;
        animator.Play("AngryAttack");
    }
    
    //Tries to damage the player. Trigger from animation event.
    private void DoMeleeAttack()
    {
        if (cluster && cluster.IsProperCluster()) return;
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > meleeRange) return;
        
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(damage);
    }

    //Starts an attack.
    public void StartAttack()
    {
        animator.Play("Attack");

        attackTimer = Time.time + attackCooldown;
    }

    public Vector3 FindNextSearchTarget()
    {
        var closestNavigationPoint = NavigationPoint.FindPointClosestToPosition(transform.position);
        
        Vector3 nextPoint;

        if (Vector3.Distance(closestNavigationPoint.transform.position, transform.position) > 0.3f) nextPoint = closestNavigationPoint.transform.position;
        else
        {
            var newPoints = closestNavigationPoint.pointsWithCost.Keys.ToList();
            if (_lastCheckedPoint) newPoints.Remove(_lastCheckedPoint);

            if (newPoints.Count != 0) nextPoint = newPoints[Random.Range(0, newPoints.Count)].transform.position;
            else if (_lastCheckedPoint) nextPoint = _lastCheckedPoint.transform.position;
            else nextPoint = closestNavigationPoint.transform.position;
        }

        _lastCheckedPoint = closestNavigationPoint;
        
        return nextPoint;
    }

    private List<NavigationPoint> GetPathAwayFromPlayer()
    {
        var vectorFromPlayerToGrunt = transform.position - playerData.PlayerPos;

        var closestNavigationPoint = NavigationPoint.FindPointClosestToPosition(transform.position);
        var preferredPoints = NavigationPoint.ActiveNavigationPoints
            .Where(point => Vector3.Dot(vectorFromPlayerToGrunt, point.transform.position - transform.position) > 0f).ToList();

        if (!preferredPoints.Any()) preferredPoints = NavigationPoint.ActiveNavigationPoints.ToList();

        var furthestNavigationPoint = preferredPoints
            .Aggregate((furthest, next) => 
                Vector3.Distance(transform.position, next.transform.position) >
                Vector3.Distance(transform.position, furthest.transform.position) ? next : furthest);
        
        return closestNavigationPoint.FindShortestPathToPoint(furthestNavigationPoint);
    }

    //Returns the closest visible grunt.
    public GruntController FindClosestVisibleGrunt()
    {
        var validGrunts = ActiveGrunts
            .Where(grunt => grunt != this)
            .Where(grunt => !grunt.cluster || (grunt.cluster && grunt.cluster != cluster))
            .Where(grunt => Vector3.Distance(transform.position, grunt.transform.position) < viewDistance)
            .Where(grunt =>
                !Physics.Raycast(transform.position, (grunt.transform.position - transform.position).normalized,
                    out var hitInfo, terrain));
        
        if (validGrunts.Any())
            return validGrunts.Aggregate((closest, next) =>
                Vector3.Distance(transform.position, next.transform.position) <
                Vector3.Distance(transform.position, closest.transform.position) ? next : closest);
        return null;
    }

    private void CreateCluster(GruntController otherGrunt)
    {
        var clusterObject = ObjectPoolController.SpawnFromPrefab(clusterPrefab);
        var clusterScript = clusterObject.GetComponent<GruntClusterController>();
        PutInCluster(clusterScript);
        
        cluster.transform.position = (transform.position + otherGrunt.transform.position) / 2f;
        cluster.transform.rotation = Quaternion.identity;
        
        otherGrunt.PutInCluster(cluster);
    }

    public void PutInCluster(GruntClusterController cluster)
    {
        this.cluster = cluster;
        cluster.AddGrunt(this);
        SetSearching();
    }

    public void RemoveFromCluster()
    {
        if (!cluster) return;
        
        cluster.RemoveGrunt(this);
        cluster = null;
    }

    public void ExplodeCluster(Vector3 centerOfCluster)
    {
        cluster = null;
        var explosionDirection = (transform.position - centerOfCluster).normalized;

        if (explosionDirection == Vector3.zero)
            SetStunned((transform.position - playerData.PlayerPos).normalized);
        else SetStunned(explosionDirection);
    }
    
    private void ResetHealth()
    {
        _currentHealth = maxHealth;
    }
    
    //Takes damage.
    public void TakeDamage(float amount)
    {
        if (cluster && cluster.IsProperCluster())
        {
            cluster.TakeDamage(amount);
            return;
        }

        _currentHealth -= amount;
        
        if (_currentHealth > 0f) return;
        
        if (cluster) RemoveFromCluster();
        
        if (TryGetComponent<EnemyDeathBase>(out var deathScript)) deathScript.OnDeath();
        else ObjectPoolController.DeactivateInstance(gameObject);
    }
}