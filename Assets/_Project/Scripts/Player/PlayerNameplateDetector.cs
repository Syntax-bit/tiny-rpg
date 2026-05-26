using TinyRPG.Gameplay;
using TinyRPG.UI;
using UnityEngine;

public class PlayerNameplateDetector : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float maxVisibilityDistance = 10f;

    private SphereCollider detectorCollider;

    private void Awake()
    {
        detectorCollider = GetComponent<SphereCollider>();

        detectorCollider.radius = maxVisibilityDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit) && !other.CompareTag("Player"))
        {
            PlayerUIManager.Instance.nameplateManager.CreateNewNameplate(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Unit>(out Unit unit))
        {
            PlayerUIManager.Instance.nameplateManager.RemoveNameplate(unit);
        }
    }
}
