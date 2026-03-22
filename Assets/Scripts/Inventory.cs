using System.Collections.Generic;
using UnityEngine;

// Esta clase representa una casilla individual
[System.Serializable] // Permite que lo veamos en el Inspector de Unity
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
    public int maxSlots = 6; // Límite de casillas diferentes
    
    // Aquí guardamos nuestro inventario real
    public List<InventorySlot> slots = new List<InventorySlot>();

    public bool AddItem(ItemData itemToAdd, int amountToAdd)
    {
        // 1. Comprobar si ya tenemos este objeto en alguna casilla para apilarlo (stack)
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == itemToAdd && slot.amount < slot.item.maxStack)
            {
                slot.amount += amountToAdd;
                Debug.Log($"Apilado: {slot.amount} {itemToAdd.itemName}s en esta casilla.");
                return true; // Se añadió con éxito
            }
        }

        // 2. Si no lo tenemos (o la casilla estaba llena), buscamos una casilla vacía
        if (slots.Count < maxSlots)
        {
            slots.Add(new InventorySlot(itemToAdd, amountToAdd));
            Debug.Log($"Nueva casilla ocupada con: {itemToAdd.itemName}. Casillas usadas: {slots.Count}/{maxSlots}");
            return true; // Se añadió con éxito
        }

        // 3. Si llegamos aquí, el inventario está lleno
        Debug.Log("¡El inventario está lleno!");
        return false; 
    }
}