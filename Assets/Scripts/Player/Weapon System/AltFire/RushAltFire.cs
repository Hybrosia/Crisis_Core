using System;
using Unity.Mathematics;
using UnityEngine;

public class RushAltFire : MonoBehaviour
{

    private SphereCollider _sphereCollider;
    private Rigidbody _rigidbody;
    
    [SerializeField] private LayerMask whatIsTerrain;
    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (GetTerrainNormal() != Vector3.zero)
        {
            print("Found ground");
            var direction = Vector3.Cross(GetTerrainNormal(), transform.right);
            if (Vector3.Cross(direction, transform.forward).magnitude > 0)
            {
                direction *= -1;
            }

            if (Mathf.Abs(direction.y) < 0.5)
            {
                transform.rotation = Quaternion.LookRotation(direction);
                _rigidbody.linearVelocity = transform.forward * _rigidbody.linearVelocity.magnitude;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public Vector3 GetTerrainNormal()
    {
        var position = transform.position;
        if (Physics.SphereCast(position, _sphereCollider.radius - 0.1f, transform.forward, out var hit, _rigidbody.linearVelocity.magnitude * Time.fixedDeltaTime + 0.1f, whatIsTerrain))
            return hit.normal;
        return Vector3.zero;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out IEnemyHealthManager enemy))
            {
                ObjectPoolController.DeactivateInstance(gameObject);
            }
        }
        else if (other.CompareTag("Wall"))
        {
            ObjectPoolController.DeactivateInstance(gameObject);
        }
    }
}
