using UnityEngine;

public class Plants : MonoBehaviour
{
    public int days = 0;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("day", days);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            changeDay();
        }
    }

    void changeDay()
    {
        days++;
        animator.SetInteger("day", days);
        Debug.Log("Days: " + days);
    }
}
