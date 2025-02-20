using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class RusherController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float startMeleeRange, startChargeRange, maxChargeDistance, avoidPlayerRadius, colliderRadius;
    [SerializeField] private float rushSpeed, chargeAttackCooldown, meleeAttackCooldown, chargeTime, staggerTime;
    [SerializeField] private float maxHealth;
    
    private float _currentHealth;
    private RusherState _state;
    private float _attackTimer, _staggerTimer, _chargeTimer;
    private Vector3 _lastKnownPlayerPosition, _startRushPoint, _rushDirection;
    private List<NavigationPoint> _currentPath = new List<NavigationPoint>();

    private enum RusherState
    {
        Idle,
        Moving,
        Charging,
        Rushing,
        Staggered,
        Attacking
    }
    
    private void OnEnable()
    {
        ResetHealth();
        SetIdle();
    }

    private void Update()
    {
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (canSeePlayer)
        {
            _lastKnownPlayerPosition = playerData.PlayerPos;
        }
        
        if (_state == RusherState.Idle) Idle(canSeePlayer);
        else if (_state == RusherState.Moving) Moving(canSeePlayer);
        else if (_state == RusherState.Charging) Charging(canSeePlayer);
        else if (_state == RusherState.Rushing) Rushing(canSeePlayer);
        else if (_state == RusherState.Staggered) Staggered(canSeePlayer);
        else if (_state == RusherState.Attacking) Attacking(canSeePlayer);
    }

    private void SetIdle()
    {
        _state = RusherState.Idle;
        animator.Play("Idle");
        agent.isStopped = true;
    }

    private void Idle(bool canSeePlayer)
    {
        if (canSeePlayer) SetMoving();
    }
    
    private void SetMoving()
    {
        agent.Warp(transform.position);
        
        _state = RusherState.Moving;
        animator.Play("Move");
        agent.isStopped = false;
        
        SetNewPath();
    }

    //Moves to flank the player.
    //If it can see the player, is in range and nothing is in the way, starts charging.
    //If it is in melee range of the player, performs a melee attack.
    private void Moving(bool canSeePlayer)
    {
        if (canSeePlayer && Time.time > _attackTimer && Vector3.Distance(transform.position, _lastKnownPlayerPosition) < startMeleeRange) SetAttacking();
        else if (canSeePlayer && Time.time > _attackTimer  && Vector3.Distance(transform.position, _lastKnownPlayerPosition) < startChargeRange && CanMoveTowardsPlayer()) SetCharging();
        else if (canSeePlayer && Vector3.Dot(playerData.player.forward, (transform.position - playerData.PlayerPos).normalized) > 1.6f) SetNewPath();
        else if (agent.remainingDistance < 0.2f)
        {
            if (_currentPath?.Count > 1)
            {
                _currentPath.RemoveAt(0);
                agent.SetDestination(_currentPath[0].transform.position);
            }
            else if (canSeePlayer) agent.SetDestination(playerData.PlayerPos);
            else SetIdle();
        }
    }
    
    private void SetCharging()
    {
        _state = RusherState.Charging;
        animator.Play("Charge");
        agent.isStopped = true;

        _chargeTimer = Time.time + chargeTime;
    }

    private void Charging(bool canSeePlayer)
    {
        if (Time.time < _chargeTimer) return;

        SetRushing();
    }
    
    private void SetRushing()
    {
        _state = RusherState.Rushing;
        animator.Play("Rush");
        agent.isStopped = true;

        _attackTimer = Time.time + chargeAttackCooldown;
        _startRushPoint = transform.position;
        _rushDirection = (playerData.PlayerPos - transform.position).normalized;
        
        agent.ResetPath();
    }

    private void Rushing(bool canSeePlayer)
    {
        if (Vector3.Distance(_startRushPoint, transform.position) > maxChargeDistance) SetMoving();
    }
    
    private void SetStaggered()
    {
        _state = RusherState.Staggered;
        animator.Play("Stagger");
        agent.isStopped = true;

        _staggerTimer = Time.time + staggerTime;
    }

    private void Staggered(bool canSeePlayer)
    {
        if (Time.time < _staggerTimer) return;
        
        SetMoving();
    }
    
    private void SetAttacking()
    {
        _state = RusherState.Attacking;
        animator.Play("Attack");
        agent.isStopped = false;

        _attackTimer = Time.time + meleeAttackCooldown;
    }

    private void Attacking(bool canSeePlayer)
    {
        
    }
    
    private void DoMeleeAttack()
    {
        //Check for damage. Trigger from animation event.
    }

    private void FixedUpdate()
    {
        if (_state != RusherState.Rushing) return;

        transform.position += rushSpeed * Time.fixedDeltaTime * _rushDirection;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_state != RusherState.Rushing) return;
        if (col.CompareTag("Player"))
        {
            //TODO: Set knockback on the player and deal damage.
            SetMoving();
        }
        else if (col.CompareTag("Terrain"))
        {
            SetStaggered();
        }
    }

    private void SetNewPath()
    {
        _currentPath = GetPathAroundPlayer();
        agent.SetDestination(_currentPath[0].transform.position);
    }
    
    //Returns the shortest path from the closest navigation point to the closest navigation point to a point behind the player.
    private List<NavigationPoint> GetPathAroundPlayer()
    {
        var closestNavigationPoint = NavigationPoint.FindPointClosestToPosition(transform.position);
        var closestPointBehindPlayer =
            NavigationPoint.FindPointClosestToPosition(playerData.PlayerPos - avoidPlayerRadius * 2f * playerData.player.forward);

        return closestNavigationPoint.FindShortestPathToPoint(closestPointBehindPlayer);
    }

    private bool CanMoveTowardsPlayer()
    {
        if (NavMesh.Raycast(transform.position + transform.forward * colliderRadius, 
                playerData.PlayerPos, out _, NavMesh.AllAreas)) return false;
        if (NavMesh.Raycast(transform.position + transform.right * colliderRadius,
                playerData.PlayerPos + transform.right * colliderRadius, out _, NavMesh.AllAreas)) return false;
        if (NavMesh.Raycast(transform.position - transform.right * colliderRadius,
                playerData.PlayerPos - transform.right * colliderRadius, out _, NavMesh.AllAreas)) return false;
        
        return true;
    }

    private void ResetHealth()
    {
        _currentHealth = maxHealth;
    }

    //Takes damage.
    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        
        if (_currentHealth > 0f) return;
        
        if (TryGetComponent<EnemyDeathBase>(out var deathScript)) deathScript.OnDeath();
        else ObjectPoolController.DeactivateInstance(gameObject);
    }
}
