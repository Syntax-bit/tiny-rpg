using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance { get; private set; }

    public Action OnInventoryChanged;

    [Header("Settings")]
    [SerializeField] private int inventorySize = 12;
    [SerializeField] private ItemData[] testItemData;

    private InventorySlot[] inventorySlots;

    public int InventorySize => inventorySize;
    public InventorySlot[] InventorySlots => inventorySlots;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance of InventoryController exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        inventorySlots = new InventorySlot[inventorySize];
        for(int i = 0; i < inventorySize; i++)
        {
            inventorySlots[i] = new InventorySlot(i);
        }
    }

    private void Update()
    {
        if(Keyboard.current.fKey.wasPressedThisFrame)
        {
            //Later add adding couple stack in one go
            AddItemToInventory(testItemData[UnityEngine.Random.Range(0, testItemData.Length)]);
        }
    }

    public bool AddItemToInventory(ItemData item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].CanStack(item))
            {
                inventorySlots[i].AddItem();

                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].IsEmpty)
            {
                inventorySlots[i].SetItem(item, 1);

                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false;
    }

    public void Swap(int oldSlotIndex, int newSlotIndex)
    {
        if (oldSlotIndex == newSlotIndex ||
            oldSlotIndex < 0 || oldSlotIndex >= inventorySize ||
            newSlotIndex < 0 || newSlotIndex >= inventorySize)
        {
            Debug.LogError("Invalid slot index for swapping!");
            return;
        }

        InventorySlot oldSlot = inventorySlots[oldSlotIndex];
        InventorySlot newSlot = inventorySlots[newSlotIndex];

        ItemData tempItemData = oldSlot.itemData;
        int tempStackSize = oldSlot.currentStackSize;

        // If the new slot is empty, we can simply move the item. Otherwise, we swap the contents of the two slots.
        if (newSlot.IsEmpty)
        {
            oldSlot.ClearSlot();
        }
        else
        {
            oldSlot.SetItem(newSlot.itemData, newSlot.currentStackSize);
        }

        // If the old slot is empty clear it. Otherwise, swap
        if (tempItemData == null)
        {
            newSlot.ClearSlot();
        }
        else
        {
            newSlot.SetItem(tempItemData, tempStackSize);
        }

        OnInventoryChanged?.Invoke();
    }
}
