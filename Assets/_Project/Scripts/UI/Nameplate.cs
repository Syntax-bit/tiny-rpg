using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TinyRPG.Gameplay;
using UnityEngine.EventSystems;

namespace TinyRPG.UI
{
    public class Nameplate : MonoBehaviour, IPointerDownHandler
    {
        [Header("References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text healthPercentageText;

        private Unit cachedUnit;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            cachedUnit.OnHealthChanged += UpdateHealthUI;
        }

        public void Initialize(Unit unit)
        {
            cachedUnit = unit;

            nameText.text = unit.UnitData.UnitName;

            UpdateHealthUI();
        }

        private void UpdateHealthUI()
        {
            healthFill.fillAmount = cachedUnit.GetNormalizedCurrentHealth();
            healthPercentageText.text = $"{cachedUnit.GetNormalizedCurrentHealth() * 100}%";
        }

        public void SetOpacity(float opacity)
        {
            canvasGroup.alpha = opacity;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (cachedUnit != null)
            {
                var player = FindFirstObjectByType<TinyRPG.Player.PlayerTargeter>();

                player.SelectUnit(cachedUnit);
            }
        }

        private void OnDestroy()
        {
            if (cachedUnit != null)
            {
                cachedUnit.OnHealthChanged -= UpdateHealthUI;
            }
        }
    }
}