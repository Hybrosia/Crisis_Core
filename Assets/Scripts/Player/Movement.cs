using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float runSpeed = 8f; 
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;
    [SerializeField] private float sprintBreath = 50;
    [SerializeField] private float jumpGravity;
    [SerializeField] private float floatGravity;
    [SerializeField] private bool canFloat; 

    private Collision _collisionLocal; 
    private bool _isGrounded;
    private float _fallingSpeed;

    [SerializeField] private GameObject cameraReference; 

    private Rigidbody _rb;
    private Vector2 _currentDirection = Vector2.zero;
    private Vector2 _currentDirectionVelocity = Vector2.zero;

    private BreathManager _breathManager;

    private void Start()
    {
        _collisionLocal = GetComponent<Collision>();
        _rb = GetComponent<Rigidbody>();
        _breathManager = GetComponent<BreathManager>();
    }

    private void FixedUpdate()
    {
        _currentDirection = Vector2.SmoothDamp(_currentDirection, InputManager.Move,
            ref _currentDirectionVelocity, moveSmoothTime);
        
        var groundNormal = _collisionLocal.GetGroundNormal();
        _isGrounded = groundNormal != Vector3.zero;

        var speed = FindMovementSpeed();
        
        _fallingSpeed += gravity * Time.fixedDeltaTime;
        if (_isGrounded) _fallingSpeed = 0f;
        
        var position = transform.position;
        
        position += _collisionLocal.CollideAndSlide(speed * Time.fixedDeltaTime * (cameraReference.transform.forward * _currentDirection.y + cameraReference.transform.right * 
            _currentDirection.x), position, false);
        
        if (_isGrounded) position += _collisionLocal.CollideAndSlide(Vector3.down, position, true);
        else position += _collisionLocal.CollideAndSlide(_fallingSpeed * Time.fixedDeltaTime * Vector3.down, position, true);
        
        _rb.MovePosition(position);
        //_rb.Move(position, transform.rotation);
        
    }

    private bool Sprint() => _isGrounded && InputManager.Sprint && (_breathManager.Breath >= sprintBreath);

    private float FindMovementSpeed()
    {
        if (Sprint())
        {
            _breathManager.Breath -= sprintBreath * Time.deltaTime;
            _breathManager.timeSinceLastBreathUse = Time.time;
            return runSpeed;
            
        }
        return movementSpeed;
    }
    
    private bool jumpAction => _isGrounded && InputManager.JumpPressed;

    bool floatAction() => canFloat && !_isGrounded && InputManager.JumpHeld; 
    
    private float FindGravity()
    {
        if (floatAction())
        {
            return floatGravity;
        }
        else if (!floatAction() && !_isGrounded)
        {
            return jumpGravity;
        }

        return gravity; 
    }
    
}