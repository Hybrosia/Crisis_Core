using UnityEngine;

[RequireComponent(typeof(InputManager))]
public class PlayerCameraInputSystem : MonoBehaviour
{
    [SerializeField] private float sensX; 
    [SerializeField] private float sensY;
    private float xRotation;
    private float yRotation; 

    [SerializeField] private Transform orientation;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; 
    }

    private void Update()
    {
        float mouseX = InputManager.Look.x * Time.deltaTime * sensX;
        float mouseY = InputManager.Look.y * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 
        
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
