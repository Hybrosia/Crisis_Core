using UnityEngine;

public class KillAfterTime : MonoBehaviour
{
    [SerializeField] private float duration;
    
    private float _timer;

    private void OnEnable()
    {
        _timer = Time.time + duration;
    }

    private void Update()
    {
        if (Time.time < _timer) return;
        
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
