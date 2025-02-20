using UnityEngine;
using UnityEngine.AI;

public class GhoulController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private float startMeleeRange, meleeRange, attackCooldown, damage;
    
    private GhoulState _state;
    private float _attackTimer;
    
    private enum GhoulState
    {
        Idle,
        Moving,
        Attacking
    }

    private void OnEnable()
    {
        SetIdle();
    }
    
    private void Update()
    {
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
        _state = GhoulState.Idle;
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
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > meleeRange) return;
        
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(damage);
    }
}
