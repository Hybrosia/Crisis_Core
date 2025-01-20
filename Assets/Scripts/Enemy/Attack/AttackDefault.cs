using UnityEngine;

public abstract class AttackDefault : MonoBehaviour
{
    protected abstract void StartAttack();
    private void OnEnable() => StartAttack();
}
