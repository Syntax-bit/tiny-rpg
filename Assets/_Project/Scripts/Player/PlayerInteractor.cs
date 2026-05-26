using TinyRPG.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyRPG.Player
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactionRange = 2f;

        private bool isInteracting;
        private float currentInteractTime;
        private IInteractable activeInteractable;

        private Camera mainCamera;
        private PlayerInputHandler playerInputHandler;

        private void Awake()
        {
            playerInputHandler = GetComponent<PlayerInputHandler>();
            mainCamera = Camera.main;
        }


        private void Update()
        {
            //Handle interaction
            IInteractable interactable = GetInteractable();
            if (interactable != null && !isInteracting && playerInputHandler.RightMouseButtonPressed)
            {
                StartInteraction(interactable);
            }

            if (isInteracting)
            {
                HandleActiveInteraction();
            }
        }

        private void StartInteraction(IInteractable interactable)
        {
            activeInteractable = interactable;
            isInteracting = true;
            currentInteractTime = 0;

            if (interactable.GetInteractionTime() <= 0)
            {
                CompleteInteraction();
                return;
            }

            PlayerUIManager.Instance.ShowInteractionBar(activeInteractable.GetInteractionPrompt());
        }

        private void HandleActiveInteraction()
        {
            if (playerInputHandler.MovementInput != Vector2.zero || playerInputHandler.JumpTriggered)
            {
                CancelInteraction();
                return;
            }

            currentInteractTime += Time.deltaTime;
            float normalizedInteractionTime = currentInteractTime / activeInteractable.GetInteractionTime();

            PlayerUIManager.Instance.SetInteractionBarProgress(normalizedInteractionTime);

            if (normalizedInteractionTime >= 1)
            {
                CompleteInteraction();
            }
        }

        private void CompleteInteraction()
        {
            activeInteractable.Interact();

            CleanInteraction(true);
        }

        private void CancelInteraction()
        {
            PlayerUIManager.Instance.CancelInteraction();

            isInteracting = false;
        }

        private void CleanInteraction(bool hideInstantly)
        {
            currentInteractTime = 0;
            activeInteractable = null;
            isInteracting = false;

            if (hideInstantly)
            {
                PlayerUIManager.Instance.HideInteractionBar();
            }
        }

        private IInteractable GetInteractable()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Ray interactionRay = mainCamera.ScreenPointToRay(mouseScreenPos);

            if (Physics.Raycast(interactionRay, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    float distance = Vector3.Distance(transform.position, hit.collider.gameObject.transform.position);

                    if (distance <= interactionRange)
                    {
                        return interactable;
                    }
                }
            }

            return null;
        }
    }
}