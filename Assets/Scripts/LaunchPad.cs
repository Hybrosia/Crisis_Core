using UnityEngine;

public class LaunchPad : MonoBehaviour
{
    [SerializeField] private float force = 12f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.TryGetComponent(out Movement playerMovement)) 
            playerMovement.SetVerticalSpeed(force);
    }
}
