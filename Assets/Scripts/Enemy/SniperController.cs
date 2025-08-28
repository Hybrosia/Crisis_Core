using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SniperController : MonoBehaviour, IEnemyTrapManager
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private float aimDuration, searchDuration, reloadDuration, minimumPlayerDistance, damage;
    [SerializeField] private LayerMask terrain;
    [SerializeField] private LayerMask playerAndBubbles;
    [SerializeField] private GameObject bounceBubblePrefab;

    private SniperState _state;
    private SniperPosition _currentPosition;
    private float _timer;
    private Transform _target;
    private Vector3 _lastSeenMovementVector;
    private bool _isTrapped;
    
    public enum SniperState
    {
        Idle,
        Targeting,
        NoTarget,
        Reloading
    }
    
    private void OnEnable()
    {
        _isTrapped = false;
        SniperPosition closestPosition = null;
        var distanceToClosest = float.PositiveInfinity;
        foreach (var sniperPosition in SniperPosition.ActiveSniperPositions)
        {
            var distance = Vector3.Distance(sniperPosition.transform.position, transform.position);
            if (distance >= distanceToClosest) continue;

            distanceToClosest = distance;
            closestPosition = sniperPosition;
        }

        if (!closestPosition)
        {
            print("No SniperPositions");
            ObjectPoolController.DeactivateInstance(gameObject);
            return;
        }

        SetPosition(closestPosition);
    }

    private void Update()
    {
        if (_isTrapped)
        {
            UpdateWhileTrapped();
            return;
        }

        if (_state == SniperState.Idle) Idle();
        else if (_state == SniperState.Targeting) Targeting();
        else if (_state == SniperState.NoTarget) NoTarget();
        else if (_state == SniperState.Reloading) Reloading();
    }

    private void SetIdle()
    {
        _state = SniperState.Idle;
        animator.Play("Idle");
        _target = null;
    }

    private void Idle()
    {
        var target = ChooseAimTarget();
        
        if (target) SetTargeting(target);
    }
    
    private void SetTargeting(Transform target)
    {
        _state = SniperState.Targeting;
        _target = target;
        _timer = Time.time + aimDuration;
    }

    private void Targeting()
    {
        if (!_target || Physics.Raycast(transform.position, (_target.position - transform.position).normalized,
                out _, Vector3.Distance(_target.position, transform.position), terrain))
        {
            var target = ChooseAimTarget();
            
            if (target) SetTargeting(target);
            else
            {
                SetNoTarget();
            }
        }
        
        if (Time.time < _timer) return;

        DoAttack();
        SetReloading();
    }
    
    private void SetNoTarget()
    {
        _state = SniperState.NoTarget;
        _target = null;
        _timer = Time.time + searchDuration;
    }

    private void NoTarget()
    {
        var target = ChooseAimTarget();

        if (target)
        {
            SetTargeting(target);
            return;
        }
        
        if (Time.time < _timer) return;

        FindNewPosition();
        target = ChooseAimTarget();
        
        if (target) SetTargeting(target);
        else SetIdle();
    }
    
    private void SetReloading()
    {
        _state = SniperState.Reloading;
        _target = null;
        _timer = Time.time + reloadDuration;
        
        if (Vector3.Distance(playerData.PlayerPos, transform.position) < minimumPlayerDistance) FindNewPosition();
    }

    private void Reloading()
    {
        if (Time.time < _timer) return;

        var target = ChooseAimTarget();
        
        if (target) SetTargeting(target);
        else SetNoTarget();
    }

    private void SetPosition(SniperPosition newPosition)
    {
        _currentPosition = newPosition;
        transform.position = newPosition.transform.position;
    }

    private Transform ChooseAimTarget()
    {
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);
        var playerMovement = playerData.player.GetComponentInChildren<Movement>();

        if (canSeePlayer && playerMovement.IsGrounded) return playerData.player;

        var bubbles = ObjectPoolController.GetActiveObjects(bounceBubblePrefab);
        if (bubbles != null && bubbles.Count > 0)
        {
            var playerVelocity = playerMovement.GetHorizontalSpeed();
            if (playerVelocity == Vector3.zero)
                return bubbles.OrderByDescending(bubble =>
                        Vector3.Distance(bubble.transform.position, playerData.PlayerPos))
                    .First().transform;
            
            return bubbles.OrderByDescending(bubble =>
                    Vector3.Dot(bubble.transform.position - playerData.PlayerPos, playerVelocity))
                .First().transform;
        }

        if (canSeePlayer) return playerData.player;

        return null;
    }

    //Returns a new SniperPosition. If validJumps is populated, chooses one of them. If not, selects one of the active sniper positions at random.
    //If not possible, returns itself.
    private void FindNewPosition()
    {
        if (_currentPosition.validJumps.Count > 0) SetPosition(_currentPosition.validJumps[Random.Range(0, _currentPosition.validJumps.Count)]);
        else if (SniperPosition.ActiveSniperPositions.Count == 0) SetPosition(_currentPosition);
        else
        {
            var validPositions = SniperPosition.ActiveSniperPositions.ToList();
            validPositions.Remove(_currentPosition);

            if (validPositions.Count == 0) SetPosition(_currentPosition);
            else SetPosition(validPositions[Random.Range(0, validPositions.Count)]);
        }
    }

    private void DoAttack()
    {
        var hitTarget = _target;
        
        var position = transform.position;
        if (Physics.Raycast(position, (_target.position - position).normalized,
            out var hit, Vector3.Distance(_target.position, position), playerAndBubbles))
        {
            hitTarget = hit.transform;
        }
        
        if (hitTarget.CompareTag("Player"))
        {
            hitTarget.GetComponentInChildren<PlayerHealthManager>().TakeDamage(damage);
            hitTarget.GetComponentInChildren<Movement>().DisableFloat();
        }
        else
        {
            Destroy(hitTarget.gameObject);
        }
    }
    
    public void Trap()
    {
        _isTrapped = true;
        animator.speed = 0f;
    }

    public void Untrap()
    {
        _isTrapped = false;
        animator.speed = 1f;
        SetPosition(_currentPosition);
    }

    private void UpdateWhileTrapped()
    {
        _timer += Time.deltaTime;
    }
}
