using UnityEngine;

public class EnemyDeathSpawnObject : EnemyDeathBase
{
    [SerializeField] private GameObject deathSequenceObject;
    
    public override void OnDeath()
    {
        var instance = ObjectPoolController.SpawnFromPrefab(deathSequenceObject);
        instance.transform.parent = transform.parent;
        instance.transform.position = transform.position;
        instance.transform.rotation = transform.rotation;

        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
