using System;
using System.Collections.Generic;
using TinyRPG.UI;
using UnityEngine;

namespace TinyRPG.Gameplay
{
    [RequireComponent(typeof(ResourceController))]
    public class Unit : MonoBehaviour
    {
        public UnitData UnitData => unitData;

        public event Action OnHealthChanged;
        public event Action OnManaChanged;
        public event Action<ActiveDoTBehavior, AbilityData> OnEffectsChanged;
        public event Action<ActiveDoTBehavior> OnEffectExpired;

        [SerializeField] private UnitData unitData;
        [SerializeField] private GameObject selectionIndicator;

        private int currentHealth;
        private Dictionary<AbilityData, ActiveDoTBehavior> activeEffects = new Dictionary<AbilityData, ActiveDoTBehavior>();
        private ResourceController resourceController;

        private void Awake()
        {
            currentHealth = unitData.MaxHealth;
            resourceController = GetComponent<ResourceController>();
        }

        private void Start()
        {
            resourceController.OnResourceChanged += HandleResourceChange;
        }

        public Dictionary<AbilityData, ActiveDoTBehavior> GetActiveEffects() => activeEffects;

        public float GetNormalizedCurrentHealth()
        {
            return (float)currentHealth / (float)unitData.MaxHealth;
        }

        public float GetNormalizedCurrentResource()
        {
            return resourceController.GetNormalizedCurrentResource();
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
            if (activeEffects.ContainsKey(abilityData) && activeEffects[abilityData] != null)
            {
                ActiveDoTBehavior runningDot = activeEffects[abilityData];

                runningDot.Refresh();
                runningDot.IncrementStack();

                OnEffectsChanged?.Invoke(runningDot, abilityData);
                return;
            }

            // No existing DoT of this type, create a new one
            ActiveDoTBehavior dotInstance = gameObject.AddComponent<ActiveDoTBehavior>();
            dotInstance.Initialize(
                dotEffect.Duration,
                dotEffect.TickInterval,
                dotEffect.DamagePerTick,
                this,
                dotEffect.DotRunningVfxPrefab,
                dotEffect.IsStackable,
                dotEffect.MaxStackSize
            );

            activeEffects.Add(abilityData, dotInstance);
            dotInstance.OnEffectExpired += HandleEffectExpired;

            OnEffectsChanged?.Invoke(dotInstance, abilityData);
        }

        private void HandleResourceChange()
        {
            OnManaChanged?.Invoke();
        }

        private void HandleEffectExpired(ActiveDoTBehavior expiredDot)
        {
            AbilityData keyToRemove = null;

            foreach (var pair in activeEffects)
            {
                if (pair.Value == expiredDot)
                {
                    keyToRemove = pair.Key;
                    break;
                }
            }

            if (keyToRemove != null)
            {
                activeEffects.Remove(keyToRemove);
                OnEffectExpired?.Invoke(expiredDot);
            }
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
            foreach (var pair in activeEffects)
            {
                if (pair.Value != null)
                {
                    pair.Value.OnEffectExpired -= HandleEffectExpired;
                    Destroy(pair.Value);
                }
            }
            activeEffects.Clear();
        }
        
        private void OnDestroy()
        {
            PlayerUIManager.Instance.nameplateManager.RemoveNameplate(this);      
            resourceController.OnResourceChanged -= HandleResourceChange;
        }
    }
}


public enum Faction
{
    Humans,
    Monsters,
    Neutral
}