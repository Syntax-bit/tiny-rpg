using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TinyRPG.Gameplay;

namespace TinyRPG.UI
{
    public class Nameplate : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text healthPercentageText;

        private Unit cachedUnit;

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

        private void OnDestroy()
        {
            cachedUnit.OnHealthChanged -= UpdateHealthUI;
        }
    }
}