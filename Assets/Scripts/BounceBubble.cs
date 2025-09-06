using UnityEngine;

public class BounceBubble : MonoBehaviour
{
    [SerializeField] private float force = 12f, timeToEnable = 0.2f, timeToStop = 0.5f;
    [SerializeField] private Animator animator;

    [HideInInspector] public Vector3 movement;

    private float spawnTimer;

    private void OnEnable()
    {
        spawnTimer = Time.time;
        animator.Play("Idle");
    }

    private void OnTriggerEnter(Collider other)
    {
        TryActivateBounce(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryActivateBounce(other);
    }

    private void TryActivateBounce(Collider other)
    {
        if (Time.time < spawnTimer + timeToEnable) return;
        if (!other.CompareTag("BounceBox") || !other.transform.parent.TryGetComponent(out Movement playerMovement)) return;
        
        playerMovement.SetVerticalSpeed(force);
        animator.Play("Pop");
    }

    private void FixedUpdate()
    {
        transform.position +=
            Vector3.Lerp(movement, Vector3.zero, Mathf.Pow((Time.time - spawnTimer) / timeToStop, 2)) *
            Time.fixedDeltaTime;
    }

    public void DisableBubble()
    {
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
