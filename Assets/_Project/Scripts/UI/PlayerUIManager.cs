using TinyRPG.Gameplay;
using UnityEngine;

namespace TinyRPG.UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager Instance;

        [field: SerializeField] public NameplateManager nameplateManager {  get; private set; }
        [SerializeField] private UnitFrame playerUnitFrame;
        [SerializeField] private UnitFrame selectedTargetUnitFrame;
        [SerializeField] private InteractionCastbar interactionCastbar;

        private Unit playerUnit;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            playerUnit = GetComponent<Unit>();
        }

        private void Start()
        {
            playerUnitFrame.Show(playerUnit);
        }

        //Interaction Bar
        public void ShowInteractionBar(string prompt) => interactionCastbar.Show(prompt);
        public void SetInteractionBarProgress(float fillAmount) => interactionCastbar.UpdateProgressBar(fillAmount);
        public void HideInteractionBar() => interactionCastbar.Hide();
        public void CancelInteraction() => interactionCastbar.Cancel();

        //Target Frames
        public void ShowSelectedTargetFrame(Unit unit) => selectedTargetUnitFrame.Show(unit);
        public void HideSelectedTargetFrame() => selectedTargetUnitFrame.Hide();
    }
}