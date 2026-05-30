using System;
using TinyRPG.Gameplay;
using TinyRPG.Player;
using UnityEngine;

public abstract class TargetingStrategy
{
    protected AbilityData abilityData;
    protected PlayerTargeter playerTargeter;
    protected Action<Unit> onTargetAcquired;
    protected bool isTargeting;

    public bool IsTargeting => isTargeting;

    public virtual void Start(AbilityData abilityData, PlayerTargeter playerTargeter, Action<Unit> onTargetAcquired)
    {
        this.abilityData = abilityData;
        this.playerTargeter = playerTargeter;
        this.onTargetAcquired = onTargetAcquired;
        isTargeting = true;
    }

    public abstract void Update();
    public abstract void Cancel();
}

