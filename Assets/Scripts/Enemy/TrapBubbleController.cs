using System;
using UnityEngine;

public class TrapBubbleController : MonoBehaviour
{
    [SerializeField] private float trapDuration;
    private Transform _originalParent;
    private GameObject _trappedEnemy;

    private float _timer;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _timer = Time.time + trapDuration;
    }

    private void Update()
    {
        if (Time.time < _timer) return;
        
        Pop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBullet")) Pop();
    }

    private void OnTriggerStay(Collider other)
    {
        var distanceVector = transform.position - other.transform.position;
        
        _rb.AddForce(Mathf.Clamp((2f - distanceVector.magnitude) / 2f, 0f, 1f) * 10f * distanceVector);
    }

    public void Trap(GameObject enemy)
    {
        _originalParent = enemy.transform.parent;
        enemy.transform.parent = transform;
        enemy.transform.localPosition = new Vector3(0f, 0f, -0.01f);
        enemy.transform.rotation = Quaternion.identity;
        enemy.GetComponent<IEnemyTrapManager>().Trap();

        _trappedEnemy = enemy;
    }

    private void Pop()
    {
        _trappedEnemy.transform.parent = _originalParent;
        _trappedEnemy.GetComponent<IEnemyTrapManager>().Untrap();
        
        ObjectPoolController.DeactivateInstance(gameObject);
    }
}
