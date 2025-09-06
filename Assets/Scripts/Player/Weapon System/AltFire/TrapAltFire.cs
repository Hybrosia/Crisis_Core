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
        print(hit);
        if (hit.CompareTag("Enemy"))
        {
            var enemy = hit.GetComponentInParent<IEnemyHealthManager>();
            
            var bubble = ObjectPoolController.SpawnFromPrefab(trapBubblePrefab);
            bubble.transform.position = hit.transform.parent.position;
            bubble.GetComponent<TrapBubbleController>().Trap(hit.transform.parent.gameObject);

            ObjectPoolController.DeactivateInstance(gameObject);
            
        }
        else if (hit.CompareTag("Terrain") || hit.CompareTag("Wall"))
        {
            ObjectPoolController.DeactivateInstance(gameObject);
        }
    }
}
