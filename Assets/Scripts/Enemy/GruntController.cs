using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GruntController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float meleeRange, closeRange, viewDistance, joinClusterRadius, damage;
    [SerializeField] private float knockbackInitialSpeed, stunTime, angryTime, knockbackSlowdownPerSecond, attackCooldown;
    [SerializeField] private float maxHealth;

    [HideInInspector] public float lastSawPlayerTime, attackTimer;
    [HideInInspector] public Vector3 lastKnownPlayerPosition;
    
    private float _currentHealth;
    private float _standardSpeed;
    private GruntState _state;
    private float _stunTimer, _angryTimer;
    private GruntClusterController _cluster;
    
    private enum GruntState
    {
        Searching,
        Afraid,
        Stunned,
        Angry
    }

    private void OnEnable()
    {
        SetSearching();
        _standardSpeed = agent.speed;
    }

    private void Update()
    {
        if (_cluster)
        {
            agent.SetDestination(_cluster.transform.position);
            return;
        }
        
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (canSeePlayer)
        {
            lastSawPlayerTime = Time.time;
            lastKnownPlayerPosition = playerData.PlayerPos;
        }
        
        if (_state == GruntState.Searching) Searching(canSeePlayer);
        else if (_state == GruntState.Afraid) Afraid(canSeePlayer);
        else if (_state == GruntState.Stunned) Stunned(canSeePlayer);
        else if (_state == GruntState.Angry) Angry(canSeePlayer);
    }

    private void SetSearching()
    {
        _state = GruntState.Searching;
        animator.Play("Search");
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
                && closestVisibleGrunt._state is GruntState.Searching or GruntState.Afraid)
            {
                //Create or join a cluster.
                if (closestVisibleGrunt._cluster) closestVisibleGrunt._cluster.AddGrunt(this);
                else CreateCluster(closestVisibleGrunt);
            }
            else agent.SetDestination(closestVisibleGrunt.transform.position);
        }
        else if (canSeePlayer && Vector3.Distance(transform.position, playerData.PlayerPos) < closeRange) SetAfraid();
        else if (agent.remainingDistance < 0.05f) agent.SetDestination(FindNextSearchTarget());
    }

    private void SetAfraid()
    {
        _state = GruntState.Afraid;
        animator.Play("Flee");
        agent.isStopped = false;
        agent.speed = _standardSpeed;

        //TODO: Find the closest hiding spot or a target away from the player.
    }

    private void Afraid(bool canSeePlayer)
    {
        var closestVisibleGrunt = FindClosestVisibleGrunt();
        
        if (closestVisibleGrunt) SetSearching();
        else if (!canSeePlayer && Vector3.Distance(transform.position, lastKnownPlayerPosition) > viewDistance) SetSearching();
        else if (canSeePlayer)
        {
            //TODO: Find the closest hiding spot or a target away from the player.
            //agent.SetDestination(_lastKnownPlayerPosition);
        }
    }
    
    private void SetStunned(Vector3 knockBackDirection)
    {
        _state = GruntState.Stunned;
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

        if (Time.time < _stunTimer) SetAngry();
        else if (agent.remainingDistance < 0.05f) agent.isStopped = true;
    }
    
    private void SetAngry()
    {
        _state = GruntState.Angry;
        animator.Play("Angry");
        agent.isStopped = false;
        agent.speed = _standardSpeed;

        agent.SetDestination(lastKnownPlayerPosition);

        _angryTimer = Time.time + angryTime;
    }

    private void Angry(bool canSeePlayer)
    {
        if (Time.time < _angryTimer) SetSearching();

        agent.SetDestination(lastKnownPlayerPosition);

        if (!canSeePlayer) return;
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > meleeRange) return;
        if (Time.time < attackTimer) return;

        attackTimer = Time.time + attackCooldown;
        animator.Play("AngryAttack");
    }

    public void DoMeleeAttack()
    {
        if (_cluster && _cluster.IsProperCluster()) return;
        //Damage the player. Trigger from AnimationEvent.
    }

    //Starts an attack.
    public void StartAttack()
    {
        animator.Play("Attack");

        attackTimer = Time.time + attackCooldown;
    }

    private Vector3 FindNextSearchTarget()
    {
        return Vector3.zero;
        //TODO: Find a suitable target to move towards. Probably pick a random of the closest navigation nodes, prioritizing those that was not just checked.
    }

    //Returns the closest visible grunt.
    private GruntController FindClosestVisibleGrunt()
    {
        var grunts = FindObjectsByType<GruntController>(FindObjectsSortMode.None); //TODO: Replace with a reference to the active grunts on the Grunt Object Pool.
        var validGrunts = grunts
            .Where(grunt => Vector3.Distance(transform.position, grunt.transform.position) < viewDistance)
            .Where(grunt =>
                Physics.Raycast(transform.position, Vector3.Normalize(grunt.transform.position - transform.position),
                    out var hitInfo) && hitInfo.transform == grunt.transform);//TODO: Switch to raycasting only against terrain.
        
        if (validGrunts.Any())
            return validGrunts.Aggregate((closest, next) =>
                Vector3.Distance(transform.position, next.transform.position) <
                Vector3.Distance(transform.position, closest.transform.position) ? next : closest);
        return null;
    }

    private void CreateCluster(GruntController otherGrunt)
    {
        //TODO: Implement
        //Create a new cluster. Position it in the average of this and the other grunt's position.
        //Put this and the other grunt in the cluster.
    }

    public void PutInCluster(GruntClusterController cluster)
    {
        _cluster = cluster;
        cluster.AddGrunt(this);
    }

    public void RemoveFromCluster()
    {
        if (!_cluster) return;
        
        _cluster.RemoveGrunt(this);
        _cluster = null;
    }

    public void ExplodeCluster(Vector3 centerOfCluster)
    {
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
        if (_cluster && _cluster.IsProperCluster())
        {
            _cluster.TakeDamage(amount);
            return;
        }

        _currentHealth -= amount;
        
        if (_currentHealth > 0f) return;
        
        if (TryGetComponent<EnemyDeathBase>(out var deathScript)) deathScript.OnDeath();
        else Destroy(gameObject);
    }
}