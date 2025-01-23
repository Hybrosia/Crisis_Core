using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class GruntClusterController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float baseHealth, healthPerGrunt, baseDamage, damagePerGrunt, meleeRange, maxGruntDistanceWhileMoving;
    [SerializeField] private int numberRequiredToCluster;
    
    private float _currentDamage, _standardSpeed;
    private List<GruntController> _grunts = new List<GruntController>();
    private Vector3 _lastKnownPlayerPosition;

    private void Start()
    {
        _standardSpeed = agent.speed;
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
            && _grunts.Any(grunt => Vector3.Distance(grunt.transform.position, _lastKnownPlayerPosition) < 0.05f)) ;//TODO: Search
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
                         Time.time < grunt.attackTimer))
            {
                grunt.StartAttack();
            }
        //TODO: Search for another cluster or more grunts, i.e., target the closest visible grunt. If in range of a cluster, merge the clusters into one.
        //If in range of a single grunt, the grunt should join the group on their own.
    }

    //Makes all grunts attack at the same time.
    private void AttackAsCluster()
    {
        foreach (var grunt in _grunts) grunt.StartAttack();
        //TODO: If any one of them hits, deal the total damage of all of them.
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
        if (_grunts.Contains(grunt)) return;
        
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
}
