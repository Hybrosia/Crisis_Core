using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private float attackRange, attackRate;

    private EnemyMovementBase _movement;
    private EnemySearchBase _search;
    private AttackDefault _attack;
    private float _attackTimer;

    private void Awake()
    {
        _movement = GetComponent<EnemyMovementBase>();
        _attack = GetComponent<AttackDefault>();
        TryGetComponent(out _search);
    }

    private void OnEnable()
    {
        _movement.enabled = true;
        _movement.NoTargetFound.AddListener(NoTarget);
        _attackTimer = Time.time;
    }

    private void Update()
    {
        if (Time.time < _attackTimer + attackRate) return;
        if (Vector3.Distance(transform.position, playerData.PlayerPos) >= attackRange) return;
        if (!playerData.CanSeePlayerFromPoint(transform.position)) return;

        _attackTimer = Time.time;
        _attack.enabled = true;
    }

    private void NoTarget()
    {
        if (!_search) return;
        
        _search.enabled = true;
        _movement.enabled = false;
        enabled = false;
    }

    private void OnDisable()
    {
        _movement.enabled = false;
        if (_search) _search.enabled = false;
        _movement.NoTargetFound.RemoveListener(NoTarget);
    }
}
