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
    public LayerMask plantLayer; 

    void Update()
    {
        // Solo el clic izquierdo para interactuar. Si equippedItem es null, simplemente no hará nada.
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

        if (cultivableTilemap.HasTile(cellPosition))
        {
            Vector3 spawnPosition = cultivableTilemap.GetCellCenterWorld(cellPosition);

            Collider2D hit = Physics2D.OverlapPoint(spawnPosition, plantLayer);
            if (hit != null)
            {
                Debug.Log("Ya hay algo plantado en esta casilla.");
                return;
            }

            // 3. Gastar semilla e instanciar
            ItemData seedToPlant = equippedItem; // Guardamos la referencia a salvo
            
            if (inventory.RemoveItem(seedToPlant))
            {
                // Usamos la referencia guardada en lugar de equippedItem
                Instantiate(seedToPlant.actionPrefab, spawnPosition, Quaternion.identity);
            }
        }
    }
}