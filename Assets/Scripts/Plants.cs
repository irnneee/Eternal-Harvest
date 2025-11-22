using UnityEngine;

public class Plants : MonoBehaviour
{
    public int days = 0;
    Animator animator;

    // Nos suscribimos cuando el objeto se activa
    void OnEnable()
    {
        Timer.OnDayPassed += changeDay; // "Cuando Timer diga OnDayPassed, ejecuta mi función Grow"
    }

    // Nos desuscribimos cuando el objeto se desactiva o destruye
    // (Es muy importante para evitar errores de memoria)
    void OnDisable()
    {
        Timer.OnDayPassed -= changeDay;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("day", days);
    }
    void Update()
    {
        
    }

    void changeDay()
    {
        days++;
        animator.SetInteger("day", days);
        Debug.Log(this + "- Days: " + days);
    }
}
