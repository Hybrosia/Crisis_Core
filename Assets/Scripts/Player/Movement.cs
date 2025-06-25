using UnityEngine;
using UnityEngine.Serialization;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private float airGravity = 9.81f;
    [SerializeField] private float movementInputForce = 5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float runInputForce = 8f;
    [SerializeField] [Range(0.0f, 1f)] private float inputImportance = 0.8f;
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;
    [SerializeField] private float sprintBreath = 50;
    [SerializeField] private float groundGravity;
    [SerializeField] private float floatGravity;
    [SerializeField] private float floatTimer = 2f;
    [SerializeField] private float floatCooldown = 15f;
    [SerializeField] private float maxHorizontalSpeed = 10, maxFallingSpeed = 50, maxVerticalSpeed = 1000;

    private float _timeSinceFloat;
    private float _currentFloat = 0;
    private bool _floatInit = false;

    private PlayerCollision _collisionLocal;
    private bool _isGrounded;
    private Vector2 _horizontalMomentum;
    private float _verticalMomentum;

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
        _timeSinceFloat = floatCooldown;
    }

    private void Update()
    {
        if (!_jumpPressed && InputManager.JumpPressed) _jumpPressed = true;
        _timeSinceFloat += Time.deltaTime;
    }

    private void FixedUpdate()
    {   
        _currentDirection = Vector2.SmoothDamp(_currentDirection, InputManager.Move,
            ref _currentDirectionVelocity, moveSmoothTime);
        
        var groundNormal = _collisionLocal.GetGroundNormal();
        _isGrounded = groundNormal != Vector3.zero;

        var speed = FindMovementSpeed();
        TryJump();

        _verticalMomentum -= FindGravity() * Time.fixedDeltaTime;
        if (_isGrounded && _verticalMomentum < 0f) _verticalMomentum = 0f;
        
        var position = transform.position;
        
        var forward = cameraReference.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        var inputVector = speed * Time.fixedDeltaTime * (forward * _currentDirection.y + cameraReference.transform.right * _currentDirection.x);
        inputVector.y = 0f;

        _horizontalMomentum = Vector2.Lerp(_horizontalMomentum, new Vector2(inputVector.x, inputVector.z), inputImportance);
        Vector3.ClampMagnitude(_horizontalMomentum, maxHorizontalSpeed);

        _verticalMomentum = Mathf.Clamp(_verticalMomentum, -maxFallingSpeed, maxVerticalSpeed);

        position += _collisionLocal.CollideAndSlide(new Vector3(_horizontalMomentum.x, 0f, _horizontalMomentum.y), position, false);

        if (_isGrounded && _verticalMomentum < 0.1f) position += _collisionLocal.CollideAndSlide(Vector3.down * 0.05f, position, true);
        else position += _collisionLocal.CollideAndSlide(_verticalMomentum * Time.fixedDeltaTime * Vector3.up, position, true);

        _rb.MovePosition(position);

        _jumpPressed = false;

        ResetFloatTimer();
    }

    private bool Sprint() => _isGrounded && InputManager.Sprint && (_breathManager.Breath >= sprintBreath);

    private void TryJump()
    {
        if (!JumpAction) return;

        _verticalMomentum = jumpSpeed;
    }

    private float FindMovementSpeed()
    {
        if (Sprint())
        {
            _breathManager.Breath -= sprintBreath * Time.deltaTime;
            _breathManager.timeSinceLastBreathUse = Time.time;
            return runInputForce;
        }
        return movementInputForce;
    }
    
    private bool JumpAction => _isGrounded && _jumpPressed;
    
    private bool CanFloat => _timeSinceFloat >= floatCooldown; 
    bool FloatAction() => _rb.linearVelocity.y <= 0f && CanFloat && !_isGrounded && InputManager.JumpHeld;

    private void ResetFloatTimer()
    {
        if (FloatAction())
        {
            _floatInit = true;
            _currentFloat += Time.deltaTime;
            if (_currentFloat >= floatTimer)
            {
                _timeSinceFloat = 0;
                _floatInit = false; 
            }
        }
        else if (_floatInit && !FloatAction())
        {
            _floatInit = false;
            _timeSinceFloat = 0;
            _currentFloat = 0; 
        }
        else
        {
            _currentFloat = 0;
            _floatInit = false; 
        }
    }
    
    private float FindGravity()
    {
        if (_isGrounded && !JumpAction) return groundGravity;
        if (!_isGrounded && FloatAction()) return floatGravity;
        return airGravity;
    }

    public void AddForce(Vector3 force)
    {
        _verticalMomentum += force.y;
        _horizontalMomentum += new Vector2(force.x, force.z);
    }
}