using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputSystem;

    public static Vector2 Move { get; private set; }
    public static Vector2 Look { get; private set; }
    

    private void Update()
    {
        Move = _inputSystem.Player.Move.ReadValue<Vector2>();
        Look = _inputSystem.Player.Look.ReadValue<Vector2>();
    }

    private void Awake()
    {
        _inputSystem = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        _inputSystem.Enable();
    }

    private void OnDisable()
    {
        _inputSystem.Disable();
    }
}
