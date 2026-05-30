using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TinyRPG.Gameplay;
using TinyRPG.Player;

[Serializable]
public class AOETargetingStrategy : TargetingStrategy
{
    public GameObject aoePreviewPrefab;
    public float aoeRadius = 5f;
    public LayerMask groundLayerMask;
    private Vector3 lastConfirmedPosition;

    private GameObject previewInstance;
    private Camera mainCamera;

    public override void Start(AbilityData abilityData, PlayerTargeter playerTargeter, Action<Unit> onTargetAcquired)
    {
        base.Start(abilityData, playerTargeter, onTargetAcquired);

        mainCamera = Camera.main;
        playerTargeter.SetCurrentStrategy(this);

        if (aoePreviewPrefab != null)
        {
            previewInstance = GameObject.Instantiate(aoePreviewPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    // 🎯 FIXED: Uses the base class signature naming convention (OnUpdate)
    public override void Update()
    {
        if (!isTargeting || previewInstance == null) return;

        // Stick visual preview indicator slightly above the floor mesh to prevent z-fighting flicker
        previewInstance.transform.position = GetMouseWorldPosition() + new Vector3(0, 0.05f, 0);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayerMask))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    public Vector3 GetLastConfirmedPosition() => lastConfirmedPosition;

    // 🎯 EXECUTED BY PLAYER_TARGETER: This handles confirmation perfectly when passed down from your input broker!
    public void ConfirmClick(RaycastHit hit)
    {
        if (!isTargeting) return;

        lastConfirmedPosition = hit.point; // 🎯 NEW: Cache the exact layout position metrics right on click!

        Unit casterUnit = playerTargeter.GetComponent<Unit>();
        onTargetAcquired?.Invoke(casterUnit);
        Cancel();
    }

    public override void Cancel()
    {
        isTargeting = false;

        if (playerTargeter != null)
        {
            playerTargeter.ClearCurrentStrategy();
        }

        if (previewInstance != null)
        {
            GameObject.Destroy(previewInstance);
        }
    }
}