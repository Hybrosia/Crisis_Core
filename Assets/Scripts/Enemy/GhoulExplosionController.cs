using System;
using System.Collections;
using UnityEngine;

public class GhoulExplosionController : MonoBehaviour
{
    [SerializeField] private float timeBeforeExplosion;
    [SerializeField] private GameObject gasCloudPrefab;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(ExplodeCoroutine());
    }

    private IEnumerator ExplodeCoroutine()
    {
        yield return new WaitForSeconds(timeBeforeExplosion);

        var instance = ObjectPoolController.SpawnFromPrefab(gasCloudPrefab);
        instance.transform.parent = transform.parent;
        instance.transform.position = transform.position;
        instance.transform.rotation = transform.rotation;

        ObjectPoolController.DeactivateInstance(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("PlayerBullet")) return;

        StopAllCoroutines();
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
