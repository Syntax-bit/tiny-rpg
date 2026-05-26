using TinyRPG.Gameplay;
using TinyRPG.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyRPG.Player
{
    public class PlayerTargeter : MonoBehaviour
    {
        private Unit selectedUnit;
        private bool wasDraggingThisClick;

        private Camera mainCamera;
        private PlayerInputHandler playerInputHandler;

        private void Awake()
        {
            mainCamera = Camera.main;
            playerInputHandler = GetComponent<PlayerInputHandler>();
        }

        private void Update()
        {
            if (playerInputHandler.LeftMouseButtonPressed)
            {
                HandleSelection();
            }

            if(playerInputHandler.CancelButtonPressed)
            {
                Deselect();
            }

            if(Keyboard.current.fKey.wasPressedThisFrame)
            {
                selectedUnit.TakeDamage(10);
            }
        }

        private void HandleSelection()
        {
            Unit hoveredUnit = GetUnit();

            if (hoveredUnit != null)
            {
                Select(hoveredUnit);
            }
        }

        private void Select(Unit unitToSelect)
        {
            selectedUnit = unitToSelect;
            PlayerUIManager.Instance.ShowSelectedTargetFrame(selectedUnit);
        }

        private void Deselect()
        {
            PlayerUIManager.Instance.HideSelectedTargetFrame();
            selectedUnit = null;
        }

        private Unit GetUnit()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Ray interactionRay = mainCamera.ScreenPointToRay(mouseScreenPos);

            if (Physics.Raycast(interactionRay, out RaycastHit hit))
            {
                if (hit.collider.gameObject.TryGetComponent<Unit>(out Unit unit))
                {
                    return unit;
                }
            }

            return null;
        }
    }
}