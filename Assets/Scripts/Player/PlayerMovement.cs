using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody2D characterBody;
    private Vector2 velocity;
    //private Vector2 inputMovement;
    private Vector2 positionDelta;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        // move the player based on input and velocity
        characterBody.MovePosition(characterBody.position + positionDelta);
    }

    public void Move(CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();

        if (inputMovement.x > 0)
        {
            // change scale of player to face right
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (inputMovement.x < 0)
        {
            // change scale of player to face left
            transform.localScale = new Vector3(-1, 1, 1);
        }

        positionDelta = inputMovement * velocity * Time.fixedDeltaTime;
    }
}