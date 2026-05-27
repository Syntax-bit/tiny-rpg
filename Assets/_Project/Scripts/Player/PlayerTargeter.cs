using NUnit.Framework;
using System.Collections.Generic;
using TinyRPG.Gameplay;
using TinyRPG.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TinyRPG.Player
{
    public class PlayerTargeter : MonoBehaviour
    {
        public Unit selectedUnit { get; private set; }

        private List<Unit> unitsInRange = new List<Unit>();
        private int currentTabTargetIndex;

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

            if(playerInputHandler.TabTargetButtonPressed)
            {
                HandleTabTargeting();
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
                SelectUnit(hoveredUnit);
            }
        }

        public void SelectUnit(Unit unitToSelect)
        {
            if (selectedUnit == unitToSelect) return;
            Unit oldUnit = selectedUnit;

            if (oldUnit != null)
            {
                oldUnit.SetSelectionVisual(false);

                if (!unitsInRange.Contains(oldUnit))
                {
                    PlayerUIManager.Instance.nameplateManager.RemoveNameplate(oldUnit);
                }
            }

            selectedUnit = unitToSelect;

            if (selectedUnit != null)
            {
                PlayerUIManager.Instance.nameplateManager.CreateNewNameplate(selectedUnit);
                PlayerUIManager.Instance.ShowSelectedTargetFrame(selectedUnit);
                selectedUnit.SetSelectionVisual(true);
            }
        }

        private void Deselect()
        {
            if (selectedUnit != null)
            {
                selectedUnit.SetSelectionVisual(false);

                if(!unitsInRange.Contains(selectedUnit))
                {
                    PlayerUIManager.Instance.nameplateManager.RemoveNameplate(selectedUnit);
                }
            }

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

        public void AddToUnitsInRange(Unit unit)
        {
            if(unit == null || unitsInRange.Contains(unit)) return;

            unitsInRange.Add(unit);
        }

        public void RemoveFromUnitsInRange(Unit unit)
        {
            if (unit == null || !unitsInRange.Contains(unit)) return;

            unitsInRange.Remove(unit);
        }

        private void HandleTabTargeting()
        {
            if (unitsInRange.Count == 0) return;

            SelectUnit(unitsInRange[currentTabTargetIndex]);

            currentTabTargetIndex++;
            if (currentTabTargetIndex > unitsInRange.Count - 1)
            {
                currentTabTargetIndex = 0;
            }
        }
    }
}