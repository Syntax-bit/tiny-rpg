using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset playerInputActionAsset;

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }
    public bool JumpTriggered { get; private set; }

    private InputActionMap playerActionMap;

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction jumpAction;


    private void Awake()
    {
        playerActionMap = playerInputActionAsset.FindActionMap("Player");

        movementAction = playerActionMap.FindAction("Move");
        rotationAction = playerActionMap.FindAction("Look");
        jumpAction = playerActionMap.FindAction("Jump");

        SetupInputEvents();
    }

    private void SetupInputEvents()
    {
        movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
        movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

        rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
        rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

        jumpAction.performed += inputInfo => JumpTriggered = true;
        jumpAction.canceled += inputInfo => JumpTriggered = false;
    }

    private void OnEnable()
    {
        playerActionMap.Enable();
    }

    private void OnDisable()
    {
        playerActionMap.Disable();
    }
}
