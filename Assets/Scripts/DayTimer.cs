using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;
using System.Collections;
using TMPro;

public class DayTimer : MonoBehaviour
{
    public static DayTimer Instance { get; private set; }
    public static DayTimer instance => Instance; // Compatibilidad por si se accede como instance o Instance

    [Header("Day & UI Settings")]
    [Tooltip("Día actual del juego (empieza en 1)")]
    public int currentDay = 1;
    public int hectomero = 1; // Hectómero actual

    [Header("Energy Settings")]
    [Tooltip("Duración máxima de la barra en cada fragmento")]
    [FormerlySerializedAs("maxFatigue")]
    public float maxEnergy = 100f;
    [Tooltip("Velocidad a la que baja la barra por segundo")]
    public float drainSpeed = 10f;
    private float currentEnergy;

    [Header("Energy Modification")]
    [Tooltip("Cantidad de energía que se reduce al regar una planta")]
    public float energyPerWatering = 50f;
    [Tooltip("Cantidad de energía que se reduce al plantar una semilla")]
    public float energyPerPlanting = 10f;
    [Tooltip("Cantidad de energía que se reduce al cosechar una planta")]
    public float energyPerHarvesting = 20f;
    [Tooltip("Cantidad de energía que se reduce al arar el suelo")]
    public float energyPerTilling = 80f;

    [Header("HUD References (Drag from Hierarchy)")]
    [Tooltip("Arrastra aquí el objeto de la barra verde")]
    [FormerlySerializedAs("fatigueBarObject")]
    [SerializeField] private GameObject energyBarObject;

    [Tooltip("Arrastra aquí los 6 objetos de las agujas rojas en orden (de la I a la VI)")]
    [SerializeField] private GameObject[] clockHands = new GameObject[6];

    [Header("Bar Reduction Mode")]
    [Tooltip("Si está activado, recorta la barra sin deformar el pixel art. Si se desactiva, encoge su escala.")]
    [SerializeField] private bool useFillInsteadOfScale = true;

    [Header("Text Display")]
    [Tooltip("Arrastra aquí el TMP_Text que mostrará el día actual")]
    [SerializeField] private TMP_Text dayText;

    private int currentFragment = 0; // 0 = Hand I, 1 = Hand II ... 5 = Hand VI
    private Image barImage;
    private RectTransform barRect;
    private Vector3 initialBarScale;

    // Eventos para el resto del juego (plantas, fin del ciclo, etc.)
    public static event Action<int> OnFragmentChanged;
    public static event Action OnDayPassed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentEnergy = maxEnergy;
        currentFragment = 0;

        if (energyBarObject != null)
        {
            barImage = energyBarObject.GetComponent<Image>();
            barRect = energyBarObject.GetComponent<RectTransform>();
            initialBarScale = energyBarObject.transform.localScale;

            // Si tiene componente Image y usamos recorte, fijamos el origen a la derecha
            if (barImage != null && useFillInsteadOfScale)
            {
                barImage.type = Image.Type.Filled;
                barImage.fillMethod = Image.FillMethod.Horizontal;
                barImage.fillOrigin = (int)Image.OriginHorizontal.Right; // Deja fijo el lado derecho (junto al reloj)
            }
            // Si se encoge por escala, fijamos el pivote a la derecha (X = 1)
            else if (barRect != null && !useFillInsteadOfScale)
            {
                SetPivotRight(barRect);
            }
        }

        UpdateHandVisuals();
        UpdateBarVisuals();
    }

    void Update()
    {
        // Reducimos la energía con el tiempo
        currentEnergy -= Time.deltaTime * drainSpeed;
        UpdateBarVisuals();

        // Cuando la barra llega a 0, pasamos al siguiente fragmento del reloj
        if (currentEnergy <= 0f)
        {
            AdvanceFragment();
        }
    }

    private void AdvanceFragment()
    {
        currentEnergy = maxEnergy;
        currentFragment++;

        // Si superamos la sexta aguja (índice 5), se completa el ciclo del día
        if (currentFragment >= clockHands.Length)
        {
            Debug.Log("Te duermes en el suelo y pasa un día...");
            PassToNextDay();
            return;
        }

        UpdateHandVisuals();
        UpdateBarVisuals();
        OnFragmentChanged?.Invoke(currentFragment);
    }

    public void PassToNextDay()
    {
        currentDay++;
        if (currentDay > 100) 
        {
            currentDay = 1;
            hectomero++;
        }

        currentEnergy = maxEnergy;
        currentFragment = 0;

        UpdateHandVisuals();
        UpdateBarVisuals();

        OnFragmentChanged?.Invoke(currentFragment);
        OnDayPassed?.Invoke();

        if (dayText != null)
        {
            dayText.text = "Día " + currentDay + " - Hectómero " + hectomero;
        }
        Debug.Log($"[DayTimer] Ahora es el Día {currentDay}. Del hectomero {hectomero}.");
    }

    private void UpdateBarVisuals()
    {
        if (energyBarObject == null) return;

        float percentage = Mathf.Clamp01(currentEnergy / maxEnergy);

        if (barImage != null && useFillInsteadOfScale)
        {
            // Recorta de izquierda a derecha manteniendo fija la parte derecha
            barImage.fillAmount = percentage;
        }
        else
        {
            // Encoge la escala en X manteniendo fijo el pivote derecho
            energyBarObject.transform.localScale = new Vector3(
                initialBarScale.x * percentage,
                initialBarScale.y,
                initialBarScale.z
            );
        }
    }

    private void UpdateHandVisuals()
    {
        // Activa solo la aguja del fragmento actual y oculta las demás
        for (int i = 0; i < clockHands.Length; i++)
        {
            if (clockHands[i] != null)
            {
                clockHands[i].SetActive(i == currentFragment);
            }
        }
    }

    // Ajusta el pivote del RectTransform al extremo derecho sin desplazar la barra en pantalla
    private void SetPivotRight(RectTransform rectTransform)
    {
        Vector2 size = rectTransform.rect.size;
        Vector2 deltaPivot = rectTransform.pivot - new Vector2(1f, 0.5f);
        Vector3 deltaPosition = new Vector3(
            deltaPivot.x * size.x * rectTransform.localScale.x,
            deltaPivot.y * size.y * rectTransform.localScale.y
        );
        rectTransform.pivot = new Vector2(1f, 0.5f);
        rectTransform.localPosition -= deltaPosition;
    }

    // --- ITEM & GAMEPLAY METHODS ---

    // Rellena al máximo la barra de energía sin retroceder de fragmento.
    public void RestoreFullEnergy()
    {
        currentEnergy = maxEnergy;
        UpdateBarVisuals();
        Debug.Log("Energy fully restored at fragment " + (currentFragment + 1));
    }

    // Suma (o resta si es negativo) una cantidad de energía sin volver a un fragmento anterior.
    public void ModifyEnergy(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
        UpdateBarVisuals();

        if (currentEnergy <= 0f)
        {
            AdvanceFragment();
        }
    }
}