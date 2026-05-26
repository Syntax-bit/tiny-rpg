using TinyRPG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TinyRPG.UI
{
    public class UnitFrame : MonoBehaviour
    {
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text healthText;

        [SerializeField] private TMP_Text nameText;

        private Unit trackedUnit;

        public void UpdateUnitFrame()
        {
            healthFill.fillAmount = trackedUnit.GetNormalizedCurrentHealth();
            healthText.text = $"{trackedUnit.GetNormalizedCurrentHealth() * 100}%";

            nameText.text = trackedUnit.UnitData.UnitName;
        }

        public void Show(Unit unit)
        {
            //Clean up old unit data
            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged -= UpdateUnitFrame;
            }

            trackedUnit = unit;
            trackedUnit.OnHealthChanged += UpdateUnitFrame;

            gameObject.SetActive(true);
            UpdateUnitFrame();
        }

        public void Hide()
        {
            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged -= UpdateUnitFrame;
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Hide();
        }
    }
}