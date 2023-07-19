using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 positionDelta;
    public ParticleSystem dust;
    public bool canMove;
    public bool moving;

    // Start is called before the first frame update
    void Start()
    {
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        moving = false;
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        canMove = !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll");
        AnimateMove();
    }

    private void FixedUpdate()
    {
        UpdateMove();
    }

    public void AnimateMove()
    {
        bool isRunning = animator.GetBool("isRunning");

        if (moving && !isRunning)
        {
            animator.SetBool("isRunning", true);
        }
        else if (!moving && isRunning)
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void UpdateMove()
    {
        if (!canMove)
            return;

        // move the player based on input and velocity
        FlipSprite(positionDelta, transform);
        rb.MovePosition(rb.position + positionDelta);
    }

    public void Move(CallbackContext context)
    {
        if (context.canceled)
        {
            positionDelta = Vector2.zero;
            moving = false;
        }

        if (context.performed)
        {
            Vector2 inputMovement = context.ReadValue<Vector2>();
            
            // set moving to true if the player is moving
            moving = Mathf.Abs(inputMovement.x) > 0 || Mathf.Abs(inputMovement.y) > 0;

            // update position delta based on input movement and movement speed (fixedDeltaTime because the movement is updated in FixedUpdate)
            positionDelta = stats.speed * Time.fixedDeltaTime * inputMovement;
        }
    }

    public void Roll(CallbackContext context)
    {
        // TODO
    }
}