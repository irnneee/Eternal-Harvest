using UnityEngine;

public enum ItemType { Material, Consumable, Seed } // Genérico y ampliable

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 99;
    
    public ItemType type; 
    public GameObject actionPrefab; // Para objetos que tengan una acción específica.
}