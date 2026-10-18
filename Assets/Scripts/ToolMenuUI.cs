using UnityEngine;

public enum ToolType
{
    Hand = 0,        // Manos vacías
    Hoe = 1,         // Azada
    WateringCan = 2  // Regadera
}

public class ToolMenuUI : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInteraction playerInteraction;
    public RectTransform toolCursor; // El sprite que marca la herramienta seleccionada
    public GameObject selectionModeIndicator; // Opcional: Borde o brillo para saber que estás eligiendo con la Q

    [Header("Slots de Herramientas (0: Manos, 1: Azada, 2: Regadera)")]
    public GameObject[] toolSlotsUI;

    private int selectedToolIndex = 0;
    public bool isSelectingTool = false;

    void Start()
    {
        SelectTool(0); // Empezamos con las manos vacías por defecto
        if (selectionModeIndicator != null)
            selectionModeIndicator.SetActive(false);
    }

    void Update()
    {
        // 1. Pulsar Q para abrir/cerrar el modo de selección de herramienta
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleSelectionMode(!isSelectingTool);
        }

        // 2. Si el modo selección está activo, permitir usar la rueda del ratón
        if (isSelectingTool)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll > 0f) // Rueda hacia arriba (anterior)
            {
                int nextIndex = selectedToolIndex - 1;
                if (nextIndex < 0) nextIndex = toolSlotsUI.Length - 1;
                SelectTool(nextIndex);
            }
            else if (scroll < 0f) // Rueda hacia abajo (siguiente)
            {
                int nextIndex = (selectedToolIndex + 1) % toolSlotsUI.Length;
                SelectTool(nextIndex);
            }

            // Confirmar selección al hacer clic izquierdo (si usas la rueda)
            if (Input.GetMouseButtonDown(0))
            {
                // Cerramos el modo selección en el siguiente frame para no disparar interacciones en el mundo
                Invoke(nameof(CloseSelectionMode), 0.05f);
            }
        }

        // Mantener el sprite selector en la posición del slot actual
        if (selectedToolIndex < toolSlotsUI.Length && toolCursor != null)
        {
            toolCursor.position = toolSlotsUI[selectedToolIndex].transform.position;
        }
    }

    // Puedes llamar a esta función desde el evento OnClick() de un componente Button en cada Slot de herramienta
    public void SelectToolFromUI(int index)
    {
        if (!isSelectingTool) return; // Solo permite cambiar si se ha pulsado la Q antes

        SelectTool(index);
        CloseSelectionMode();
    }

    private void SelectTool(int index)
    {
        selectedToolIndex = index;

        if (index < toolSlotsUI.Length && toolCursor != null)
        {
            toolCursor.position = toolSlotsUI[index].transform.position;
        }

        if (playerInteraction != null)
        {
            playerInteraction.currentTool = (ToolType)selectedToolIndex;
            Debug.Log("Herramienta equipada: " + playerInteraction.currentTool);
        }
    }

    private void ToggleSelectionMode(bool state)
    {
        isSelectingTool = state;
        if (playerInteraction != null)
        {
            playerInteraction.isSelectingTool = isSelectingTool;
        }

        if (selectionModeIndicator != null)
        {
            selectionModeIndicator.SetActive(isSelectingTool);
        }
    }

    private void CloseSelectionMode()
    {
        ToggleSelectionMode(false);
    }
}