using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.EventSystems;

namespace TinyRPG.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float waitTime = 0.5f;
        [SerializeField] private float catchUpTime = .1f;

        private CinemachineInputAxisController axisController;

        private CinemachineInputAxisController.Controller lookXController;
        private CinemachineInputAxisController.Controller lookYController;
        private CinemachineOrbitalFollow cinemachineOrbitalFollow;

        private PlayerInputHandler playerInputHandler;
        private Transform playerTransform;

        private Vector2 cursorPositionOnClick;
        private bool wasDraggingLastFrame;

        private void Awake()
        {
            playerTransform = GameObject.Find("Player").transform;

            playerInputHandler = playerTransform.GetComponent<PlayerInputHandler>();

            axisController = GetComponent<CinemachineInputAxisController>();
            cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();

            if (axisController != null && axisController.Controllers.Count >= 2)
            {
                lookXController = axisController.Controllers[0];
                lookYController = axisController.Controllers[1];
            }
        }

        private void Start()
        {
            cinemachineOrbitalFollow.HorizontalAxis.Recentering.Wait = waitTime;
        }

        private void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject() ||
                DragDropManager.Instance.IsDragging) return;

            bool isLooking = playerInputHandler.LeftMouseButtonHeld;

            lookXController.Enabled = isLooking;
            lookYController.Enabled = isLooking;

            if(!isLooking)
            {
                lookXController.InputValue = 0;
                lookYController.InputValue = 0;
            }

            if (isLooking)
            {
                if (!wasDraggingLastFrame)
                {
                    cursorPositionOnClick = Mouse.current.position.ReadValue();
                    wasDraggingLastFrame = true;
                }

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                SetRecenteringState(enable: false, forceSnap: false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                if (wasDraggingLastFrame)
                {
                    Mouse.current.WarpCursorPosition(cursorPositionOnClick);
                    wasDraggingLastFrame = false;
                }

                bool isMoving = playerInputHandler.MovementInput != Vector2.zero;

                if (isMoving)
                {
                    Vector3 flatCameraForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
                    float angleDelta = Vector3.Dot(flatCameraForward, playerTransform.forward);

                    if (angleDelta >= .99f)
                    {
                        SetRecenteringState(true, forceSnap: true);
                    }
                    else
                    {
                        SetRecenteringState(true, forceSnap: false);
                    }
                }
                else
                {
                    SetRecenteringState(false, forceSnap: false);
                }
            }
        }

        private void SetRecenteringState(bool enable, bool forceSnap)
        {
            InputAxis axis = cinemachineOrbitalFollow.HorizontalAxis;
            float targetTime = forceSnap ? 0f : catchUpTime;

            if (axis.Recentering.Enabled != enable || axis.Recentering.Time != targetTime)
            {
                axis.Recentering.Enabled = enable;
                axis.Recentering.Time = targetTime;

                cinemachineOrbitalFollow.HorizontalAxis = axis;
            }
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}