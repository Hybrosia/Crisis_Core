using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GruntClusterController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float baseHealth, healthPerGrunt, baseDamage, damagePerGrunt, meleeRange, maxGruntDistanceWhileMoving, joinClusterRadius;
    [SerializeField] private int numberRequiredToCluster;
    
    private float _currentDamage, _standardSpeed;
    private List<GruntController> _grunts = new List<GruntController>();
    private Vector3 _lastKnownPlayerPosition;

    private void Start()
    {
        _standardSpeed = agent.speed;
    }

    private void OnDisable()
    {
        _grunts.Clear();
    }

    private void Update()
    {
        if (_grunts.Any(grunt => Vector3.Distance(grunt.transform.position, transform.position) > maxGruntDistanceWhileMoving))
            agent.speed = 0f;
        else agent.speed = _standardSpeed;
        
        if (_grunts.Count == 0) return;
        
        var canSeePlayer = _grunts.Any(grunt => playerData.CanSeePlayerFromPoint(grunt.transform.position));

        _lastKnownPlayerPosition = _grunts
            .Aggregate((newest, next) => newest.lastSawPlayerTime > next.lastSawPlayerTime ? newest : next)
            .lastKnownPlayerPosition;

        if (IsProperCluster()) UpdateAsCluster(canSeePlayer);
        else UpdateAsTooFew(canSeePlayer);
    }

    private void UpdateAsCluster(bool canSeePlayer)
    {
        if (!canSeePlayer
            && _grunts.Any(grunt => Vector3.Distance(grunt.transform.position, _lastKnownPlayerPosition) < 0.05f))
        {
            agent.SetDestination(_grunts[0].FindNextSearchTarget());
        }
        else if (canSeePlayer 
                 && _grunts.Any(grunt => Vector3.Distance(grunt.transform.position, playerData.PlayerPos) < meleeRange) 
                 && _grunts.All(grunt => Time.time >= grunt.attackTimer)) AttackAsCluster();
        else if (canSeePlayer) agent.SetDestination(_lastKnownPlayerPosition);
    }

    private void UpdateAsTooFew(bool canSeePlayer)
    {
        if (canSeePlayer)
            foreach (var grunt in _grunts.Where(grunt => 
                         Vector3.Distance(grunt.transform.position, playerData.PlayerPos) < meleeRange && 
                         Time.time > grunt.attackTimer))
            {
                grunt.StartAttack();
            }
        
        if (agent.remainingDistance > 0.1f) return;

        GruntController currentClosestGrunt = null;
        var currentSmallestDistance = float.PositiveInfinity;
        
        foreach (var grunt in _grunts)
        {
            var potentialClosestGrunt = grunt.FindClosestVisibleGrunt();
            if (potentialClosestGrunt &&
                Vector3.Distance(potentialClosestGrunt.transform.position, grunt.transform.position) < currentSmallestDistance) 
                currentClosestGrunt = potentialClosestGrunt;
        }
        
        if (currentClosestGrunt && currentClosestGrunt.cluster && currentClosestGrunt.cluster != this &&
            Vector3.Distance(transform.position, currentClosestGrunt.transform.position) <= joinClusterRadius &&
            currentClosestGrunt.state is GruntController.GruntState.Searching or GruntController.GruntState.Afraid)
        {
            MergeClusters(currentClosestGrunt.cluster);
        }
        else if (currentClosestGrunt) agent.SetDestination(currentClosestGrunt.transform.position);
        else agent.SetDestination(_grunts[0].FindNextSearchTarget());
    }

    //Makes all grunts attack at the same time.
    private void AttackAsCluster()
    {
        foreach (var grunt in _grunts) grunt.StartAttack();
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(baseDamage + damagePerGrunt * _grunts.Count);
    }

    public bool IsProperCluster()
    {
        return _grunts.Count >= numberRequiredToCluster;
    }

    public void AddGrunt(GruntController grunt)
    {
        if (_grunts.Contains(grunt)) return;
        
        _grunts.Add(grunt);
    }

    public void RemoveGrunt(GruntController grunt)
    {
        if (!_grunts.Contains(grunt)) return;
        
        _grunts.Remove(grunt);

        if (_grunts.Count == 0) ObjectPoolController.DeactivateInstance(gameObject);
    }
    
    //Takes damage.
    public void TakeDamage(float amount)
    {
        _currentDamage += amount;
        
        if (_currentDamage < baseHealth + healthPerGrunt * _grunts.Count) return;

        var centerOfCluster =
            _grunts.Select(grunt => grunt.transform.position)
                .Aggregate(Vector3.zero, (sum, position) => sum + position) / _grunts.Count;
        
        foreach (var grunt in _grunts) grunt.ExplodeCluster(centerOfCluster);
        
        ObjectPoolController.DeactivateInstance(gameObject);
    }

    //Registers all the grunts in the other cluster to this cluster and removes the other cluster.
    private void MergeClusters(GruntClusterController otherCluster)
    {
        otherCluster._grunts.ForEach(grunt => grunt.PutInCluster(this));
        
        ObjectPoolController.DeactivateInstance(otherCluster.gameObject);
    }
}
