using UnityEngine;

public class GasCloudController : MonoBehaviour
{
    [SerializeField] private float duration, damagePerSecond;
    private float _despawnTimer;

    private void Start()
    {
        _despawnTimer = Time.time + duration;
    }

    private void Update()
    {
        if (Time.time < _despawnTimer) return;
        ObjectPoolController.DeactivateInstance(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerHealthManager player)) 
            player.TakeDamage(damagePerSecond * Time.fixedDeltaTime);
    }
}
