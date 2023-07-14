using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 positionDelta;
    private PlayerAnimationStateController animationController;
    public bool moving;

    // Start is called before the first frame update
    void Start()
    {
        moving = false;
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
        // move the player based on input and velocity
        characterBody.MovePosition(characterBody.position + positionDelta);
    }

    private void FlipSprite(Vector2 inputMovement)
    {
        // flip sprite to face left or right depending on input x value
        if (inputMovement.x >= 0.01)
        {
            // change scale of player to face right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputMovement.x <= -0.01)
        {
            // change scale of player to face left
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void Move(CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();
        FlipSprite(inputMovement);

        // set moving to true if the player is moving
        moving = Mathf.Abs(inputMovement.x) >= 0.01 || Mathf.Abs(inputMovement.y) >= 0.01;

        // update position delta based on input movement and movement speed (fixedDeltaTime because the movement is updated in FixedUpdate)
        positionDelta = inputMovement * velocity * Time.fixedDeltaTime;
    }
}