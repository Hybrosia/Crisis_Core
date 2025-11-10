using UnityEngine;
using UnityEngine.UI;

public class BreathManager : MonoBehaviour
{
    public float Breath;
    [SerializeField] private float MaximumBreath;
    [SerializeField] private float timeNeededForBreathRestoration;
    [SerializeField] private float BreathIncrease;
    [SerializeField] private Image breathUI;

    private float _timeSinceLastBreathUse;

    private void Start()
    {
        Breath = MaximumBreath;
        UpdateBreathUI();
    }

    private void Update()
    {
        if (Breath > MaximumBreath)
        {
            Breath = MaximumBreath;
            UpdateBreathUI();
        }
        
        RestoreBreath();
        //print("my current breath is " + Breath);
    }

    private bool BreathRestoration() => Breath != MaximumBreath && Time.time > (timeNeededForBreathRestoration + _timeSinceLastBreathUse);

    private void RestoreBreath()
    {
        if (BreathRestoration())
        {
            Breath = Breath + BreathIncrease;
            _timeSinceLastBreathUse = Time.time;
            UpdateBreathUI();
        }
    }

    public void UseBreath(float amount)
    {
        Breath -= amount;
        _timeSinceLastBreathUse = Time.time;
        UpdateBreathUI();
    }

    public void ResetBreath()
    {
        Breath = MaximumBreath;
        UpdateBreathUI();
    }

    private void UpdateBreathUI()
    {
        breathUI.fillAmount = Breath / MaximumBreath;
    }
}
