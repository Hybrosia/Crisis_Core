using UnityEngine;

public class TrapAltFire : MonoBehaviour
{
    private SphereCollider _collider;
    private Rigidbody _rigidbody;
    [SerializeField] private GameObject trapBubblePrefab;
    
    
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
                var bubble = ObjectPoolController.SpawnFromPrefab(trapBubblePrefab);
                bubble.transform.position = hit.transform.position;
                bubble.GetComponent<TrapBubbleController>().Trap(hit.gameObject);

                ObjectPoolController.DeactivateInstance(gameObject);
            }
        }
        else if (hit.CompareTag("Terrain") || hit.CompareTag("Wall"))
        {
            ObjectPoolController.DeactivateInstance(gameObject);
        }
    }
}
