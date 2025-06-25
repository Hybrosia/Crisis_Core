using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputSystem_Actions _inputSystem;

    public static Vector2 Move { get; private set; }
    public static Vector2 Look { get; private set; }
    public static bool Sprint { get; private set; }
    public static bool JumpPressed { get; private set; }
    public static bool JumpReleased { get; private set; }
    
    public static bool AltFirePressed { get; private set; }
    
    public static bool AltFireReleased { get; private set; }
    public static bool JumpHeld { get; private set; } // This is used for Gliding
                                                      // (When Jump is held while in air you start gliding)

    public static bool PreviousPressed { get; private set; }
    public static bool NextPressed { get; private set; }
    public static bool FirePressed { get; private set; }


    private void Update()
    {
        Move = _inputSystem.Player.Move.ReadValue<Vector2>();
        Look = _inputSystem.Player.Look.ReadValue<Vector2>();
        Sprint = _inputSystem.Player.Sprint.IsPressed();
        JumpPressed = _inputSystem.Player.Jump.WasPressedThisFrame();
        JumpReleased = _inputSystem.Player.Jump.WasReleasedThisFrame();
        JumpHeld = _inputSystem.Player.Jump.IsPressed();
        PreviousPressed = _inputSystem.Player.Previous.WasPressedThisFrame();
        NextPressed = _inputSystem.Player.Next.WasPressedThisFrame();
        FirePressed = _inputSystem.Player.Fire.WasPressedThisFrame();
        AltFirePressed = _inputSystem.Player.AltFire.WasPressedThisFrame();
        AltFireReleased = _inputSystem.Player.AltFire.WasReleasedThisFrame();
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
