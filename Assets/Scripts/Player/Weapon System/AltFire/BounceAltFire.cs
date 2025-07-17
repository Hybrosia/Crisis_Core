using System;
using UnityEngine;

public class BounceAltFire : MonoBehaviour
{
    private SphereCollider _collider;
    private Rigidbody _rigidbody; 
    
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        throw new NotImplementedException();
    }
}
