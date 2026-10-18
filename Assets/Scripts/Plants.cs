using UnityEngine;

public class Plants : MonoBehaviour
{
    [Header("Configuración de Crecimiento")]
    public int days = 0;
    public int daysToMature = 3; // Días necesarios para dar fruto
    
    [Header("Configuración de Interacción")]
    public float interactionRadius = 1.5f; // Distancia máxima para recoger

    [Header("Estado de Riego")]
    public bool requiresWaterToGrow = true; // Desmarca esto en el Inspector si quieres que crezcan sin regar
    public bool isWatered = false;
    
    bool isHarvestable = false;

    public ItemData fruitToGive;
    Animator animator;

    // Nos suscribimos cuando el objeto se activa
    void OnEnable()
    {
        Timer.OnDayPassed += changeDay; 
    }

    // Nos desuscribimos cuando el objeto se desactiva o destruye
    void OnDisable()
    {
        Timer.OnDayPassed -= changeDay;
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        CheckMaturity(); // Verificar estado inicial
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
        // Actualizamos la animación
        animator.SetInteger("day", days);

        // Si los días actuales superan o igualan los necesarios, se puede cosechar
        if (days >= daysToMature)
        {
            isHarvestable = true;
            Debug.Log("¡Una planta ha madurado!");
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
            bool wasAdded = playerInventory.AddItem(fruitToGive, 1); 
            
            if (wasAdded)
            {
                // Solo reiniciamos la planta si el jugador tenía espacio para recogerlo
                days = 0; 
                isHarvestable = false;
                animator.SetInteger("day", days); 
                Debug.Log("Planta cosechada y reiniciada.");
            }
            else
            {
                Debug.Log("No puedes cosechar, no tienes espacio.");
            }
        }
    }
}