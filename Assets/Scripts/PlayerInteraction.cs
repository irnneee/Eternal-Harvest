using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public Tilemap groundTilemap;     // Tilemap del suelo base 
    public Tilemap cultivableTilemap; // Tilemap superpuesto para tierra arada
    public Tilemap sueloArable;       // Tilemap invisible que define dónde se puede usar la azada
    public TileBase tilledTile;       // TilledRuleTile
    public Inventory inventory; 

    [Header("Colores de Riego")]
    public Color dryColor = Color.white; // Color normal del tile labrado
    public Color wateredColor = new Color(0.7f, 0.7f, 0.7f, 1f); // Tono más oscuro al regar

    [Header("Configuración de Interacción")]
    public float interactionRadius = 1.5f;
    public LayerMask plantLayer; 

    [Header("Estado de Mano y Herramienta")]
    public ItemData equippedItem; 
    public ToolType currentTool = ToolType.Hand; 
    [HideInInspector] public bool isSelectingTool = false;

    // Registro de las casillas que han sido regadas hoy
    private HashSet<Vector3Int> wateredCells = new HashSet<Vector3Int>();

    void OnEnable()
    {
        DayTimer.OnDayPassed += ResetWateredTiles;
    }

    void OnDisable()
    {
        DayTimer.OnDayPassed -= ResetWateredTiles;
    }

    void Update()
    {
        if (isSelectingTool) return;
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        if (Input.GetMouseButtonDown(0)) 
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(transform.position, mouseWorldPos) > interactionRadius)
            {
                Debug.Log("Demasiado lejos para interactuar.");
                return;
            }

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

        if (equippedItem != null && equippedItem.type == ItemType.Seed)
        {
            TryPlantSeed(mouseWorldPos);
        }
    }

    // 2. AZADA: Superponer el tile de tierra labrada
    void UseHoe(Vector2 mouseWorldPos)
    {
        Vector3Int cellPosition = cultivableTilemap.WorldToCell(mouseWorldPos);

        // 1. Si ya está arada la casilla en cultivableTilemap, no hacemos nada
        if (cultivableTilemap.HasTile(cellPosition))
        {
            Debug.Log("Esta tierra ya está arada.");
            return;
        }

        // 2. Comprobamos en el tilemap invisible 'sueloArable' si esta zona permite usar la azada
        if (sueloArable != null && !sueloArable.HasTile(sueloArable.WorldToCell(mouseWorldPos)))
        {
            Debug.Log("No puedes usar la azada aquí.");
            return;
        }

        // 3. Pintamos el Rule Tile en sueloCultivable
        if (tilledTile != null)
        {
            cultivableTilemap.SetTile(cellPosition, tilledTile);
            cultivableTilemap.SetTileFlags(cellPosition, TileFlags.None);
            cultivableTilemap.SetColor(cellPosition, dryColor);

            // Reaplicamos el color oscuro a las casillas regadas por si el Rule Tile actualizó un vecino regado
            RefreshWateredColors();

            DayTimer.Instance.ModifyEnergy(-DayTimer.Instance.energyPerTilling); // Reduce energía al arar

            Debug.Log("Tierra arada con la azada.");
        }
    }

    void RefreshWateredColors()
    {
        foreach (Vector3Int wateredPos in wateredCells)
        {
            if (cultivableTilemap.HasTile(wateredPos))
            {
                cultivableTilemap.SetTileFlags(wateredPos, TileFlags.None);
                cultivableTilemap.SetColor(wateredPos, wateredColor);
            }
        }
    }

    // 3. REGADERA: Oscurecer el tile labrado y regar la planta (si ya hay una)
    void UseWateringCan(Vector2 mouseWorldPos)
    {
        Vector3Int cellPosition = cultivableTilemap.WorldToCell(mouseWorldPos);

        // Comprobamos si la casilla está labrada
        if (!cultivableTilemap.HasTile(cellPosition))
        {
            Debug.Log("Primero debes labrar la tierra antes de regarla.");
            return;
        }

        // Si no estaba regada, la oscurecemos y la guardamos en el registro
        if (!wateredCells.Contains(cellPosition))
        {
            wateredCells.Add(cellPosition);
            cultivableTilemap.SetTileFlags(cellPosition, TileFlags.None);
            cultivableTilemap.SetColor(cellPosition, wateredColor);
            DayTimer.Instance.ModifyEnergy(-DayTimer.Instance.energyPerWatering); // Reduce energía al regar
            Debug.Log("Tierra regada.");
        }

        // Si ya hay una planta en esa casilla, le avisamos de que ha sido regada
        Vector3 cellCenter = cultivableTilemap.GetCellCenterWorld(cellPosition);
        Collider2D hit = Physics2D.OverlapPoint(cellCenter, plantLayer);
        if (hit != null)
        {
            Plants plant = hit.GetComponent<Plants>();
            if (plant != null)
            {
                plant.WaterPlant();
            }
        }
    }

    // Plantar semilla comprobando si la tierra ya estaba regada previamente
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
                GameObject newPlantObj = Instantiate(seedToPlant.actionPrefab, spawnPosition, Quaternion.identity);

                DayTimer.Instance.ModifyEnergy(-DayTimer.Instance.energyPerPlanting); // Reduce energía al plantar

                // Si el jugador regó la tierra ANTES de plantar la semilla, la planta nace ya regada
                if (wateredCells.Contains(cellPosition))
                {
                    Plants newPlant = newPlantObj.GetComponent<Plants>();
                    if (newPlant != null)
                    {
                        newPlant.WaterPlant();
                    }
                }
            }
        }
        else
        {
            Debug.Log("Primero debes arar la tierra con la azada.");
        }
    }

    // Se ejecuta automáticamente cuando cambia el día en DayTimer.cs
    void ResetWateredTiles()
    {
        foreach (Vector3Int cellPos in wateredCells)
        {
            if (cultivableTilemap.HasTile(cellPos))
            {
                cultivableTilemap.SetTileFlags(cellPos, TileFlags.None);
                cultivableTilemap.SetColor(cellPos, dryColor);
            }
        }
        wateredCells.Clear();
    }
}