using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;

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

    private Coroutine recenteringCoroutine;
    private bool isRecentered;

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
        if (axisController == null || cinemachineOrbitalFollow == null) return;

        bool isLooking = playerInputHandler.LeftMouseButtonPressed;

        lookXController.Enabled = isLooking;
        lookYController.Enabled = isLooking;

        if (isLooking)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SetRecenteringState(enable: false, forceSnap: false);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            bool isMoving = playerInputHandler.MovementInput != Vector2.zero;

            if (isMoving)
            {
                Vector3 flatCameraForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
                float angleDelta = Vector3.Dot(flatCameraForward, playerTransform.forward);

                if (angleDelta >= .99f)
                {
                    SetRecenteringState(enable: true, forceSnap: true);
                }
                else
                {
                    SetRecenteringState(enable: true, forceSnap: false);
                }
            }
            else
            {
                SetRecenteringState(enable: false, forceSnap: false);
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