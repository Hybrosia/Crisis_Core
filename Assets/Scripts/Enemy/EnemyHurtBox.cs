using UnityEngine;

public class EnemyHurtBox : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1f;
    
    private IEnemyHealthManager _healthManager;

    private void Start()
    {
        _healthManager = GetComponentInParent<IEnemyHealthManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerBullet")) return;
        if (other.TryGetComponent(out BulletScript bullet))
            _healthManager?.TakeDamage(bullet.weaponStats.weaponDamage * damageMultiplier);
    }
}
