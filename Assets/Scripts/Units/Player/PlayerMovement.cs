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
    // for when an external source needs to stop the player from moving
    public bool playerFreezeOverride;
    public bool moving;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Color oldColor = Gizmos.color;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stats.interactRange);

        Gizmos.color = oldColor;
    }
#endif

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
        if (!playerFreezeOverride)
        {
            canMove = !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Roll");
        }
        AnimateMove();

        if (playerFreezeOverride || !canMove || !moving)
        {
            FlipSprite(GetProjectedMousePos() - (Vector2)transform.position, transform);
        }
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
        if (!canMove || playerFreezeOverride || rb.velocity.magnitude > 0.4f) // knockback/roll velocity check
            return;

        // move the player based on input and velocity
        FlipSprite(positionDelta, transform);
        rb.MovePosition(rb.position + positionDelta);
    }

    public void Knockback(Vector2 force)
    {
        canMove = false;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void Move(CallbackContext context)
    {
        if (playerFreezeOverride || !canMove)
        {
            moving = false;
            return;
        }

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

    public void Slide(CallbackContext context)
    {
        // TODO
    }

    // TODO: maybe move this into separate class?
    public void Interact(CallbackContext context)
    {
        if (context.performed)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, stats.interactRange);
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("NPC"))
                {
                    collider.GetComponent<NPCController>().OnInteract();
                }
            }
        }
    }
}