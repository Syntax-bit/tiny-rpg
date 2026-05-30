using System;
using TinyRPG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HotbarSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownOverlayImage;
    [SerializeField] private Image disabledOverlayImage;
    [SerializeField] private TMP_Text keybindText;

    private CooldownTracker cooldownTracker;
    private IHotbarAction hotbarAction;
    private float currentMaxCooldownDuration;

    private void Start()
    {
        enabled = false;
    }

    public void Initialize(int index, IHotbarAction hotbarAction, CooldownTracker cooldownTracker)
    {
        // Unsubscribe from the old tracker if we are re-initializing this slot during a swap
        if (this.cooldownTracker != null)
        {
            this.cooldownTracker.OnCooldownStarted -= HandleCooldownStarted;
            this.cooldownTracker.OnGlobalCooldownStarted -= HandleGlobalCooldownStarted;
        }

        this.hotbarAction = hotbarAction;
        this.cooldownTracker = cooldownTracker;

        if (this.cooldownTracker != null)
        {
            this.cooldownTracker.OnCooldownStarted += HandleCooldownStarted;
            this.cooldownTracker.OnGlobalCooldownStarted += HandleGlobalCooldownStarted;
        }

        keybindText.text = (index + 1).ToString();

        if (hotbarAction != null && hotbarAction.Icon != null)
        {
            cooldownOverlayImage.sprite = hotbarAction.Icon;
            disabledOverlayImage.sprite = hotbarAction.Icon;

            iconImage.sprite = hotbarAction.Icon;
            iconImage.gameObject.SetActive(true);
        }
        else
        {
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);

            iconImage.sprite = null;
            iconImage.gameObject.SetActive(false);
        }
    }

    private void HandleGlobalCooldownStarted(float gcdDuration)
    {
        if (hotbarAction == null) return;

        currentMaxCooldownDuration = Mathf.Max(currentMaxCooldownDuration, gcdDuration);
        enabled = true;

        if (cooldownOverlayImage != null)
        {
            cooldownOverlayImage.gameObject.SetActive(true);
        }
    }

    public void Use()
    {

    }

    private void HandleCooldownStarted(IHotbarAction action, float cooldownDuration)
    {
        if(action == hotbarAction)
        {
            currentMaxCooldownDuration = cooldownDuration;
            enabled = true;

            cooldownOverlayImage.fillAmount = 1;
            cooldownOverlayImage.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (hotbarAction == null || cooldownTracker == null)
        {
            DisableCooldownVisuals();
            return;
        }

        float cooldownProgress = cooldownTracker.GetCooldownProgress(hotbarAction);

        if (cooldownOverlayImage != null)
        {
            cooldownOverlayImage.fillAmount = cooldownProgress / currentMaxCooldownDuration;
        }

        // 🎯 FIXED: Once the fill hits zero, shut off the update loop entirely until next use cast sequence
        if (cooldownProgress <= 0f)
        {
            DisableCooldownVisuals();
        }
    }

    private void DisableCooldownVisuals()
    {
        if (cooldownOverlayImage != null)
        {
            cooldownOverlayImage.fillAmount = 0f;
            cooldownOverlayImage.gameObject.SetActive(false);
        }
        enabled = false;
    }

    private void OnDestroy()
    {
        if (cooldownTracker != null)
        {
            cooldownTracker.OnCooldownStarted -= HandleCooldownStarted;
            cooldownTracker.OnGlobalCooldownStarted -= HandleGlobalCooldownStarted;
        }
    }
}
