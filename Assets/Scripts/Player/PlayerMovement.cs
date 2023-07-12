using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody2D characterBody;
    private Vector2 velocity;
    private Vector2 inputMovement;

    // Start is called before the first frame update
    void Start()
    {
        velocity = new Vector2(speed, speed);
        characterBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputMovement = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

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
    }

    private void FixedUpdate()
    {
        // move the player based on input and velocity
            characterBody.MovePosition(characterBody.position + inputMovement * velocity * Time.fixedDeltaTime);
    }
}
