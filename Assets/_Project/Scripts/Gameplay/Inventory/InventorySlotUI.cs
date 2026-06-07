using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text stackSizeText;

    private InventorySlot slot;
    private int index;

    private void Awake()
    {
        stackSizeText.enabled = false;
    }

    public void Initialize(InventorySlot slot, int index)
    {
        this.slot = slot;
        this.index = index;
    }

    public void UpdateVisuals(InventorySlot slot)
    {
        if (slot == null) return;

        if(slot.IsEmpty)
        {
            ClearSlotVisuals();
            return;
        }

        iconImage.sprite = slot.itemData.Icon;

        stackSizeText.enabled = slot.itemData.IsStackable ? true : false;
        if (slot.itemData.IsStackable)
        {
            stackSizeText.text = slot.currentStackSize.ToString();
        }
    }

    private void ClearSlotVisuals()
    {
        iconImage.sprite = null;
        stackSizeText.text = "";
        stackSizeText.enabled = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent<InventorySlotUI>(out var draggedSlotUI))
        {
            int oldIndex = draggedSlotUI.index;
            int newIndex = this.index;

            InventoryController.Instance.Swap(oldIndex, newIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragDropManager.Instance.EndDrag();
        UpdateVisuals(slot);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragDropManager.Instance.UpdateDragPosition();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slot == null || slot.IsEmpty) return;

        DragDropManager.Instance.StartDrag(slot.itemData.Icon, Mouse.current.position.ReadValue(), index);
        ClearSlotVisuals();
    }
}
