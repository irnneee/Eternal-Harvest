using UnityEngine;

public class Plants : MonoBehaviour
{
    [Header("Configuración de Crecimiento")]
    public int days = 0;
    public int daysToMature = 3; // Días necesarios para dar fruto
    
    [Header("Configuración de Interacción")]
    public float interactionRadius = 1.5f; // Distancia máxima para recoger
    
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
        // Solo crece si no está ya lista para cosechar (opcional, depende de tu diseño)
        if (!isHarvestable) 
        {
            days++;
            CheckMaturity();
            Debug.Log(this.name + "- Days: " + days);
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