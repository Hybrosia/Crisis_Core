using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private float airGravity = 9.81f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float runSpeed = 8f; 
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;
    [SerializeField] private float sprintBreath = 50;
    [SerializeField] private float groundGravity;
    [SerializeField] private float floatGravity;
    [SerializeField] private bool canFloat; 

    private PlayerCollision _collisionLocal; 
    private bool _isGrounded;
    private float _verticalSpeed;

    [SerializeField] private GameObject cameraReference; 

    private Rigidbody _rb;
    private Vector2 _currentDirection = Vector2.zero;
    private Vector2 _currentDirectionVelocity = Vector2.zero;
    private bool _jumpPressed;

    private BreathManager _breathManager;

    private void Start()
    {
        _collisionLocal = GetComponent<PlayerCollision>();
        _rb = GetComponent<Rigidbody>();
        _breathManager = GetComponent<BreathManager>();
    }

    private void Update()
    {
        if (!_jumpPressed && InputManager.JumpPressed) _jumpPressed = true;
    }

    private void FixedUpdate()
    {
        _currentDirection = Vector2.SmoothDamp(_currentDirection, InputManager.Move,
            ref _currentDirectionVelocity, moveSmoothTime);
        
        var groundNormal = _collisionLocal.GetGroundNormal();
        _isGrounded = groundNormal != Vector3.zero;

        var speed = FindMovementSpeed();
        TryJump();

        _verticalSpeed -= FindGravity() * Time.fixedDeltaTime;
        if (_isGrounded && _verticalSpeed < 0f) _verticalSpeed = 0f;
        
        var position = transform.position;
        
        var forward = cameraReference.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        var movementVector = speed * Time.fixedDeltaTime * (forward * _currentDirection.y + cameraReference.transform.right * _currentDirection.x);
        movementVector.y = 0f;
        
        position += _collisionLocal.CollideAndSlide(movementVector, position, false);

        if (_isGrounded && _verticalSpeed < 0.1f) position += _collisionLocal.CollideAndSlide(Vector3.down * 0.05f, position, true);
        else position += _collisionLocal.CollideAndSlide(_verticalSpeed * Time.fixedDeltaTime * Vector3.up, position, true);

        _rb.MovePosition(position);

        _jumpPressed = false;
    }

    private bool Sprint() => _isGrounded && InputManager.Sprint && (_breathManager.Breath >= sprintBreath);

    private void TryJump()
    {
        if (!JumpAction) return;

        _verticalSpeed = jumpSpeed;
    }

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
    
    private bool JumpAction => _isGrounded && _jumpPressed;

    bool FloatAction() => _rb.linearVelocity.y <= 0f && canFloat && !_isGrounded && InputManager.JumpHeld;
    
    private float FindGravity()
    {
        if (_isGrounded && !JumpAction) return groundGravity;
        if (!_isGrounded && FloatAction()) return floatGravity;
        return airGravity;
    }
}