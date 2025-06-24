using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class SpewerController : MonoBehaviour, IEnemyHealthManager
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private float startMeleeRange, meleeRange, startCloseRange, closeRange, crashDamageRange, jumpDistance, jumpChance, biteDamage, crashDamage;
    [SerializeField] private float biteAttackCooldown, burpAttackCooldown, projectileAttackCooldown, chargeTime, staggerTime, staggerDamageThreshold;
    [SerializeField] private float maxHealth;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject burpTrigger;
    [SerializeField] private float[] jumpAngles;
    
    private float _currentHealth;
    private SpewerState _state;
    private float _attackTimer, _staggerTimer, _chargeTimer, _damageTakenWhileCharging;
    private Vector3 _lastKnownPlayerPosition;
    
    private enum SpewerState
    {
        Idle,
        Moving,
        Jumping,
        Charging,
        Staggered,
        Biting,
        Burping,
        ProjectileAttacking
    }

    private void OnEnable()
    {
        ResetHealth();
        burpTrigger.SetActive(false);
        SetIdle();
    }

    private void Update()
    {
        var canSeePlayer = playerData.CanSeePlayerFromPoint(transform.position);

        if (canSeePlayer)
        {
            _lastKnownPlayerPosition = playerData.PlayerPos;
        }
        
        if (_state == SpewerState.Idle) Idle(canSeePlayer);
        else if (_state == SpewerState.Moving) Moving(canSeePlayer);
        else if (_state == SpewerState.Jumping) Jumping(canSeePlayer);
        else if (_state == SpewerState.Charging) Charging(canSeePlayer);
        else if (_state == SpewerState.Staggered) Staggered(canSeePlayer);
        else if (_state == SpewerState.Biting) Biting(canSeePlayer);
        else if (_state == SpewerState.Burping) Burping(canSeePlayer);
        else if (_state == SpewerState.ProjectileAttacking) ProjectileAttacking(canSeePlayer);
    }

    private void SetIdle()
    {
        _state = SpewerState.Idle;
        animator.Play("Idle");
        agent.isStopped = true;
    }

    private void Idle(bool canSeePlayer)
    {
        if (canSeePlayer) SetMoving();
    }

    private void SetMoving()
    {
        _state = SpewerState.Moving;
        animator.Play("Move");
        agent.isStopped = false;
    }

    private void Moving(bool canSeePlayer)
    {
        if (!canSeePlayer && Vector3.Distance(transform.position, _lastKnownPlayerPosition) < 0.05f) SetIdle();
        else if (canSeePlayer && Vector3.Distance(transform.position, playerData.PlayerPos) < startMeleeRange) SetBiting();
        else if (canSeePlayer && Vector3.Distance(transform.position, playerData.PlayerPos) < startCloseRange) SetBurping();
        else if (canSeePlayer)
        {
            agent.SetDestination(_lastKnownPlayerPosition);
        }
    }

    private void SetJumping()
    {
        _state = SpewerState.Jumping;
        SetJumpTarget();
        agent.isStopped = false;
    }

    private void Jumping(bool canSeePlayer)
    {
        if (agent.remainingDistance > 0.05f) return;

        SetMoving();
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
        if (Time.time > _attackTimer)
        {
            _attackTimer = Time.time + biteAttackCooldown;
            animator.StopPlayback();
            animator.Play("AttackJump");
        }
        else
        {
            animator.StopPlayback();
            animator.Play("Jump");
        }
    }
    
    private void SetCharging()
    {
        _state = SpewerState.Charging;
        animator.Play("Charge");
        agent.isStopped = true;

        _chargeTimer = Time.time + chargeTime;
        _damageTakenWhileCharging = 0f;
    }

    private void Charging(bool canSeePlayer)
    {
        if (_damageTakenWhileCharging >= staggerDamageThreshold)
        {
            SetStaggered();
            return;
        }
        
        if (_chargeTimer < Time.time) return;
        
        SetProjectileAttacking();
    }
    
    private void SetBiting()
    {
        _state = SpewerState.Biting;
        animator.Play("Bite");
        agent.isStopped = false;

        _attackTimer = Time.time + biteAttackCooldown;
    }

    private void Biting(bool canSeePlayer)
    {
        if (Time.time < _attackTimer) return;
        
        SetMoving();
    }
    
    private void SetBurping()
    {
        _state = SpewerState.Burping;
        animator.Play("Burp");
        agent.isStopped = true;

        _attackTimer = Time.time + burpAttackCooldown;
        burpTrigger.SetActive(true);
    }

    private void Burping(bool canSeePlayer)
    {
        if (Time.time < _attackTimer) return;
        
        burpTrigger.SetActive(false);
        SetMoving();
    }
    
    private void SetProjectileAttacking()
    {
        _state = SpewerState.ProjectileAttacking;
        animator.Play("ProjectileAttack");
        agent.isStopped = true;

        _attackTimer = Time.time + projectileAttackCooldown;
    }

    private void ProjectileAttacking(bool canSeePlayer)
    {
        if (Time.time < _attackTimer) return;
        
        SetMoving();
    }
    
    private void SetStaggered()
    {
        _state = SpewerState.Staggered;
        animator.Play("Stagger");
        agent.isStopped = true;

        _staggerTimer = Time.time + staggerTime;
    }

    private void Staggered(bool canSeePlayer)
    {
        if (Time.time < _staggerTimer) return;
        
        SetMoving();
    }

    //Damages player if too close to impact. Trigger from animation event.
    public void OnGroundHit()
    {
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > crashDamageRange) return;
        
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(crashDamage);
    }
    
    //Tries to damage the player. Trigger from animation event.
    private void DoBiteAttack()
    {
        if (Vector3.Distance(transform.position, playerData.PlayerPos) > meleeRange) return;
        
        playerData.player.GetComponent<PlayerHealthManager>().TakeDamage(biteDamage);
    }

    //Spawns a projectile and fires it towards the player. Trigger from animation event.
    public void DoProjectileAttack()
    {
        var projectile = ObjectPoolController.SpawnFromPrefab(projectilePrefab);

        projectile.transform.position = spawnPoint.position;
        projectile.transform.rotation = transform.rotation;
        projectile.GetComponent<EnemyProjectile>().Fire();
    }
    
    private void ResetHealth()
    {
        _currentHealth = maxHealth;
    }

    //Takes damage.
    public void TakeDamage(float amount)
    {
        if (_state is SpewerState.Idle or SpewerState.Moving && playerData.CanSeePlayerFromPoint(transform.position))
        {
            if (Random.Range(0f, 1f) > jumpChance) SetJumping();
            else SetCharging();
        }
        
        //TODO: If hit with bubble rush, set _damageTakenWhileCharging to staggerDamageThreshold.
        if (_state == SpewerState.Charging) _damageTakenWhileCharging += amount;
        
        _currentHealth -= amount;
        
        if (_currentHealth > 0f) return;
        
        if (TryGetComponent<EnemyDeathBase>(out var deathScript)) deathScript.OnDeath();
        else ObjectPoolController.DeactivateInstance(gameObject);
    }
}