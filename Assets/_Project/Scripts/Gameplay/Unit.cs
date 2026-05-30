using System;
using System.Collections.Generic;
using TinyRPG.UI;
using UnityEngine;

namespace TinyRPG.Gameplay
{
    public class Unit : MonoBehaviour
    {
        public UnitData UnitData => unitData;

        public event Action OnHealthChanged;

        [SerializeField] private UnitData unitData;
        [SerializeField] private GameObject selectionIndicator;

        private int currentHealth;
        private Dictionary<AbilityData, List<ActiveDoTBehavior>> activeEffects = new Dictionary<AbilityData, List<ActiveDoTBehavior>>();

        private void Awake()
        {
            currentHealth = unitData.MaxHealth;
        }

        public float GetNormalizedCurrentHealth()
        {
            return (float)currentHealth / (float)unitData.MaxHealth;
        }

        public void SetSelectionVisual(bool isSelected)
        {
            selectionIndicator.SetActive(isSelected);
        }

        public void ApplyEffect(IEffect<Unit> effect, Unit caster, AbilityData abilityData)
        {
            effect.Apply(caster, this, abilityData);
        }

        public void RegisterDoT(DamageOverTimeEffect dotEffect, Unit caster, AbilityData abilityData)
        {
            if (!activeEffects.ContainsKey(abilityData))
            {
                activeEffects[abilityData] = new List<ActiveDoTBehavior>();
            }

            List<ActiveDoTBehavior> currentStacks = activeEffects[abilityData];

            // Refresh non-stackable effects if already present
            if (!dotEffect.IsStackable && currentStacks.Count > 0)
            {
                if (currentStacks[0] != null)
                {
                    currentStacks[0].Refresh();
                }
                return;
            }

            // Prevent adding new stacks if we've already reached the max stack limit configuration
            if (dotEffect.IsStackable && currentStacks.Count >= dotEffect.MaxStackSize)
            {
                return;
            }

            // Create a new DoT tracking component using values passed down from the effect payload
            ActiveDoTBehavior dotInstance = gameObject.AddComponent<ActiveDoTBehavior>();
            dotInstance.Initialize(
                dotEffect.Duration,
                dotEffect.TickInterval,
                dotEffect.DamagePerTick,
                this,
                dotEffect.DotRunningVfxPrefab
            );

            currentStacks.Add(dotInstance);
            dotInstance.OnEffectExpired += HandleEffectExpired;
        }

        private void HandleEffectExpired(ActiveDoTBehavior expiredDot)
        {
            foreach (var pair in activeEffects)
            {
                if(pair.Value.Contains(expiredDot))
                {
                    pair.Value.Remove(expiredDot);
                    break;
                }
            }
            //TriggerAuraUIUpdate();
        }

        public void TakeDamage(int amount)
        {
            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke();

            if (currentHealth <= 0)
            {
                Debug.Log(unitData.UnitName + " died!");
                ClearAllActiveEffects();

                Destroy(gameObject);
            }
        }

        public void Heal(int amount)
        {
            currentHealth = Mathf.Min(unitData.MaxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke();
        }

        private void ClearAllActiveEffects()
        {
            foreach(var pair in activeEffects)
            {
                foreach(var dot in pair.Value)
                {
                    if(dot != null) dot.OnEffectExpired -= HandleEffectExpired;
                }

                pair.Value.Clear();
            }

            activeEffects.Clear();
        }
        
        private void OnDestroy()
        {
            PlayerUIManager.Instance.nameplateManager.RemoveNameplate(this);            
        }
    }
}