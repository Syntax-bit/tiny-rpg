using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DragDropManager : MonoBehaviour
{
    public static DragDropManager Instance { get; private set; }

    [SerializeField] private GameObject ghostDragObject;
    public int SourceSlotIndex { get; private set; } = -1;
    public bool IsDragging { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartDrag(Sprite icon, Vector3 position, int sourceSlotIndex)
    {
        ghostDragObject.SetActive(true);
        ghostDragObject.GetComponent<Image>().sprite = icon;
        ghostDragObject.transform.position = position;

        SourceSlotIndex = sourceSlotIndex;

        transform.SetAsLastSibling();
        IsDragging = true;
    }

    public void UpdateDragPosition()
    {
        if (ghostDragObject.activeSelf)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            ghostDragObject.transform.position = mousePosition;
        }
    }

    public void EndDrag()
    {
        ghostDragObject.SetActive(false);
        SourceSlotIndex = -1;
        IsDragging = false;
    }
}