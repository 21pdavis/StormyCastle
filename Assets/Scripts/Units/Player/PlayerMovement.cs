using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 positionDelta;
    private PlayerAnimationStateController animationController;
    public ParticleSystem dust;
    public bool canMove;
    public bool moving;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
        canMove = true;
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
        animationController = GetComponent<PlayerAnimationStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        UpdateMove();
    }

    private void UpdateAnimation()
    {
        animationController.Move();
        animationController.Roll();
    }

    private void UpdateMove()
    {
        if (!canMove)
            return;

        // move the player based on input and velocity
        characterBody.MovePosition(characterBody.position + positionDelta);
    }

    public void Move(CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();
        FlipSprite(inputMovement, transform);

        // set moving to true if the player is moving
        moving = Mathf.Abs(inputMovement.x) >= 0.01 || Mathf.Abs(inputMovement.y) >= 0.01;

        // update position delta based on input movement and movement speed (fixedDeltaTime because the movement is updated in FixedUpdate)
        positionDelta = inputMovement * velocity * Time.fixedDeltaTime;
    }
}