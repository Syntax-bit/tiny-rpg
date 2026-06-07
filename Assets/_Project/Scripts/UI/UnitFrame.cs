using TinyRPG.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TinyRPG.UI
{
    public class UnitFrame : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image healthFill;
        [SerializeField] private TMP_Text healthPercentageText;
        [SerializeField] private Image resourceFill;
        [SerializeField] private TMP_Text resourcePercentageText;
        [SerializeField] private TMP_Text nameText;

        [SerializeField] private Color friendlyColor;
        [SerializeField] private Color neutralColor;
        [SerializeField] private Color enemyColor;

        private Unit trackedUnit;

        public void UpdateUnitFrame()
        {
            healthFill.fillAmount = trackedUnit.GetNormalizedCurrentHealth();
            healthPercentageText.text = $"{trackedUnit.GetNormalizedCurrentHealth() * 100}%";

            resourceFill.fillAmount = trackedUnit.GetNormalizedCurrentResource();
            resourcePercentageText.text = $"{trackedUnit.GetNormalizedCurrentResource() * 100}%";

            nameText.text = trackedUnit.UnitData.UnitName;

            switch (trackedUnit.UnitData.UnitFaction)
            {
                case Faction.Humans:
                    healthFill.color = friendlyColor;
                    break;
                case Faction.Monsters:
                    healthFill.color = enemyColor;
                    break;
                case Faction.Neutral:
                    healthFill.color = neutralColor;
                    break;
            }
        }

        public void Show(Unit unit)
        {
            //Clean up old unit data
            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged -= UpdateUnitFrame;
                trackedUnit.OnManaChanged -= UpdateUnitFrame;
            }

            trackedUnit = unit;
            trackedUnit.OnHealthChanged += UpdateUnitFrame;
            trackedUnit.OnManaChanged += UpdateUnitFrame;

            gameObject.SetActive(true);
            UpdateUnitFrame();
        }

        public void Hide()
        {
            if (trackedUnit != null)
            {
                trackedUnit.OnHealthChanged -= UpdateUnitFrame;
                trackedUnit.OnManaChanged -= UpdateUnitFrame;
            }

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Hide();
        }
    }
}