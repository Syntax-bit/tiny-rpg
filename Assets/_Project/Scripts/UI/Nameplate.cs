using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TinyRPG.Gameplay;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace TinyRPG.UI
{
    public class Nameplate : MonoBehaviour, IPointerDownHandler
    {
        [Header("References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text healthPercentageText;
        [SerializeField] private Transform effectHolder;
        [SerializeField] private GameObject effectPrefab;

        private Unit cachedUnit;
        private Dictionary<AbilityData, EffectUI> activeEffectIcons = new Dictionary<AbilityData, EffectUI>();

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Initialize(Unit unit)
        {
            if (unit != null)
            {
                cachedUnit = unit;
                cachedUnit.OnHealthChanged += UpdateHealthUI;
                cachedUnit.OnEffectsChanged += UpdateEffects;
                cachedUnit.OnEffectExpired += RemoveEffectUI;

                nameText.text = unit.UnitData.UnitName;

                UpdateHealthUI();
            }
        }

        public void SetOpacity(float opacity)
        {
            canvasGroup.alpha = opacity;
        }

        public void Highlight()
        {
            canvasGroup.alpha = 1.0f;
            transform.localScale = Vector3.one * 1.35f;
        }

        public void Unhighlight()
        {
            transform.localScale = Vector3.one;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (cachedUnit != null)
            {
                var player = FindFirstObjectByType<TinyRPG.Player.PlayerTargeter>();

                player.SelectUnit(cachedUnit);
            }
        }

        private void UpdateHealthUI()
        {
            healthFill.fillAmount = cachedUnit.GetNormalizedCurrentHealth();
            healthPercentageText.text = $"{cachedUnit.GetNormalizedCurrentHealth() * 100}%";
        }

        private void UpdateEffects(ActiveDoTBehavior dot, AbilityData abilityData)
        {
            if (!activeEffectIcons.ContainsKey(abilityData))
            {
                GameObject effectInstance = Instantiate(effectPrefab, effectHolder);
                EffectUI effectUI = effectInstance.GetComponent<EffectUI>();

                effectUI.Initialize(abilityData, dot);

                effectUI.UpdateStackCount(dot.currentStacks);

                activeEffectIcons.Add(abilityData, effectUI);
            }
            else
            {
                activeEffectIcons[abilityData].UpdateStackCount(dot.currentStacks);
            }
        }

        private void RemoveEffectUI(ActiveDoTBehavior dot)
        {
            AbilityData keyToRemove = null;

            foreach (var pair in activeEffectIcons)
            {
                if (pair.Value != null && pair.Value.ActiveEffect == dot)
                {
                    keyToRemove = pair.Key;
                    break;
                }
            }

            if (keyToRemove != null)
            {
                if (activeEffectIcons[keyToRemove] != null)
                {
                    Destroy(activeEffectIcons[keyToRemove].gameObject);
                }
                activeEffectIcons.Remove(keyToRemove);
            }
        }

        private void UnsubscribeFromUnitEvents()
        {
            if (cachedUnit != null)
            {
                cachedUnit.OnHealthChanged -= UpdateHealthUI;
                cachedUnit.OnEffectsChanged -= UpdateEffects;
                cachedUnit.OnEffectExpired -= RemoveEffectUI;
            }
        }

        private void OnDestroy()
        {
            UnsubscribeFromUnitEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromUnitEvents();
        }
    }
}

