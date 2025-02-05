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

        ObjectPoolController.SpawnFromPrefab(gasCloudPrefab);
        ObjectPoolController.DeactivateInstance(gameObject);
    }

    private void OnCollisionEnter(UnityEngine.Collision other)
    {
        //TODO: Despawn when hit with secondary fire or spells.
    }
}
