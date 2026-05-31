using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TinyRPG.Gameplay;
using System;
using Unity.VisualScripting;

public class EffectUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private Image durationOverlayImage;
    [SerializeField] private TMP_Text countText;

    private ActiveDoTBehavior activeEffect;
    private int stackCount = 1;
    private float remainingDuration;

    public ActiveDoTBehavior ActiveEffect => activeEffect;

    public void Initialize(AbilityData abilityData, ActiveDoTBehavior effect)
    {
        activeEffect = effect;
        iconImage.sprite = abilityData.icon;
        durationOverlayImage.sprite = abilityData.icon;
        countText.text = stackCount.ToString();
        remainingDuration = activeEffect.duration;

        UpdateStackVisuals();
    }

    private void Update()
    {
        if (activeEffect == null) return;

        remainingDuration -= Time.deltaTime;

        durationText.text = remainingDuration.ToString("F0");
        durationOverlayImage.fillAmount = 1 - (remainingDuration / activeEffect.duration);
    }

    public void UpdateStackCount(int currentStackCount)
    {
        remainingDuration = activeEffect.duration;

        if (activeEffect.isStackable)
        {
            stackCount = currentStackCount;
            UpdateStackVisuals();
        }
    }

    private void UpdateStackVisuals()
    {
        countText.text = stackCount > 1 ? stackCount.ToString() : "";
    }
}
