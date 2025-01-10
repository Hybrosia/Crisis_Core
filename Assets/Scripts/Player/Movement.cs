using System;
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

    private Collision collisionLocal; 
    private bool _isGrounded;
    private float fallingSpeed;

    [SerializeField] private GameObject cameraReference; 

    private Rigidbody _rb;
    private Vector2 _currentDirection = Vector2.zero;
    private Vector2 _currentDirectionVelocity = Vector2.zero;

    private BreathManager _breathManager; 
    
    void Start()
    {
        collisionLocal = GetComponent<Collision>();
        _rb = GetComponent<Rigidbody>();
        _breathManager = GetComponent<BreathManager>();
    }

    private void FixedUpdate()
    {
        _currentDirection = Vector2.SmoothDamp(_currentDirection, InputManager.Move,
            ref _currentDirectionVelocity, moveSmoothTime);
        
        var groundNormal = collisionLocal.GetGroundNormal();
        _isGrounded = groundNormal != Vector3.zero;

        var Speed = FindMovementSpeed();
        
        fallingSpeed += gravity * Time.fixedDeltaTime;
        if (_isGrounded) fallingSpeed = 0f;
        
        var position = transform.position;
        
        position += collisionLocal.CollideAndSlide(Speed * Time.fixedDeltaTime * (cameraReference.transform.forward * _currentDirection.y + cameraReference.transform.right * 
            _currentDirection.x), position, false);
        
        if (_isGrounded) position += collisionLocal.CollideAndSlide(Vector3.down, position, true);
        else position += collisionLocal.CollideAndSlide(fallingSpeed * Time.fixedDeltaTime * Vector3.down, position, true);
        
        _rb.MovePosition(position);
        //_rb.Move(position, transform.rotation);
    }

    //private bool jumpAction => _isGrounded && InputManager.

    private bool sprint() => _isGrounded && InputManager.Sprint && (_breathManager.Breath >= sprintBreath);

    private float FindMovementSpeed()
    {
        if (sprint())
        {
            _breathManager.Breath -= sprintBreath * Time.deltaTime;
            _breathManager.timeSinceLastBreathUse = Time.time;
            return runSpeed;
            
        }
        return movementSpeed;
    }

    
    
    
}
