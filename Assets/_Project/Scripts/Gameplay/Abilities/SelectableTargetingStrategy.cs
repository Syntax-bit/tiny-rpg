using TinyRPG.Gameplay;
using TinyRPG.Player;
using System;

public class SelectableTargetingStrategy : TargetingStrategy
{
    public override void Start(AbilityData abilityData, PlayerTargeter playerTargeter, Action<Unit> onTargetAcquired)
    {
        base.Start(abilityData, playerTargeter, onTargetAcquired);

        if (playerTargeter.selectedUnit != null)
        {
            this.onTargetAcquired?.Invoke(playerTargeter.selectedUnit);
        }

        Cancel();
    }

    public override void Cancel()
    {
        isTargeting = false;
    }

    public override void Update()
    {
        
    }
}
