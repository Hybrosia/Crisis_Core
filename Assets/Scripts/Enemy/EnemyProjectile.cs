using System;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float damage;
    [SerializeField] private float initialSpeed, fireAngle;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Fire()
    {
        _rigidbody.linearVelocity = Quaternion.Euler(fireAngle, 0f, 0f) * Vector3.forward * initialSpeed;
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
