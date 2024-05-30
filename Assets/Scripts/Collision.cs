using System;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class Collision : MonoBehaviour
{
    [SerializeField] private int maxBounces = 5;
    [SerializeField] private float skinWidth = 0.015f;
    [SerializeField] private float maxSlopeAngle = 55;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float movementSpeed = 0.5f;
    [SerializeField] private LayerMask whatIsTerrain;

    private bool _isGrounded;
    private CapsuleCollider _collider;
    private float fallingSpeed;
    
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
    }

    //Adds gravity and actually moves the collider in two steps. The first is for movement, the second for gravity.
    private void FixedUpdate()
    {
        fallingSpeed += gravity * Time.fixedDeltaTime;
        if (_isGrounded) fallingSpeed = 0f;

        transform.position += CollideAndSlide(movementSpeed * Time.fixedDeltaTime * transform.forward, transform.position, false);
        transform.position += CollideAndSlide(fallingSpeed * Time.fixedDeltaTime * Vector3.down, transform.position, true);
    }

    //Uses the collide and slide algorithm to calculate the movement of the collider based on the given velocity and position,
    //with small differences if the call is for gravity. Returns the modified movement vector.
    public Vector3 CollideAndSlide(Vector3 velocity, Vector3 position, bool gravityPass)
    {
        return CollideAndSlideRecursive(velocity, position, 0, gravityPass, velocity);
    }
    
    //The implementation of the collide and slide algorithm
    public Vector3 CollideAndSlideRecursive(Vector3 velocity, Vector3 position, int depth, bool gravityPass,
        Vector3 velocityInitial)
    {
        if (depth > maxBounces) return Vector3.zero;

        var distance = velocity.magnitude + skinWidth;
        
        var bottomCenter = position + Vector3.down * (_collider.height * 0.5f - _collider.radius);
        var topCenter = position + Vector3.up * (_collider.height * 0.5f - _collider.radius);
        
        if (!Physics.CapsuleCast(bottomCenter, topCenter, _collider.radius - skinWidth, velocity.normalized, out var hit, distance, whatIsTerrain))
            return velocity;

        var snapToSurface = velocity.normalized * (hit.distance - skinWidth);
        var leftover = velocity - snapToSurface;
        var angle = Vector3.Angle(Vector3.up, hit.normal);

        if (snapToSurface.magnitude <= skinWidth) snapToSurface = Vector3.zero;

        if (angle <= maxSlopeAngle)
        {
            if (gravityPass) return snapToSurface;
            leftover = ProjectAndScale(leftover, hit.normal);
        }
        else
        {
            var scale = 1 - Vector3.Dot(new Vector3(hit.normal.x, 0f, hit.normal.z).normalized,
                new Vector3(-velocityInitial.x, 0f, -velocityInitial.z).normalized);

            if (_isGrounded && !gravityPass)
                leftover = ProjectAndScale(new Vector3(leftover.x, 0f, leftover.z), new Vector3(hit.normal.x, 0f, hit.normal.z)) * scale;
            else leftover = ProjectAndScale(leftover, hit.normal) * scale;
        }

        return snapToSurface + CollideAndSlideRecursive(leftover, position, depth + 1, gravityPass, velocityInitial);
    }

    //Projects the vector onto the plane defined by the normal, but keeps the magnitude constant.
    private Vector3 ProjectAndScale(Vector3 vector, Vector3 normal)
    {
        return Vector3.ProjectOnPlane(vector, normal).normalized * vector.magnitude;
    }
}
