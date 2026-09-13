using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public ItemData item;
    public int amount;

    public InventorySlot(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }
}

public class Inventory : MonoBehaviour
{
    public int maxSlots = 10; 
    public List<InventorySlot> slots = new List<InventorySlot>();

    // EVENTO: Para avisar a la UI cuando haya cambios
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    public bool AddItem(ItemData itemToAdd, int amountToAdd)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToAdd && slot.amount < slot.item.maxStack)
            {
                slot.amount += amountToAdd;
                onItemChangedCallback?.Invoke(); // Avisar a la UI
                return true; 
            }
        }

        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(itemToAdd, amountToAdd));
            onItemChangedCallback?.Invoke(); // Avisar a la UI
            return true; 
        }

        Debug.Log("¡El inventario está lleno!");
        return false; 
    }

    public bool RemoveItem(ItemData itemToRemove)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToRemove && slot.amount > 0)
            {
                slot.amount--;
                if (slot.amount == 0)
                {
                    slots.Remove(slot); 
                }
                onItemChangedCallback?.Invoke(); // Avisar a la UI
                return true; 
            }
        }
        return false; 
    }
}