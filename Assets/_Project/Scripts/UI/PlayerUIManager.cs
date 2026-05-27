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
        [SerializeField] private CastbarUI castBar;

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

        // Cast Bar
        public void ShowCastBar(ICastable source) => castBar.SetCastSource(source);
        public void SetCastBarProgress(float fillAmount, bool invertFill) => castBar.UpdateProgressBar(fillAmount, invertFill);
        public void HideCastBar() => castBar.Hide();
        public void CancelCastBar() => castBar.Cancel();

        // Target Frames
        public void ShowSelectedTargetFrame(Unit unit) => selectedTargetUnitFrame.Show(unit);
        public void HideSelectedTargetFrame() => selectedTargetUnitFrame.Hide();
    }
}