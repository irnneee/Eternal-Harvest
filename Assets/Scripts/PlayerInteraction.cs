using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap cultivableTilemap; 
    public Inventory inventory; 

    [Header("Configuración")]
    public float interactionRadius = 1.5f;
    public ItemData equippedItem; 
    public LayerMask plantLayer; // Capa para detectar si ya hay una planta

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            TryInteract();
        }
    }

    void TryInteract()
    {
        if (equippedItem == null) return;

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Vector2.Distance(transform.position, mouseWorldPos) > interactionRadius)
        {
            Debug.Log("Demasiado lejos para plantar.");
            return;
        }

        if (equippedItem.type == ItemType.Seed)
        {
            TryPlantSeed(mouseWorldPos);
        }
    }

    void TryPlantSeed(Vector2 actionPosition)
    {
        Vector3Int cellPosition = cultivableTilemap.WorldToCell(actionPosition);

        // 1. Validar si hay tierra cultivable en el Tilemap invisible
        if (cultivableTilemap.HasTile(cellPosition))
        {
            Vector3 spawnPosition = cultivableTilemap.GetCellCenterWorld(cellPosition);

            // 2. Comprobar si ya hay una planta en ese punto exacto
            Collider2D hit = Physics2D.OverlapPoint(spawnPosition, plantLayer);
            if (hit != null)
            {
                Debug.Log("Ya hay algo plantado en esta casilla.");
                return;
            }

            // 3. Gastar semilla e instanciar
            if (inventory.RemoveItem(equippedItem))
            {
                Instantiate(equippedItem.actionPrefab, spawnPosition, Quaternion.identity);
                Debug.Log("¡Semilla plantada!");
            }
            else
            {
                Debug.Log("No te quedan semillas en el inventario.");
            }
        }
        else
        {
            Debug.Log("Aquí no puedes plantar.");
        }
    }
}