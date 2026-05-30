using UnityEngine;
using UnityEngine.InputSystem;
using System;

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
        public bool LeftMouseButtonPressed => leftMouseButtonAction.WasPressedThisFrame();
        public bool LeftMouseButtonReleased => leftMouseButtonAction.WasReleasedThisFrame();
        public bool RightMouseButtonPressed => rightMouseButtonAction.WasPressedThisFrame();
        public bool BothMouseButtonsPressed => bothButtonsAction.WasPressedThisFrame();
        public bool CancelButtonPressed => cancelAction.WasPressedThisFrame();
        public bool TabTargetButtonPressed => tabTargetAction.WasPressedThisFrame();

        // --- CONTINUOUS HELD STATES ---
        public bool LeftMouseButtonHeld { get; private set; }
        public bool RightMouseButtonHeld { get; private set; }
        public bool BothMouseButtonsHeld { get; private set; }

        public event Action<int> OnHotbarPressed;

        private InputActionMap playerActionMap;
        private InputActionMap combatActionMap;

        private InputAction movementAction;
        private InputAction rotationAction;
        private InputAction jumpAction;
        private InputAction bothButtonsAction;
        private InputAction leftMouseButtonAction;
        private InputAction rightMouseButtonAction;
        private InputAction cancelAction;
        private InputAction tabTargetAction;

        private InputAction hotbarPressedAction;

        private void Awake()
        {
            playerActionMap = playerInputActionAsset.FindActionMap("Player");
            combatActionMap = playerInputActionAsset.FindActionMap("Combat");

            movementAction = playerActionMap.FindAction("Move");
            rotationAction = playerActionMap.FindAction("Look");
            jumpAction = playerActionMap.FindAction("Jump");
            bothButtonsAction = playerActionMap.FindAction("BothMouseButtons");
            leftMouseButtonAction = playerActionMap.FindAction("LeftClick");
            rightMouseButtonAction = playerActionMap.FindAction("RightClick");
            cancelAction = playerActionMap.FindAction("Cancel");
            tabTargetAction = playerActionMap.FindAction("TabTarget");

            hotbarPressedAction = combatActionMap.FindAction("HotbarPressed");

            SetupInputEvents();
        }

        private void SetupInputEvents()
        {
            movementAction.performed += inputInfo => MovementInput = inputInfo.ReadValue<Vector2>();
            movementAction.canceled += inputInfo => MovementInput = Vector2.zero;

            rotationAction.performed += inputInfo => RotationInput = inputInfo.ReadValue<Vector2>();
            rotationAction.canceled += inputInfo => RotationInput = Vector2.zero;

            leftMouseButtonAction.performed += _ => LeftMouseButtonHeld = true;
            leftMouseButtonAction.canceled += _ => LeftMouseButtonHeld = false;

            rightMouseButtonAction.performed += _ => RightMouseButtonHeld = true;
            rightMouseButtonAction.canceled += _ => RightMouseButtonHeld = false;

            bothButtonsAction.performed += _ => BothMouseButtonsHeld = true;
            bothButtonsAction.canceled += _ => BothMouseButtonsHeld = false;

            if (hotbarPressedAction != null)
            {
                hotbarPressedAction.performed += HandleHotbarInput;
            }
        }

        private void HandleHotbarInput(InputAction.CallbackContext context)
        {
            if (!context.ReadValueAsButton()) return;

            string keyName = context.control.name;
            int targetIndex = -1;

            switch (keyName)
            {
                case "1": targetIndex = 0; break;
                case "2": targetIndex = 1; break;
                case "3": targetIndex = 2; break;
                case "4": targetIndex = 3; break;
                case "5": targetIndex = 4; break;
                case "6": targetIndex = 4; break;
            }

            if (targetIndex != -1)
            {
                OnHotbarPressed?.Invoke(targetIndex);
            }
        }

        private void OnEnable() => playerActionMap.Enable();
        private void OnDisable() => playerActionMap.Disable();
    }
}