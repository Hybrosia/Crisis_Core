using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float maxHealth;

    private float _currentHealth;

    private void Start()
    {
        ResetHealth();
    }

    public float GetCurrentHealth() => _currentHealth;

    public void HealDamage(float amount)
    {
        _currentHealth += amount;
        if (_currentHealth > maxHealth) _currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(float amount)
    {
        print("DAMAGE: " + amount);
        _currentHealth -= amount;
        UpdateHealthUI();
        
        if (_currentHealth > 0f) return;
        
        GameOver();
    }

    private void GameOver()
    {
        //SaveScript.LoadFromDisk();
    }

    public void ResetHealth()
    {
        _currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        //TODO: Implement.
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyProjectile")) return;
        if (other.TryGetComponent(out EnemyProjectile bullet))
            TakeDamage(bullet.damage);
    }
}
