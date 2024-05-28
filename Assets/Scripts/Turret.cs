using UnityEngine;

public class Turret : MonoBehaviour
{
    
   
    [SerializeField] private PlayerData playerData;
    [Space(5)]
    [SerializeField] private float range = 10f;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private float reloadTime = 1f;
   

 
    private float _reloadTimeCounter;
    private float _distance;
    private Vector3 _heading;
    private Vector3 _direction;
    private Vector3 _currentRotation;
    private LayerMask _toHitLayerMask;
    private Transform _turretHead;
    
    // public float frameRateInterval = 30;


    private void Awake()
    {
       _toHitLayerMask = LayerMask.GetMask("Player");
       _turretHead = transform.GetChild(0);
    }

    private void Update()
    {
        UpdateDirectionDistance();
        
       
        if (!TargetIsInRange() )
            return; LookTowards();
        if (!TargetIsInView())
            return; AttackPlayer();
        
    }

    private bool TargetIsInRange()
    {
        return Vector3.Distance(playerData.PlayerPos, _turretHead.position) < range;
    }

    private bool TargetIsInView()
    {
        return Physics.Raycast(_turretHead.position, _direction, range, _toHitLayerMask);
    }

    private void AttackPlayer()
    {
        if (!(Time.time > _reloadTimeCounter)) return;
        
        print(_turretHead.parent.name + "Is Shooting at: " + playerData.player);
        
        _reloadTimeCounter = reloadTime + Time.time;

    }
    
    private void UpdateDirectionDistance()
    {
        _heading = playerData.PlayerPos - _turretHead.position;
        _distance = _heading.magnitude;
        _direction = _heading / _distance; // This is now the normalized direction.
    }

    private void LookTowards()
    {
        
        Quaternion toRotation = Quaternion.LookRotation(_direction, _turretHead.up);
        _turretHead.rotation = Quaternion.Lerp(_turretHead.rotation, toRotation, rotateSpeed * Time.deltaTime);
        
       /* if (Time.frameCount % frameRateInterval == 0)
        {
            //_turretHead.rotation = Quaternion.LookRotation(_direction);
            _turretHead.LookAt(playerData.PlayerPos);
        }*/
    }
}