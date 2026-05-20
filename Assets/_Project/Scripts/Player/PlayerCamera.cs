using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    private CinemachineInputAxisController axisController;

    private CinemachineInputAxisController.Controller lookXController;
    private CinemachineInputAxisController.Controller lookYController;

    private void Awake()
    {
        axisController = GetComponent<CinemachineInputAxisController>();

        if (axisController != null && axisController.Controllers.Count >= 2)
        {
            lookXController = axisController.Controllers[0];
            lookYController = axisController.Controllers[1];
        }
    }

    private void Update()
    {
        if (axisController == null) return;

        bool isDragging = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (lookXController != null) lookXController.Enabled = isDragging;
        if (lookYController != null) lookYController.Enabled = isDragging;

        if (isDragging)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}