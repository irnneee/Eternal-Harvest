using UnityEngine;

// Esto añade un botón en el menú de Unity para crear objetos fácilmente
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon; // Para cuando hagamos la interfaz visual
    public int maxStack = 99; // Límite de objetos por casilla
}