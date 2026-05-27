using TinyRPG.Gameplay;
using TinyRPG.Player;
using TinyRPG.UI;
using UnityEngine;

public class PlayerNameplateDetector : MonoBehaviour
{
    [Header("Settings")]
    [field: SerializeField] public float maxVisibilityDistance {get; private set;}

    private SphereCollider detectorCollider;
    private PlayerTargeter playerTargeter;

    private void Awake()
    {
        detectorCollider = GetComponent<SphereCollider>();
        playerTargeter = GetComponentInParent<PlayerTargeter>();

        detectorCollider.radius = maxVisibilityDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit) && !other.CompareTag("Player"))
        {
            PlayerUIManager.Instance.nameplateManager.CreateNewNameplate(unit);
            playerTargeter.AddToUnitsInRange(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit))
        {
            playerTargeter.RemoveFromUnitsInRange(unit);

            if (unit != playerTargeter.selectedUnit)
            {
                PlayerUIManager.Instance.nameplateManager.RemoveNameplate(unit);
            }
        }
    }
}
