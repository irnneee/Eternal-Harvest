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

    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    void Awake()
    {
        // Rellenar el inventario con huecos vacíos fijos al iniciar
        while (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(null, 0));
        }
    }

    public bool AddItem(ItemData itemToAdd, int amountToAdd)
    {
        // 1. Intentar apilar en una casilla que ya tenga este objeto
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToAdd && slot.amount < slot.item.maxStack)
            {
                slot.amount += amountToAdd;
                onItemChangedCallback?.Invoke();
                return true; 
            }
        }

        // 2. Buscar la PRIMERA casilla vacía disponible y ocuparla
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == null)
            {
                slot.item = itemToAdd;
                slot.amount = amountToAdd;
                onItemChangedCallback?.Invoke();
                return true;
            }
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
                    slot.item = null; // Limpiamos la casilla, NO la borramos de la lista
                }
                onItemChangedCallback?.Invoke(); 
                return true; 
            }
        }
        return false; 
    }
}