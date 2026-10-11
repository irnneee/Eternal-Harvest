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
        if (Input.GetMouseButtonDown(0)) 
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            // 1. Comprobar si hay una planta ya existente debajo del ratón
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, plantLayer);
            if (hit != null)
            {
                Plants clickedPlant = hit.GetComponent<Plants>();
                if (clickedPlant != null)
                {
                    clickedPlant.InteractWithPlant(); // Cosechamos la planta
                    return; // Importante: Salimos de la función para no intentar plantar una semilla encima
                }
            }

            // 2. Si no hay planta, continuamos con la lógica normal (intentar plantar)
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