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

        private TargetingStrategy currentStrategy;

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
                if(currentStrategy == null)
                {
                    HandleSelection();
                }
                else
                {
                    EvaluateStrategyConfirmation();
                }
            }

            if(playerInputHandler.CancelButtonPressed)
            {
                Deselect();
            }

            if(playerInputHandler.TabTargetButtonPressed)
            {
                HandleTabTargeting();
            }

            if(currentStrategy != null && currentStrategy.IsTargeting)
            {
                currentStrategy.Update();
            }
        }

        private void EvaluateStrategyConfirmation()
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

            LayerMask mask = Physics.DefaultRaycastLayers;
            if (currentStrategy is AOETargetingStrategy aoe) mask = aoe.groundLayerMask;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, mask))
            {
                if (currentStrategy is AOETargetingStrategy aoeStrategy)
                {
                    aoeStrategy.ConfirmClick(hit);
                }
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

        public void SetCurrentStrategy(TargetingStrategy strategy) => currentStrategy = strategy;
        public void ClearCurrentStrategy() => currentStrategy = null;

        public void SelectUnit(Unit unitToSelect)
        {
            if (selectedUnit == unitToSelect) return;
            Unit oldUnit = selectedUnit;

            if (oldUnit != null)
            {
                oldUnit.SetSelectionVisual(false);
                PlayerUIManager.Instance.nameplateManager.UnhighlightNameplate(oldUnit);

                if (!unitsInRange.Contains(oldUnit))
                {
                    PlayerUIManager.Instance.nameplateManager.RemoveNameplate(oldUnit);
                }
            }

            selectedUnit = unitToSelect;

            if (selectedUnit != null)
            {
                PlayerUIManager.Instance.nameplateManager.HighlightNameplate(selectedUnit);
                PlayerUIManager.Instance.nameplateManager.CreateNewNameplate(selectedUnit);
                PlayerUIManager.Instance.ShowSelectedTargetFrame(selectedUnit);
                selectedUnit.SetSelectionVisual(true);

                int trackingIndex = unitsInRange.IndexOf(selectedUnit);
                if (trackingIndex != -1)
                {
                    currentTabTargetIndex = trackingIndex;
                }
            }
        }

        private void Deselect()
        {
            if (selectedUnit != null)
            {
                selectedUnit.SetSelectionVisual(false);
                PlayerUIManager.Instance.nameplateManager.UnhighlightNameplate(selectedUnit);

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

            if (currentTabTargetIndex >= unitsInRange.Count)
            {
                currentTabTargetIndex = Mathf.Max(0, unitsInRange.Count - 1);
            }
        }

        private void HandleTabTargeting()
        {
            if (unitsInRange.Count == 0) return;

            currentTabTargetIndex++;
            if (currentTabTargetIndex >= unitsInRange.Count)
            {
                currentTabTargetIndex = 0;
            }

            SelectUnit(unitsInRange[currentTabTargetIndex]);
        }
    }
}