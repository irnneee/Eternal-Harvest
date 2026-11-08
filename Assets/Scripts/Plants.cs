using UnityEngine;

[System.Serializable]
public struct GrowthStage
{
    public Sprite sprite;
    public int dayToReach; // El día exacto en el que aparece este sprite
}

public class Plants : MonoBehaviour
{
    [Header("Configuración de Crecimiento")]
    public int days = 0;
    public int daysToMature = 3; // Días necesarios para dar fruto
    public bool destroyOnHarvest = false; 
    public int daysToRegrow = 2; // Días que tarda en volver a dar fruto tras la 1ª cosecha

    bool isRegrowing = false; // Controla si está en fase de rebrote
    
    [Header("Configuración de Interacción")]
    public float interactionRadius = 1.5f; // Distancia máxima para recoger
    public int recolectedAmount = 1; // Cantidad de fruto que se obtiene al cosechar

    [Header("Estado de Riego")]
    public bool requiresWaterToGrow = true; // Desmarca esto en el Inspector si quieres que crezcan sin regar
    public bool isWatered = false;
    
    bool isHarvestable = false;

    public ItemData fruitToGive;
    
    public GrowthStage[] growthStages;

    SpriteRenderer spriteRenderer;

    // Nos suscribimos cuando el objeto se activa
    void OnEnable()
    {
        DayTimer.OnDayPassed += changeDay; 
    }

    // Nos desuscribimos cuando el objeto se desactiva o destruye
    void OnDisable()
    {
        DayTimer.OnDayPassed -= changeDay;
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        CheckMaturity(); 
    }

    void changeDay()
    {
        if (!isHarvestable) 
        {
            // Si requiere agua y no fue regada hoy, no avanza su crecimiento
            if (requiresWaterToGrow && !isWatered)
            {
                Debug.Log(this.name + " no creció hoy porque le faltó agua.");
                return;
            }

            days++;
            isWatered = false; // Se seca al pasar de día para tener que volver a regarla
            CheckMaturity();
            Debug.Log(this.name + "- Days: " + days);
        }
    }

    // Nueva función llamada desde PlayerInteraction al usar la regadera
    public void WaterPlant()
    {
        if (!isWatered)
        {
            isWatered = true;
            Debug.Log(this.name + " ha sido regada.");
        }
        else
        {
            Debug.Log(this.name + " ya estaba regada hoy.");
        }
    }

    void CheckMaturity()
    {
        if (!isRegrowing)
        {
            // CRECIMIENTO INICIAL: Usa el struct normal por días
            for (int i = 0; i < growthStages.Length; i++)
            {
                if (days >= growthStages[i].dayToReach)
                {
                    spriteRenderer.sprite = growthStages[i].sprite;
                }
            }

            if (days >= daysToMature)
            {
                isHarvestable = true;
            }
        }
        else
        {
            // FASE DE REBROTE: Ya es una planta adulta esperando dar fruto otra vez
            if (days >= daysToRegrow)
            {
                isHarvestable = true;
                // Ponemos el último sprite de la lista (con fruto)
                spriteRenderer.sprite = growthStages[growthStages.Length - 1].sprite;
            }
            else
            {
                // Mientras espera (ej. durante esos 2, 3 o 5 días), 
                // forzamos que se vea siempre el penúltimo sprite (arbusto sin fruto)
                spriteRenderer.sprite = growthStages[growthStages.Length - 2].sprite;
            }
        }
    }

    // Función que se llama desde PlayerInteraction
    public void InteractWithPlant()
    {
        CheckMaturity();

        if (!isHarvestable) 
        {
            Debug.Log("Esta planta aún no está lista para ser cosechada.");
            return;
        }

        GameObject player = GameManager.instance.player;
        if (player == null) 
        {
            Debug.Log("No se encontró al jugador.");
            return;
        }

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= interactionRadius)
        {
            Harvest(player);
        }
        else
        {
            Debug.Log("Estás demasiado lejos para cosechar.");
        }
    }

    void Harvest(GameObject player)
    {
        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory != null)
        {
            // Intentamos añadir el objeto. Si devuelve true, se añadió bien.
            bool wasAdded = playerInventory.AddItem(fruitToGive, recolectedAmount); 
            
            if (wasAdded)
            {
                if (destroyOnHarvest)
                {
                    Destroy(gameObject);
                }
                else
                {
                    days = 0; 
                    isHarvestable = false;
                    isRegrowing = true; // Entramos en bucle de rebrote
                    CheckMaturity(); 
                }
            }
            else
            {
                Debug.Log("No puedes cosechar, no tienes espacio.");
            }
        }
    }
}