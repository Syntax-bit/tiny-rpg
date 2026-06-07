using TinyRPG.Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class InventorySlot
{
    public ItemData itemData { get; private set; }
    public int currentStackSize { get; private set; }

    private int slotIndex;

    public bool IsEmpty => itemData == null;
    public int SlotIndex => slotIndex;


    public InventorySlot(int index)
    {
        slotIndex = index;
        itemData = null;
        currentStackSize = 0;
    }

    public void SetItem(ItemData itemData, int stackSize)
    {
        this.itemData = itemData;
        currentStackSize = stackSize;
    }

    public bool AddItem()
    {
        if (currentStackSize < itemData.StackSize)
        {
            currentStackSize++;
            return true;
        }
        
        Debug.LogWarning("Cannot add more items to this stack!");
        return false;
    }

    public void ClearSlot()
    {
        itemData = null;
        currentStackSize = 0;
    }

    public bool CanStack(ItemData newItemData)
    {
        return itemData != null && itemData == newItemData && itemData.IsStackable && currentStackSize < itemData.StackSize;
    }
}
