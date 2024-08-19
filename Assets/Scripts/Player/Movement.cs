using System;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float movementSpeed = 0.5f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float runSpeed = 5f; 
    [SerializeField] [Range(0.0f, 0.5f)] private float moveSmoothTime = 0.3f;

    private Collision collisionLocal; 
    private bool _isGrounded;
    private float fallingSpeed;

    [SerializeField] private GameObject cameraReference; 

    private Rigidbody _rb;
    private Vector2 _currentDirection = Vector2.zero;
    private Vector2 _currentDirectionVelocity = Vector2.zero;
    
    void Start()
    {
        collisionLocal = GetComponent<Collision>();
        _rb = GetComponent<Rigidbody>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
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

    private float FindMovementSpeed()
    {
        if (InputManager.Sprint)
        {
            return runSpeed;
        }
        return movementSpeed;
    }

    
    
    
}
