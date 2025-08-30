using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController2D : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 3f;
    Vector2 motionVector;
    public Vector2 lastMotionVector;
    Animator animator;
    public bool isMoving = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetFloat("lastVertical", -1f);
    }

    private void Update()
    {
        
    }
    void FixedUpdate()
    {
        motionVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        animator.SetFloat("horizontal", motionVector.x);
        animator.SetFloat("vertical", motionVector.y);

        isMoving = motionVector.x != 0 || motionVector.y != 0;
        animator.SetBool("isMoving", isMoving);

        // Para mantener la dirección del último movimiento al detenerse
        if (isMoving)
        {
            lastMotionVector = new Vector2(
                motionVector.x,
                motionVector.y)
                .normalized;

            animator.SetFloat("lastHorizontal", lastMotionVector.x);
            animator.SetFloat("lastVertical", lastMotionVector.y);
        }

        Move();
    }

    private void Move()
    {
        rb.linearVelocity = motionVector * speed;
    }
}
