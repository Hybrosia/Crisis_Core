using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpitterController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float startJumpRange, endJumpRange, attackCooldown, jumpDistance;
    [SerializeField] private float[] jumpAngles;
    
    private SpitterState _state;
    private float _attackTimer;
    private Vector3 _lastKnownPlayerPosition;
    
    private enum SpitterState
    {
        Idle,
        Moving,
        Jumping
    }

    private void OnEnable()
    {
        SetState(SpitterState.Idle);
    }

    private void Update()
    {
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (canSeePlayer)
        {
            _lastKnownPlayerPosition = playerData.PlayerPos;
        }
        
        if (_state == SpitterState.Idle) Idle(canSeePlayer);
        else if (_state == SpitterState.Moving) Moving(canSeePlayer);
        else if (_state == SpitterState.Jumping) Jumping(canSeePlayer);
    }

    private void SetState(SpitterState newState)
    {
        _state = newState;
        
        if (_state == SpitterState.Idle) SetIdle();
        if (_state == SpitterState.Moving) SetMoving();
        if (_state == SpitterState.Jumping) SetJumping();
    }

    private void SetIdle()
    {
        _state = SpitterState.Idle;
        animator.Play("Idle");
        agent.isStopped = true;
    }

    private void Idle(bool canSeePlayer)
    {
        if (canSeePlayer) SetMoving();
    }

    private void SetMoving()
    {
        _state = SpitterState.Moving;
        animator.Play("Move");
        agent.isStopped = false;
    }

    private void Moving(bool canSeePlayer)
    {
        if (!canSeePlayer && Vector3.Distance(transform.position, _lastKnownPlayerPosition) < 0.05f) SetIdle();
        else if (Vector3.Distance(transform.position, playerData.PlayerPos) < startJumpRange) SetJumping();
        else if (canSeePlayer)
        {
            agent.SetDestination(_lastKnownPlayerPosition);
        }
    }
    
    private void SetJumping()
    {
        _state = SpitterState.Jumping;
        SetJumpTarget();
        agent.isStopped = false;
    }

    private void Jumping(bool canSeePlayer)
    {
        if (agent.remainingDistance > 0.05f) return;
        
        if (!canSeePlayer || Vector3.Distance(transform.position, playerData.PlayerPos) > endJumpRange)
        {
            SetMoving();
            return;
        }

        SetJumpTarget();
    }

    //Select one of the possible locations at random, but only one that can be reached.
    private void SetJumpTarget()
    {
        var targetPosition = Vector3.zero;
        var directionTowardsPlayer = (playerData.PlayerPos - transform.position).normalized;
        
        var index = Random.Range(0, jumpAngles.Length);
        for (int i = 0; i < jumpAngles.Length; i++)
        {
            index++;
            if (index >= jumpAngles.Length) index = 0;

            var checkTargetPosition = transform.position + Quaternion.AngleAxis(jumpAngles[index], Vector3.up) * directionTowardsPlayer * jumpDistance;
            
            if (agent.Raycast(checkTargetPosition, out var hit)) continue;
            
            targetPosition = checkTargetPosition;
            break;
        }

        if (targetPosition == Vector3.zero) targetPosition = transform.position + Quaternion.AngleAxis(180f, Vector3.up) * directionTowardsPlayer;
        
        agent.SetDestination(targetPosition);
        if (Time.time >= _attackTimer)
        {
            _attackTimer = Time.time + attackCooldown;
            animator.StopPlayback();
            animator.Play("AttackJump");
        }
        else
        {
            animator.StopPlayback();
            animator.Play("Jump");
        }
    }

    public void DoAttack()
    {
        //Spawn projectile and fire towards the player.
    }
}