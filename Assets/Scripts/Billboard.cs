using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform _mainCameraTransform;
    
    private void Awake()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(_mainCameraTransform, _mainCameraTransform.up);
        transform.Rotate(0f, 180f, 0f);
    }
}
