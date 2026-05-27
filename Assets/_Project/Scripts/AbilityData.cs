using System;
using System.Collections.Generic;
using TinyRPG.Gameplay;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Scriptable Objects/AbilityData")]
class AbilityData : ScriptableObject
{
    public string label;

    public AnimationClip animationClip;
    [Range(0f, 5f)] public float castTime = 1f;
    public Projectile vfxPrefab;

    [SerializeReference] public List<AbilityEffect> effects;

    void OnEnable()
    {
        if (string.IsNullOrEmpty(label)) label = name;
        if(effects == null) effects = new List<AbilityEffect>();
    }
}

[Serializable]
abstract class AbilityEffect
{
    public abstract void Execute(Unit caster, Unit target);
}

[Serializable]
class DamageEffect : AbilityEffect
{
    public int amount;

    public override void Execute(Unit caster, Unit target)
    {
        target.TakeDamage(amount);
    }
}