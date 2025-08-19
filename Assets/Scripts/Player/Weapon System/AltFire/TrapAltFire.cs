using System;
using UnityEngine;

public class TrapAltFire : MonoBehaviour
{
    private SphereCollider _collider;
    private Rigidbody _rigidbody;
    
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _collider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Enemy"))
        {
            if (hit.TryGetComponent(out IEnemyHealthManager enemy))
            {
                
                enemy.Trap();
                ObjectPoolController.DeactivateInstance(gameObject);
            }
        }
        else if (hit.CompareTag("Terrain") || hit.CompareTag("Wall"))
        {
            ObjectPoolController.DeactivateInstance(gameObject);
        }
    }
}
