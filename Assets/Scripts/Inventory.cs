using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Más adelante usaremos diccionarios para manejar distintos tipos de ítems.
    public int fruits = 0;

    public void AddItem(int amount)
    {
        fruits += amount;
        Debug.Log("Inventario actualizado. Frutos totales: " + fruits);
    }
}