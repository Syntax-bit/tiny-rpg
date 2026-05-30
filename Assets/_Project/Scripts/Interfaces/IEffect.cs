using System;
using UnityEngine;
using TinyRPG.Gameplay;

public interface IEffect<T>
{
    void Apply(Unit caster, T target, AbilityData abilityData);
}

public interface IEffectFactory<T>
{
    IEffect<T> Create();
}

// ==========================================
// DIRECT DAMAGE PIPELINE
// ==========================================
[Serializable]
public class DamageEffectFactory : IEffectFactory<Unit>
{
    public int damageAmount = 10;

    public IEffect<Unit> Create()
    {
        return new DamageEffect(damageAmount);
    }
}

// 🎯 Changed from struct to class to allow clean interface mapping
public class DamageEffect : IEffect<Unit>
{
    private readonly int damageAmount;

    public DamageEffect(int damageAmount)
    {
        this.damageAmount = damageAmount;
    }

    public void Apply(Unit caster, Unit target, AbilityData abilityData)
    {
        target.TakeDamage(damageAmount);
    }
}

// ==========================================
// DAMAGE OVER TIME (DoT) PIPELINE
// ==========================================
[Serializable]
public class DamageOverTimeEffectFactory : IEffectFactory<Unit>
{
    public float duration = 3f;
    public float tickInterval = 1f;
    public int damagePerTick = 10;

    public bool isStackable = false;
    [Range(1, 10)] public int maxStackSize = 1;

    [Header("Visuals")]
    public GameObject dotRunningVfxPrefab;

    public IEffect<Unit> Create()
    {
        // Pass blueprint metrics straight through a clean initialization constructor
        return new DamageOverTimeEffect(duration, tickInterval, damagePerTick, isStackable, maxStackSize, dotRunningVfxPrefab);
    }
}

// 🎯 CHANGED TO CLASS: Prevents boxing performance costs and tracks state parameters accurately
public class DamageOverTimeEffect : IEffect<Unit>
{
    public float Duration { get; }
    public float TickInterval { get; }
    public int DamagePerTick { get; }
    public bool IsStackable { get; }
    public int MaxStackSize { get; }
    public GameObject DotRunningVfxPrefab { get; }

    public DamageOverTimeEffect(float duration, float tickInterval, int damagePerTick, bool isStackable, int maxStackSize, GameObject vfxPrefab)
    {
        Duration = duration;
        TickInterval = tickInterval;
        DamagePerTick = damagePerTick;
        IsStackable = isStackable;
        MaxStackSize = maxStackSize;
        DotRunningVfxPrefab = vfxPrefab;
    }

    public void Apply(Unit caster, Unit target, AbilityData abilityData)
    {
        target.RegisterDoT(this, caster, abilityData);
    }
}

// HEALING

[Serializable]
public class HealingEffectFactory : IEffectFactory<Unit>
{
    public int healingAmount = 10;

    public IEffect<Unit> Create()
    {
        return new HealingEffect(healingAmount);
    }
}

// 🎯 Changed from struct to class to allow clean interface mapping
public class HealingEffect : IEffect<Unit>
{
    private readonly int healingAmount;

    public HealingEffect(int healingAmount)
    {
        this.healingAmount = healingAmount;
    }

    public void Apply(Unit caster, Unit target, AbilityData abilityData)
    {
        target.Heal(healingAmount);
    }
}