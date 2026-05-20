using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float jumpForce = 4f;
    [SerializeField] private float gravityMultiplier = 1f;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 1f;

    private Vector3 currentMovement;

    private CharacterController characterController;
    private PlayerInputHandler playerInputHandler;

}
