using System;
using UnityEngine;
using UnityEngine.AI;

public class SandSharkController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private ParticleSystem sandParticles;
    [SerializeField] private GameObject hitBox;
    [SerializeField] private float attackDuration, stunDuration, diveRange, attackRange, damage, launchForce;

    private SandSharkState _state;
    private Vector3 _lastKnownPlayerPosition;
    private float _attackTimer, _stunTimer;

    private enum SandSharkState
    {
        Idle,
        Moving,
        Diving,
        Attacking,
        Stunned
    }
    
    private void OnEnable()
    {
        sandParticles.Stop();
        sandParticles.Clear();
        audioSource.Stop();
        SetIdle();
    }

    private void Update()
    {
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (canSeePlayer)
        {
            _lastKnownPlayerPosition = playerData.PlayerPos;
        }
        
        if (_state == SandSharkState.Idle) Idle(canSeePlayer);
        else if (_state == SandSharkState.Moving) Moving(canSeePlayer);
        else if (_state == SandSharkState.Diving) Diving(canSeePlayer);
        else if (_state == SandSharkState.Attacking) Attacking(canSeePlayer);
        else if (_state == SandSharkState.Stunned) Stunned(canSeePlayer);
    }

    private void SetIdle()
    {
        _state = SandSharkState.Idle;
        sandParticles.Stop();
        hitBox.SetActive(true);
        animator.Play("Idle");
    }
    
    private void Idle(bool canSeePlayer)
    {
        if (canSeePlayer) SetMoving();
    }
    
    private void SetMoving()
    {
        _state = SandSharkState.Moving;
        sandParticles.Play();
        hitBox.SetActive(false);
        animator.Play("Idle");
        audioSource.Play();
    }

    private void Moving(bool canSeePlayer)
    {
        if (Vector3.Distance(transform.position, playerData.PlayerPos) < diveRange) SetDiving();
        
        if (canSeePlayer) agent.SetDestination(playerData.PlayerPos);
        else if (agent.remainingDistance < 0.1f) SetIdle();
    }
    
    private void SetDiving()
    {
        _state = SandSharkState.Diving;
        sandParticles.Stop();
        hitBox.SetActive(false);
        audioSource.Stop();
        agent.SetDestination(playerData.PlayerPos);
    }

    private void Diving(bool canSeePlayer)
    {
        if (agent.remainingDistance < 0.1f) SetAttacking();
    }

    private void SetAttacking()
    {
        _state = SandSharkState.Attacking;
        sandParticles.Stop();
        hitBox.SetActive(false);
        agent.isStopped = false;

        _attackTimer = Time.time + attackDuration;
        DoAttack();
    }
    
    private void Attacking(bool canSeePlayer)
    {
        if (Time.time > _attackTimer) SetStunned();
    }
    
    private void SetStunned()
    {
        _state = SandSharkState.Stunned;
        sandParticles.Stop();
        hitBox.SetActive(true);

        _stunTimer = Time.time + stunDuration;
    }
    
    private void Stunned(bool canSeePlayer)
    {
        if (Time.time > _stunTimer) SetMoving();
    }

    private void DoAttack()
    {
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > attackRange) return;
        
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(damage);
        playerData.player.GetComponent<Movement>().AddForce(launchForce * Vector3.up);
        
        animator.Play("Attack");
    }
}
