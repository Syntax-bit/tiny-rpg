using System;
using System.Collections.Generic;
using TinyRPG.Gameplay;
using TinyRPG.Player;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Scriptable Objects/AbilityData")]
public class AbilityData : ScriptableObject, IHotbarAction
{
    public string label;
    [Range(0f, 5f)] public float castTime = 1f;
    public float range = 10f;
    [Range(0f, 60f)] public float cooldown = 5f;
    public bool canCastWhileMoving;
    public bool triggersGlobalCooldown = true;

    [Header("Gameplay Effects")]
    [SerializeReference] public Ability ability;

    [Header("Targeting")]
    [SerializeReference] [SelectableStrategy] public TargetingStrategy targetingStrategy;

    public void Target(PlayerTargeter playerTargeter, Action<Unit> onTargetAcquired)
    {
        if (targetingStrategy != null)
        {
            targetingStrategy.Start(this, playerTargeter, onTargetAcquired);
        }
    }

    [Header("Visuals")]
    public Sprite icon;
    public AnimationClip animationClip;
    public GameObject castVfx;
    public GameObject runningVfx;

    public string Label => label;
    public Sprite Icon => icon;
    public float Cooldown => cooldown;

    void OnEnable()
    {
        if (string.IsNullOrEmpty(label)) label = name;

        if (ability == null)
        {
            ability = new Ability();
        }

        if (ability.effects == null) ability.effects = new List<IEffectFactory<Unit>>();

        if (targetingStrategy == null)
        {
            targetingStrategy = new SelfTargetingStrategy();
        }
    }

    public void Execute(Unit caster, Unit target, Vector3 targetWorldPosition)
    {
        ability.Execute(caster, target, targetWorldPosition, this);
    }

    public void Use(GameObject user)
    {
        user.GetComponent<PlayerAbilityCaster>().TryCast(this);
    }
}

[Serializable]
public class Ability
{
    [SelectableEffect]
    [SerializeReference] public List<IEffectFactory<Unit>> effects;

    // 🎯 FIXED: Accept the world position parameter straight through the pipeline execution call
    public void Execute(Unit caster, Unit target, Vector3 targetWorldPosition, AbilityData abilityData)
    {
        if (effects == null) return;

        if (abilityData.targetingStrategy is AOETargetingStrategy aoeStrategy)
        {
            float radius = aoeStrategy.aoeRadius;

            // 🎯 FIXED: Overlap check now drops perfectly onto your mouse coordinates!
            Vector3 blastCenter = targetWorldPosition;

            Collider[] colliders = Physics.OverlapSphere(blastCenter, radius);

            foreach (var col in colliders)
            {
                if (col.TryGetComponent<Unit>(out Unit hitUnit))
                {
                    if (hitUnit == caster) continue; // Keep the self-damage block safe

                    ApplyEffectsToTarget(caster, hitUnit, abilityData);
                }
            }
        }
        else
        {
            if (abilityData.castVfx != null && target != null)
            {
                GameObject.Instantiate(abilityData.castVfx, target.transform.position, Quaternion.identity);
            }

            // Standard Single-Target fallback loop path
            foreach (var effect in effects)
            {
                if (effect == null) continue;
                var runtimeEffect = effect.Create();
                target.ApplyEffect(runtimeEffect, caster, abilityData);
            }
        }
    }

    private void ApplyEffectsToTarget(Unit caster, Unit target, AbilityData abilityData)
    {
        foreach (var effect in effects)
        {
            if (effect == null) continue;
            var runtimeEffect = effect.Create();
            target.ApplyEffect(runtimeEffect, caster, abilityData);
        }
    }

    void HandleVFX(Unit target, AbilityData abilityData)
    {
        if (target is MonoBehaviour targetMb)
        {
            if (abilityData.castVfx)
            {
                GameObject.Instantiate(abilityData.castVfx, targetMb.transform.position, Quaternion.identity);
            }

            if (abilityData.runningVfx)
            {
                ParticleSystem runningVfxInstance = GameObject.Instantiate(abilityData.runningVfx, targetMb.transform).gameObject.GetComponent<ParticleSystem>();
                runningVfxInstance.transform.localPosition = Vector3.zero;
                GameObject.Destroy(runningVfxInstance.gameObject, runningVfxInstance.main.duration);
            }
        }
    }
}