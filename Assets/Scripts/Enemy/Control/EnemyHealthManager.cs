using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    
    protected float CurrentHealth;

    protected virtual void OnEnable()
    {
        ResetHealth();
    }

    protected void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }

    //Takes damage.
    public virtual void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        
        if (CurrentHealth > 0f) return;
        
        if (TryGetComponent<EnemyDeathBase>(out var deathScript)) deathScript.OnDeath();
        else Destroy(gameObject);
    }
    
    /*public virtual void TakeDamage(WeaponStats weaponStats, bool isSecondary)
    {
        CurrentHealth -= isSecondary ? weaponStats.secondaryDamage : weaponStats.weaponDamage;
        
        if (CurrentHealth <= 0f)
        {
            if (corpsePrefab) Instantiate(corpsePrefab, transform.position, transform.rotation, transform.parent);
            Destroy(gameObject);
            return;
        }
    }*/
}
