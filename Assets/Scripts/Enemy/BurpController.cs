using System;
using UnityEngine;

public class BurpController : MonoBehaviour
{
    [SerializeField] private float force;
    
    private void OnTriggerEnter(Collider other) => ApplyBurpEffects(other);
    private void OnTriggerStay(Collider other) => ApplyBurpEffects(other);

    private void ApplyBurpEffects(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Movement playerMovement)) 
            playerMovement.AddMomentum(force);
    }
}
