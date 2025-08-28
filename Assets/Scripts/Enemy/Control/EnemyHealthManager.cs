using UnityEngine;

public class EnemyHealthManager : MonoBehaviour, IEnemyHealthManager
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
    public void TakeDamage(float amount)
    {
        CurrentHealth -= amount;
        
        if (CurrentHealth > 0f) return;
        
        if (TryGetComponent<EnemyDeathBase>(out var deathScript)) deathScript.OnDeath();
        else ObjectPoolController.DeactivateInstance(gameObject);
    }
}

public interface IEnemyHealthManager
{
    public void TakeDamage(float amount);

    public void Trap()
    {
        
    }
}
