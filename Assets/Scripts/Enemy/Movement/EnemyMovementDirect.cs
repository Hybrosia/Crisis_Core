using System;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovementDirect : EnemyMovementBase
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private float periodBetweenRaycasts = 0.2f;
    
    private NavMeshAgent _agent;
    private float _timer;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    public void OnEnable()
    {
        UpdateTargetPosition();
        _timer = Time.time;
        _agent.isStopped = false;
    }

    private void UpdateTargetPosition()
    {
        if (playerData.CanSeePlayerFromPoint(transform.position))
        {
            _agent.SetDestination(playerData.PlayerPos);
        }
    }

    private void Update()
    {
        if (Time.time >= _timer + periodBetweenRaycasts)
        {
            UpdateTargetPosition();
            _timer = Time.time;
        }

        if (_agent.remainingDistance < 0.1f && !playerData.CanSeePlayerFromPoint(transform.position))
        {
            NoTargetFound.Invoke();
            enabled = false;
        }
    }

    public void OnDisable()
    {
        StopAllCoroutines();
        _agent.isStopped = true;
    }
}
