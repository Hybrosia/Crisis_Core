using UnityEngine;
using UnityEngine.AI;

public class GhoulController : MonoBehaviour, IEnemyTrapManager
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float startMeleeRange, meleeRange, attackCooldown, damage;
    
    private GhoulState _state;
    private float _attackTimer;
    private bool _isTrapped, _preTrappedMovingState;
    
    private enum GhoulState
    {
        Idle,
        Moving,
        Attacking
    }

    private void OnEnable()
    {
        SetIdle();
        _isTrapped = false;
    }
    
    private void Update()
    {
        if (_isTrapped)
        {
            UpdateWhileTrapped();
            return;
        }

        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (_state == GhoulState.Idle) Idle(canSeePlayer);
        else if (_state == GhoulState.Moving) Moving(canSeePlayer);
        else if (_state == GhoulState.Attacking) Attacking(canSeePlayer);
    }

    private void SetIdle()
    {
        _state = GhoulState.Idle;
        animator.Play("Idle");
        agent.isStopped = true;
    }

    private void Idle(bool canSeePlayer)
    {
        if (canSeePlayer) SetMoving();
    }
    
    private void SetMoving()
    {
        _state = GhoulState.Moving;
        animator.Play("Move");
        agent.isStopped = false;
    }

    private void Moving(bool canSeePlayer)
    {
        if (Time.time > _attackTimer && Vector3.Distance(transform.position, playerData.PlayerPos) < startMeleeRange) SetAttacking();
        
        agent.SetDestination(playerData.PlayerPos);
    }
    
    private void SetAttacking()
    {
        print("Start attack");
        _state = GhoulState.Attacking;
        animator.Play("Attack");
        agent.isStopped = false;

        _attackTimer = Time.time + attackCooldown;
    }

    private void Attacking(bool canSeePlayer)
    {
        
    }
    
    //Tries to damage the player. Trigger from animation event.
    private void DoMeleeAttack()
    {
        print("ATTACK");
        SetMoving();

        if (Vector3.Distance(transform.position, playerData.PlayerPos) > meleeRange) return;
        
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(damage);
    }
    
    public void Trap()
    {
        _isTrapped = true;
        _preTrappedMovingState = agent.isStopped;
        agent.isStopped = true;
        animator.speed = 0f;
    }

    public void Untrap()
    {
        _isTrapped = false;
        agent.Warp(transform.position);
        agent.isStopped = _preTrappedMovingState;
        animator.speed = 1f;
    }

    private void UpdateWhileTrapped()
    {
        _attackTimer += Time.deltaTime;
    }
}
