using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private TextMeshProUGUI healthUI;

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
        healthUI.text = Mathf.RoundToInt(_currentHealth).ToString();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("EnemyProjectile")) return;
        if (other.TryGetComponent(out EnemyProjectile bullet))
            TakeDamage(bullet.damage);
    }
}
