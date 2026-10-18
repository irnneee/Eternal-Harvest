using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems; // Necesario para detectar si hacemos clic en la UI

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap groundTilemap;     // Tilemap del suelo base (césped/tierra sin arar)
    public Tilemap cultivableTilemap; // Tilemap donde se pintan los cuadros arados/cultivables
    public TileBase tilledTile;       // El Tile de tierra arada que pondrá la azada
    public Inventory inventory; 

    [Header("Configuración de Interacción")]
    public float interactionRadius = 1.5f;
    public LayerMask plantLayer; 

    [Header("Estado de Mano y Herramienta")]
    public ItemData equippedItem; // Objeto del inventario principal (semillas, frutos, etc.)
    public ToolType currentTool = ToolType.Hand; // Herramienta activa
    [HideInInspector] public bool isSelectingTool = false;

    void Update()
    {
        // Si estamos en el menú de la Q o el ratón está sobre la UI, no interactuamos con el entorno
        if (isSelectingTool) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0)) 
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Comprobación general de distancia al jugador
            if (Vector2.Distance(transform.position, mouseWorldPos) > interactionRadius)
            {
                Debug.Log("Demasiado lejos para interactuar.");
                return;
            }

            // Ejecutar acción según la herramienta equipada
            switch (currentTool)
            {
                case ToolType.Hand:
                    UseEmptyHands(mouseWorldPos);
                    break;

                case ToolType.Hoe:
                    UseHoe(mouseWorldPos);
                    break;

                case ToolType.WateringCan:
                    UseWateringCan(mouseWorldPos);
                    break;
            }
        }
    }

    // 1. MANOS VACÍAS: Cosechar plantas o plantar semillas del inventario principal
    void UseEmptyHands(Vector2 mouseWorldPos)
    {
        // Comprobar si hay una planta debajo del ratón para cosecharla
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, plantLayer);
        if (hit != null)
        {
            Plants clickedPlant = hit.GetComponent<Plants>();
            if (clickedPlant != null)
            {
                clickedPlant.InteractWithPlant();
                return; 
            }
        }

        // Si no hay planta y tenemos una semilla seleccionada en el inventario normal, plantamos
        if (equippedItem != null && equippedItem.type == ItemType.Seed)
        {
            TryPlantSeed(mouseWorldPos);
        }
    }

    // 2. AZADA: Arar la tierra para volverla cultivable
    void UseHoe(Vector2 mouseWorldPos)
    {
        Vector3Int cellPosition = cultivableTilemap.WorldToCell(mouseWorldPos);

        // Si ya está arada la casilla, no hacemos nada
        if (cultivableTilemap.HasTile(cellPosition))
        {
            Debug.Log("Esta tierra ya está arada.");
            return;
        }

        // Opcional: Si tienes un groundTilemap para validar que sea suelo válido antes de arar
        if (groundTilemap != null && !groundTilemap.HasTile(groundTilemap.WorldToCell(mouseWorldPos)))
        {
            Debug.Log("No puedes usar la azada aquí.");
            return;
        }

        if (tilledTile != null)
        {
            cultivableTilemap.SetTile(cellPosition, tilledTile);
            Debug.Log("Tierra arada con la azada.");
        }
        else
        {
            Debug.LogWarning("Falta asignar el 'tilledTile' en el Inspector de PlayerInteraction.");
        }
    }

    // 3. REGADERA: Regar una planta existente
    void UseWateringCan(Vector2 mouseWorldPos)
    {
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, plantLayer);
        if (hit != null)
        {
            Plants clickedPlant = hit.GetComponent<Plants>();
            if (clickedPlant != null)
            {
                clickedPlant.WaterPlant();
                return;
            }
        }

        Debug.Log("Usaste la regadera, pero no hay ninguna planta en esta casilla.");
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

            ItemData seedToPlant = equippedItem;
            
            if (inventory.RemoveItem(seedToPlant))
            {
                Instantiate(seedToPlant.actionPrefab, spawnPosition, Quaternion.identity);
            }
        }
        else
        {
            Debug.Log("Primero debes arar la tierra con la azada.");
        }
    }
}