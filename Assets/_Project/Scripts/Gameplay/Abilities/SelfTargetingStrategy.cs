using System;
using TinyRPG.Player;
using TinyRPG.Gameplay;

public class SelfTargetingStrategy : TargetingStrategy
{
    // 🎯 FIXED: Match the base class signature parameters
    public override void Start(AbilityData abilityData, PlayerTargeter playerTargeter, Action<Unit> onTargetAcquired)
    {
        // Cache variables cleanly using the base implementation setup
        base.Start(abilityData, playerTargeter, onTargetAcquired);

        if (playerTargeter.transform.TryGetComponent<Unit>(out Unit unit))
        {
            // Now this field is fully populated and will fire perfectly!
            this.onTargetAcquired?.Invoke(unit);
        }

        // Self-casting finishes instantly on the exact same frame, so close its loop
        Cancel();
    }

    public override void Update()
    {
        // No tick updates needed for self-targeting
    }

    public override void Cancel()
    {
        isTargeting = false;
    }
}