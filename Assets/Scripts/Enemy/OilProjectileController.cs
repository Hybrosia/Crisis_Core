using UnityEngine;

public class OilProjectileController : MonoBehaviour
{
    public float damage;
    [SerializeField] private float initialSpeed, fireAngle;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject oilFieldPrefab;

    public void Fire()
    {
        var directionTowardsPlayer = (playerData.PlayerPos - transform.position).normalized;
        var rotationAxis = Vector3.Cross(directionTowardsPlayer, Vector3.up);
        var rotation = Quaternion.identity;
        if (rotationAxis != Vector3.zero) Quaternion.AngleAxis(fireAngle, rotationAxis);

        rb.linearVelocity = rotation * directionTowardsPlayer * initialSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        var oilField = ObjectPoolController.SpawnFromPrefab(oilFieldPrefab);
        oilField.transform.position = transform.position;

        ObjectPoolController.DeactivateInstance(gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
