using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage;
    [SerializeField] private float initialSpeed, fireAngle;
    [SerializeField] private PlayerData playerData;

    [SerializeField] private Rigidbody rb;

    public void Fire()
    {
        var directionTowardsPlayer = (playerData.PlayerPos - transform.position).normalized;
        rb.linearVelocity = Quaternion.Euler(fireAngle, 0f, 0f) * directionTowardsPlayer * initialSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        ObjectPoolController.DeactivateInstance(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
