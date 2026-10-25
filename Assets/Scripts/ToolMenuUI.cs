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
    public RectTransform menuRect;   // El RectTransform del contenedor de este menú
    public RectTransform toolCursor; // El sprite que marca la herramienta seleccionada

    [Header("Animación del Menú")]
    public Vector3 shownPosition = new Vector3(80f, 0f, 0f);
    public Vector3 hiddenPosition = new Vector3(-60f, 0f, 0f);
    public float slideSpeed = 10f; // Velocidad del deslizamiento (mayor número = más rápido)

    [Header("Slots de Herramientas (0: Manos, 1: Azada, 2: Regadera)")]
    public GameObject[] toolSlotsUI;

    private int selectedToolIndex = 0;
    public bool isSelectingTool = false;

    void Start()
    {
        // Si no arrastraste el RectTransform manualmente, intenta coger el de este mismo objeto
        if (menuRect == null)
        {
            menuRect = GetComponent<RectTransform>();
        }

        // Empezamos con el menú oculto en (-60, 0, 0) y con las manos seleccionadas
        if (menuRect != null)
        {
            menuRect.anchoredPosition3D = hiddenPosition;
        }

        SelectTool(0);
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

            // Confirmar selección al hacer clic izquierdo
            if (Input.GetMouseButtonDown(0))
            {
                // Cerramos en el siguiente frame para evitar que el clic interactúe con el mundo
                Invoke(nameof(CloseSelectionMode), 0.05f);
            }
        }

        // 3. Mover el menú de forma fluida hacia la posición objetivo
        if (menuRect != null)
        {
            Vector3 targetPos = isSelectingTool ? shownPosition : hiddenPosition;
            menuRect.anchoredPosition3D = Vector3.Lerp(
                menuRect.anchoredPosition3D, 
                targetPos, 
                Time.deltaTime * slideSpeed
            );
        }

        // 4. Actualizar la posición del cursor DESPUÉS de mover el menú para que se deslice con él
        if (selectedToolIndex < toolSlotsUI.Length && toolCursor != null)
        {
            toolCursor.position = toolSlotsUI[selectedToolIndex].transform.position;
        }
    }

    // Función para los eventos OnClick() de los botones de cada herramienta en la UI
    public void SelectToolFromUI(int index)
    {
        if (!isSelectingTool) return;

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
    }

    private void CloseSelectionMode()
    {
        ToggleSelectionMode(false);
    }
}