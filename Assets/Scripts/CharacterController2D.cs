using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 3f;
    Vector2 motionVector;
    Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        motionVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        animator.SetFloat("horizontal", motionVector.x);
        animator.SetFloat("vertical", motionVector.y);
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rb.linearVelocity = motionVector * speed;
    }
}
