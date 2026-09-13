using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para el texto

public class InventoryUI : MonoBehaviour
{
    [Header("Referencias")]
    public Inventory inventory;
    public PlayerInteraction playerInteraction;
    public RectTransform cursor;
    
    [Header("Slots Visuales")]
    public GameObject[] slotsUI; // Arrastra aquí tus 10 objetos Slot_1 al Slot_0

    private int selectedSlotIndex = 0;

    void Start()
    {
        // Nos suscribimos para escuchar cada vez que el inventario cambie
        inventory.onItemChangedCallback += UpdateUI;
        
        SelectSlot(0); // Empezamos con el primer slot seleccionado
        UpdateUI();
    }

    void Update()
    {
        // Detectar teclas numéricas para mover el cursor
        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SelectSlot(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SelectSlot(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SelectSlot(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SelectSlot(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SelectSlot(8);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SelectSlot(9);
    }

    void SelectSlot(int index)
    {
        selectedSlotIndex = index;

        // Mover el cursor físicamente
        if (index < slotsUI.Length)
        {
            cursor.position = slotsUI[index].transform.position;
        }

        UpdateEquippedItem();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            // Buscamos a los hijos por su nombre exacto
            Image icon = slotsUI[i].transform.Find("Icono").GetComponent<Image>();
            TextMeshProUGUI amountText = slotsUI[i].transform.Find("TextoCantidad").GetComponent<TextMeshProUGUI>();

            if (i < inventory.slots.Count)
            {
                // Si hay objeto, lo mostramos
                icon.sprite = inventory.slots[i].item.icon;
                icon.color = Color.white; // Opacidad al 100%
                amountText.text = inventory.slots[i].amount.ToString();
                amountText.enabled = true;
            }
            else
            {
                // Si está vacío, lo ocultamos
                icon.sprite = null;
                icon.color = Color.clear; // Transparente
                amountText.enabled = false;
            }
        }
        UpdateEquippedItem(); 
    }

    void UpdateEquippedItem()
    {
        // Actualizamos las manos del jugador según la casilla seleccionada
        if (selectedSlotIndex < inventory.slots.Count)
        {
            playerInteraction.equippedItem = inventory.slots[selectedSlotIndex].item;
        }
        else
        {
            playerInteraction.equippedItem = null; // Manos vacías
        }
    }
}