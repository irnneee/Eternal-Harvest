using UnityEngine;

/// <summary>
/// Coloca este script en el GameObject de la choza (o en el hijo donde tengas el Box Collider 2D con "Is Trigger").
/// Al entrar el jugador en el trigger, se pasará al siguiente día, restaurando la barra de cansancio al máximo
/// y reiniciando el reloj al primer fragmento.
/// </summary>
public class HutSleepTrigger : MonoBehaviour
{
    [Header("Configuración de Choza")]
    [Tooltip("Tiempo de espera en segundos para evitar que se active varias veces por accidente")]
    [SerializeField] private float cooldown = 5f;

    [Tooltip("Punto opcional al que mover al jugador al despertar (ej: en la puerta de la choza). Si está vacío, el jugador se queda donde está.")]
    [SerializeField] private Transform wakeUpPoint;

    [Header("Audio (Opcional)")]
    [Tooltip("Efecto de sonido opcional al ir a dormir")]
    [SerializeField] private AudioClip sleepSound;
    private AudioSource audioSource;

    private float lastTriggerTime = -999f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && sleepSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Para colisiones 2D (recomendado en Unity 2D con Rigidbody2D)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        TrySleep(collision.gameObject);
    }

    // Por si se utilizó un BoxCollider 3D por error
    private void OnTriggerEnter(Collider other)
    {
        TrySleep(other.gameObject);
    }

    private void TrySleep(GameObject target)
    {
        // Comprobamos si el objeto que entra es el jugador
        if (!IsPlayer(target)) return;

        // Evitamos que se active varias veces seguidas
        if (Time.time - lastTriggerTime < cooldown) return;
        lastTriggerTime = Time.time;

        // Sonido de dormir
        if (audioSource != null && sleepSound != null)
        {
            audioSource.PlayOneShot(sleepSound);
        }

        // Buscamos el Timer y avanzamos de día
        Timer timer = Timer.Instance;
        if (timer == null)
        {
            timer = Object.FindFirstObjectByType<Timer>();
        }

        if (timer != null)
        {
            timer.PassToNextDay();
            Debug.Log("<color=cyan>[Choza]</color> El jugador ha dormido. ¡Nuevo día iniciado!");
        }
        else
        {
            Debug.LogWarning("[Choza] No se encontró ningún componente Timer en la escena.");
        }

        // Si hay un punto de reaparición/despertar configurado, movemos al jugador allí
        if (wakeUpPoint != null)
        {
            target.transform.position = wakeUpPoint.position;
        }
    }

    private bool IsPlayer(GameObject obj)
    {
        if (obj.CompareTag("Player")) return true;
        if (obj.GetComponent<CharacterController2D>() != null) return true;
        if (obj.GetComponentInParent<CharacterController2D>() != null) return true;
        if (GameManager.instance != null && GameManager.instance.player == obj) return true;
        return false;
    }
}

