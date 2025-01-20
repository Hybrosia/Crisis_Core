using UnityEngine;

public class EnemyDeathSpawnObject : EnemyDeathBase
{
    [SerializeField] private GameObject deathSequenceObject;
    
    public override void OnDeath()
    {
        //TODO: Return to object pool. For now:
        Destroy(gameObject);
        Instantiate(deathSequenceObject, transform.position, transform.rotation, transform.parent);
    }
}
