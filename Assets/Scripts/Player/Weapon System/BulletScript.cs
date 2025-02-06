using System;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public WeaponStats weaponStats;
    
    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }
    private void OnTriggerEnter(Collider other)
    {
        /*if (col.TryGetComponent(out EnemyHealthManager enemy))
        {
            enemy.TakeDamage(_weaponStats, false);
            if (col.TryGetComponent(out EnemyActivation activation)) activation.Activate();
            if (col.TryGetComponent(out EnemyMovementHandler movement))
            {
                movement.StartKnockback(_rigidBody.velocity.normalized * _weaponStats.knockbackSpeed);
            }
        }*/
        
        /*enemy.takeDamage settes opp som følgende: 
        public virtual void TakeDamage(WeaponStats weaponStats)
    {
        CurrentHealth -= weaponStats.weaponDamage;
        */
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        Destroy(gameObject);
    }
}
