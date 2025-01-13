using System;
using UnityEngine;

public class CameraHolder : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    private void Update()
    {
        transform.position = cameraPosition.position; 
    }
}
