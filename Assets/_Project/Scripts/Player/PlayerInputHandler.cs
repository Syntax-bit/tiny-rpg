using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyRPG.Player
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private InputActionAsset playerInputActionAsset;

        // --- CONTINUOUS AXIS TRACKING ---
        public Vector2 MovementInput { get; private set; }
        public Vector2 RotationInput { get; private set; }

        // --- DISCRETE PULSES ---
        public bool JumpTriggered => jumpAction.WasPressedThisFrame();
        // Mouse Inputs
        public bool LeftMouseButtonPressed => leftMouseButtonAction.WasPressedThisFrame();
        public bool LeftMouseButtonReleased => leftMouseButtonAction.WasReleasedThisFrame();
        public bool RightMouseButtonPressed => rightMouseButtonAction.WasPressedThisFrame();
        public bool BothMouseButtonsPressed => bothButtonsAction.WasPressedThisFrame();

        public bool CancelButtonPressed => cancelAction.WasPressedThisFrame();

        // --- CONTINUOUS HELD STATES ---
        public bool LeftMouseButtonHeld { get; private set; }
        public bool RightMouseButtonHeld { get; private set; }
        public bool BothMouseButtonsHeld { get; private set; }

        private InputActionMap playerActionMap;
        private InputAction movementAction;
        private InputAction rotationAction;
        private InputAction jumpAction;
        private InputAction bothButtonsAction;
        private InputAction leftMouseButtonAction;
        private InputAction rightMouseButtonAction;
        private InputAction cancelAction;

        private void Awake()
        {
            playerActionMap = playerInputActionAsset.FindActionMap("Player");

            movementAction = playerActionMap.FindAction("Move");
            rotationAction = playerActionMap.FindAction("Look");
            jumpAction = playerActionMap.FindAction("Jump");
            bothButtonsAction = playerActionMap.FindAction("BothMouseButtons");
            leftMouseButtonAction = playerActionMap.FindAction("LeftClick");
            rightMouseButtonAction = playerActionMap.FindAction("RightClick");
            cancelAction = playerActionMap.FindAction("Cancel");

            SetupInputEvents();
        }

        private void SetupInputEvents()
        {
            // Continuous reading vectors
            movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
            movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

            rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
            rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

            // Continuous held states
            leftMouseButtonAction.performed += _ => LeftMouseButtonHeld = true;
            leftMouseButtonAction.canceled += _ => LeftMouseButtonHeld = false;

            rightMouseButtonAction.performed += _ => RightMouseButtonHeld = true;
            rightMouseButtonAction.canceled += _ => RightMouseButtonHeld = false;

            bothButtonsAction.performed += _ => BothMouseButtonsHeld = true;
            bothButtonsAction.canceled += _ => BothMouseButtonsHeld = false;
        }

        private void OnEnable() => playerActionMap.Enable();
        private void OnDisable() => playerActionMap.Disable();
    }
}