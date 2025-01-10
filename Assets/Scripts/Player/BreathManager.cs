using UnityEngine;

public class BreathManager : MonoBehaviour
{
    public float Breath;
    [SerializeField] private float MaximumBreath;
    public float timeSinceLastBreathUse;
    [SerializeField] private float timeNeededForBreathRestoration;
    [SerializeField] private float BreathIncrease;

    private void Start()
    {
        Breath = MaximumBreath; 
    }

    private void Update()
    {
        if (Breath > MaximumBreath)
        {
            Breath = MaximumBreath;
        }
        
        RestoreBreath();
        
        print("I currently have" + Breath);
    }

    private bool BreathRestoration() => Breath != MaximumBreath && Time.time > (timeNeededForBreathRestoration + timeSinceLastBreathUse);

    private void RestoreBreath()
    {
        if (BreathRestoration())
        {
            Breath = Breath + BreathIncrease;
            timeSinceLastBreathUse = Time.time; 
        }
    }
}
