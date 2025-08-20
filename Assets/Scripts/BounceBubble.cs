using UnityEngine;

public class BounceBubble : MonoBehaviour
{
    [SerializeField] private float force = 12f;
    [SerializeField] private Animator animator;

    private void OnEnable()
    {
        animator.Play("Idle");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !other.TryGetComponent(out Movement playerMovement)) return;
        
        playerMovement.SetVerticalSpeed(force);
        animator.Play("Pop");
    }

    public void DisableBubble()
    {
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
