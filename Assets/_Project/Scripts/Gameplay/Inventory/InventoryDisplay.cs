using System;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject inventorySlotUI;
    [SerializeField] private Transform inventorySlotHolder;

    private InventorySlotUI[] uiSlots;
    private bool isInitialized;

    private void Start()
    {
        if (InventoryController.Instance != null)
        {
            InitializeSlots();
            UpdateInventoryDisplay();
        }
    }

    private void InitializeSlots()
    {
        uiSlots = new InventorySlotUI[InventoryController.Instance.InventorySize];

        for (int i = 0; i < InventoryController.Instance.InventorySize; i++)
        {
            GameObject slot = Instantiate(inventorySlotUI, inventorySlotHolder);
            slot.name = $"SlotUI_{i}";

            if(slot.TryGetComponent<InventorySlotUI>(out var slotUI))
            {
                uiSlots[i] = slotUI;
                slotUI.Initialize(InventoryController.Instance.InventorySlots[i], InventoryController.Instance.InventorySlots[i].SlotIndex);
            }
        }

        isInitialized = true;
    }

    private void OnEnable()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventoryChanged += UpdateInventoryDisplay;
        }

        UpdateInventoryDisplay();
    }

    private void OnDisable()
    {
        if (InventoryController.Instance != null)
        {
            InventoryController.Instance.OnInventoryChanged -= UpdateInventoryDisplay;
        }
    }

    private void UpdateInventoryDisplay()
    {
        if (!isInitialized || uiSlots == null) return;

        InventorySlot[] slots = InventoryController.Instance.InventorySlots;

        for(int i = 0; i < slots.Length; i++)
        {
            uiSlots[i].UpdateVisuals(slots[i]);
        }
    }
}
