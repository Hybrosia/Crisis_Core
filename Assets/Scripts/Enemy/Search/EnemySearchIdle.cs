using System;
using UnityEngine;

public class EnemySearchIdle : EnemySearchBase
{
    private void OnEnable()
    {
        GetComponent<EnemyIdleBase>().enabled = true;
        enabled = false;
    }
}
