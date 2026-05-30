using UnityEngine;

namespace TinyRPG.Player
{

    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        public bool IsMoving => movementDirection.sqrMagnitude > 0.01f;

        [Header("Settings")]
        [SerializeField] private float movementSpeed = 7f;
        [Range(0.1f, 1f)]
        [SerializeField] private float backwardsSpeedMultiplier = .75f;
        [SerializeField] private float turnSpeed = 240f;
        [SerializeField] private float jumpHeight = 4f;
        [SerializeField] private float gravity = -24f;
        [SerializeField] private float terminalVelocity = -18f;

        private Vector3 movementDirection;
        private float verticalVelocity;

        private CharacterController characterController;
        private PlayerInputHandler playerInputHandler;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            characterController = GetComponent<CharacterController>();
            playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Update()
        {
            HandleRotation();
            movementDirection = CalculateMoveDirection();

            float currentSpeed = movementSpeed;
            if (playerInputHandler.MovementInput.y < 0 && !playerInputHandler.BothMouseButtonsPressed)
            {
                currentSpeed *= backwardsSpeedMultiplier;
            }

            Vector3 finalVelocity = movementDirection * currentSpeed;

            ApplyVerticalVelocity();
            finalVelocity.y = verticalVelocity;

            characterController.Move(finalVelocity * Time.deltaTime);
        }

        private Vector3 CalculateMoveDirection()
        {
            if (!characterController.isGrounded) return movementDirection;

            Vector3 horizontalDirection = Vector3.zero;

            if (playerInputHandler.BothMouseButtonsHeld)
            {
                float strafeInput = playerInputHandler.MovementInput.x;
                if (Mathf.Abs(strafeInput) > 0.01f)
                {
                    strafeInput = Mathf.Sign(strafeInput);
                }

                float forwardMovement = (playerInputHandler.MovementInput.y < 0f) ? 0f : 1f;

                Vector3 localInput = new Vector3(strafeInput, 0f, forwardMovement);

                horizontalDirection = transform.TransformDirection(localInput);
            }
            else
            {
                float forwardInput = playerInputHandler.MovementInput.y;
                if (Mathf.Abs(forwardInput) > 0.01f)
                {
                    forwardInput = Mathf.Sign(forwardInput);
                }
                horizontalDirection = transform.forward * forwardInput;
            }

            if (horizontalDirection.sqrMagnitude > 1f)
            {
                horizontalDirection.Normalize();
            }

            return horizontalDirection;
        }

        private void HandleRotation()
        {
            if (playerInputHandler.BothMouseButtonsHeld)
            {
                Vector3 cameraDirection = new Vector3(mainCamera.transform.forward.x, 0f, mainCamera.transform.forward.z);
                if (cameraDirection.sqrMagnitude > 0.001f)
                {
                    cameraDirection.Normalize();
                    transform.rotation = Quaternion.LookRotation(cameraDirection);
                }
            }
            else
            {
                float turnAmount = playerInputHandler.MovementInput.x * turnSpeed * Time.deltaTime;
                transform.Rotate(0f, turnAmount, 0f);
            }
        }

        private void ApplyVerticalVelocity()
        {
            if (characterController.isGrounded)
            {
                if (verticalVelocity < 0f)
                {
                    verticalVelocity = -2f;
                }

                if (playerInputHandler.JumpTriggered)
                {
                    verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;

                if (verticalVelocity < terminalVelocity)
                {
                    verticalVelocity = terminalVelocity;
                }
            }
        }
    }
}