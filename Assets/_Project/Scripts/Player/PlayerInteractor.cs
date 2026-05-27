using ImprovedTimers;
using TinyRPG.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyRPG.Player
{
    public class PlayerInteractor : MonoBehaviour, ICastable
    {
        [SerializeField] private float interactionRange = 2f;

        private bool isInteracting;
        private IInteractable activeInteractable;
        private CountdownTimer interactionTimer;

        private Camera mainCamera;
        private PlayerInputHandler playerInputHandler;

        public string CastActionName => activeInteractable.GetInteractionPrompt();
        public float CastProgress => interactionTimer.Progress;
        public bool IsCurrentlyCasting => interactionTimer.IsRunning;
        public bool InvertFill => false;

        private void Awake()
        {
            playerInputHandler = GetComponent<PlayerInputHandler>();
            mainCamera = Camera.main;

            interactionTimer = new CountdownTimer(0f);
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

            float duration = activeInteractable.GetInteractionTime();

            if (duration <= 0)
            {
                CompleteInteraction();
                return;
            }

            isInteracting = true;

            interactionTimer.Stop();
            interactionTimer = new CountdownTimer(duration);

            interactionTimer.OnTimerStop = () => CompleteInteraction();
            interactionTimer.Start();

            PlayerUIManager.Instance.ShowCastBar(this);
        }

        private void HandleActiveInteraction()
        {
            if (playerInputHandler.MovementInput != Vector2.zero || playerInputHandler.JumpTriggered)
            {
                CancelInteraction();
                return;
            }
        }

        private void CompleteInteraction()
        {
            if (activeInteractable == null) return;

            activeInteractable.Interact();

            CleanInteraction(true);
        }

        private void CancelInteraction()
        {
            PlayerUIManager.Instance.CancelCastBar();
            CleanInteraction(false);
        }

        private void CleanInteraction(bool hideInstantly)
        {
            activeInteractable = null;
            isInteracting = false;

            if (hideInstantly)
            {
                PlayerUIManager.Instance.HideCastBar();
            }

            interactionTimer.Stop();
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