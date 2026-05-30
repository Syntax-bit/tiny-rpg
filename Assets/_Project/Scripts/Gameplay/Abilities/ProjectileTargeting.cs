using System;
using TinyRPG.Player;
using TinyRPG.Gameplay;
using UnityEngine;

public class ProjectileTargeting : TargetingStrategy
{
    public GameObject projectilePrefab;
    public float speed = 10f;

    public override void Start(AbilityData abilityData, PlayerTargeter playerTargeter, Action<Unit> onTargetAcquired)
    {
        base.Start(abilityData, playerTargeter, onTargetAcquired);

        if (playerTargeter.selectedUnit != null)
        {
            // Instantly pass the selected unit back as our valid casting target context
            this.onTargetAcquired?.Invoke(playerTargeter.selectedUnit);
        }
        else
        {
            Debug.LogWarning("Cannot cast projectile: No target selected!");
        }

        Cancel();
    }

    public override void Cancel()
    {
        
    }

    public override void Update()
    {
    }
}
