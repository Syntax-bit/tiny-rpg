using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager Instance;

    [SerializeField] private InteractionCastbar interactionCastbar;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowInteractionBar(string prompt) => interactionCastbar.Show(prompt);
    public void SetInteractionBarProgress(float fillAmount) => interactionCastbar.UpdateProgressBar(fillAmount);
    public void HideInteractionBar() => interactionCastbar.Hide();
    public void CancelInteraction() => interactionCastbar.Cancel();
}
