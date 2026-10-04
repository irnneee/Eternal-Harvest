using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    [Header("Fatigue Settings")]
    [Tooltip("Duración máxima de la barra en cada fragmento")]
    public float maxFatigue = 100f;
    [Tooltip("Velocidad a la que baja la barra por segundo")]
    public float drainSpeed = 10f;
    private float currentFatigue;

    [Header("HUD References (Drag from Hierarchy)")]
    [Tooltip("Arrastra aquí el objeto de la barra verde")]
    [SerializeField] private GameObject fatigueBarObject;

    [Tooltip("Arrastra aquí los 6 objetos de las agujas rojas en orden (de la I a la VI)")]
    [SerializeField] private GameObject[] clockHands = new GameObject[6];

    [Header("Bar Reduction Mode")]
    [Tooltip("Si está activado, recorta la barra sin deformar el pixel art. Si se desactiva, encoge su escala.")]
    [SerializeField] private bool useFillInsteadOfScale = true;

    private int currentFragment = 0; // 0 = Hand I, 1 = Hand II ... 5 = Hand VI
    private Image barImage;
    private RectTransform barRect;
    private Vector3 initialBarScale;

    // Eventos para el resto del juego (plantas, fin del ciclo, etc.)
    public static event Action<int> OnFragmentChanged;
    public static event Action OnDayPassed;

    void Start()
    {
        currentFatigue = maxFatigue;
        currentFragment = 0;

        if (fatigueBarObject != null)
        {
            barImage = fatigueBarObject.GetComponent<Image>();
            barRect = fatigueBarObject.GetComponent<RectTransform>();
            initialBarScale = fatigueBarObject.transform.localScale;

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
        // Reducimos el cansancio con el tiempo
        currentFatigue -= Time.deltaTime * drainSpeed;
        UpdateBarVisuals();

        // Cuando la barra llega a 0, pasamos al siguiente fragmento del reloj
        if (currentFatigue <= 0f)
        {
            AdvanceFragment();
        }
    }

    private void AdvanceFragment()
    {
        currentFatigue = maxFatigue;
        currentFragment++;

        // Si superamos la sexta aguja (índice 5), se completa el ciclo
        if (currentFragment >= clockHands.Length)
        {
            currentFragment = 0;
            OnDayPassed?.Invoke();
            Debug.Log("All 6 clock fragments completed.");
        }

        UpdateHandVisuals();
        UpdateBarVisuals();
        OnFragmentChanged?.Invoke(currentFragment);
    }

    private void UpdateBarVisuals()
    {
        if (fatigueBarObject == null) return;

        float percentage = Mathf.Clamp01(currentFatigue / maxFatigue);

        if (barImage != null && useFillInsteadOfScale)
        {
            // Recorta de izquierda a derecha manteniendo fija la parte derecha
            barImage.fillAmount = percentage;
        }
        else
        {
            // Encoge la escala en X manteniendo fijo el pivote derecho
            fatigueBarObject.transform.localScale = new Vector3(
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

    // Rellena al máximo la barra de cansancio sin retroceder de fragmento.
    public void RestoreFullFatigue()
    {
        currentFatigue = maxFatigue;
        UpdateBarVisuals();
        Debug.Log("Fatigue fully restored at fragment " + (currentFragment + 1));
    }

    // Suma (o resta si es negativo) una cantidad de cansancio sin volver a un fragmento anterior.
    public void ModifyFatigue(float amount)
    {
        currentFatigue = Mathf.Clamp(currentFatigue + amount, 0f, maxFatigue);
        UpdateBarVisuals();

        if (currentFatigue <= 0f)
        {
            AdvanceFragment();
        }
    }
}