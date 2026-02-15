using UnityEngine;

public class Plants : MonoBehaviour
{
    [Header("Configuración de Crecimiento")]
    public int days = 0;
    public int daysToMature = 3; // Días necesarios para dar fruto
    
    [Header("Configuración de Interacción")]
    public float interactionRadius = 1.5f; // Distancia máxima para recoger
    
    bool isHarvestable = false;
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

    void Start()
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

    // Esta función de Unity se ejecuta automáticamente al hacer clic en el Collider del objeto
    private void OnMouseDown()
    {
        // 1. Verificamos si la planta tiene frutos
        if (!isHarvestable) return;

        // 2. Obtenemos al jugador desde tu GameManager
        GameObject player = GameManager.instance.player;
        if (player == null) return;

        // 3. Calculamos la distancia entre la planta y el jugador
        float distance = Vector2.Distance(transform.position, player.transform.position);

        // 4. Si está lo suficientemente cerca, cosechamos
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
        // A. Añadir al inventario
        // Buscamos el script Inventory en el jugador
        Inventory playerInventory = player.GetComponent<Inventory>();
        if (playerInventory != null)
        {
            playerInventory.AddItem(1); // Sumamos 1 fruto
        }

        // B. Reiniciar la planta
        days = 1; // Madura en dias alternos
        isHarvestable = false;
        animator.SetInteger("day", days); // Actualizamos el gráfico al inicio

        Debug.Log("Planta cosechada y reiniciada.");  
    }
}