using UnityEngine;

public class EnemyHurtBox : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1f;
    
    [HideInInspector] public IEnemyHealthManager HealthManager;

    private void Start()
    {
        HealthManager = GetComponentInParent<IEnemyHealthManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerBullet")) return;
        if (other.TryGetComponent(out BulletScript bullet))
            HealthManager?.TakeDamage(bullet.weaponStats.weaponDamage * damageMultiplier);
    }
}
