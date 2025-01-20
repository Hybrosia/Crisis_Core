using UnityEngine;

public class EnemyIdleStationary : EnemyIdleBase
{
    [SerializeField] private PlayerData playerData;
    
    //Activates the enemy.
    private void Update()
    {
        if (!playerData.CanSeePlayerFromPoint(transform.position)) return;

        GetComponent<EnemyController>().enabled = true;
        enabled = false;
    }
}
