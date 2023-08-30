using UnityEngine;

public class EnvironmentObjectController : MonoBehaviour
{
    private Rigidbody2D rb;
    // TODO: environment object stats?
    private Vector2 _gravityTarget;

    public Vector2 gravityTarget
    {
        get { return _gravityTarget; }
        set { _gravityTarget = value; }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.01f)
        {
            //! NOTE: very minor ~5 fps performance hit here, but benefits are very much worth it to update paths, will see if it becomes a scaling issue
            // this is also somewhat optimized already since only the graphs overlapping the collider bounds are updated
            AstarPath.active.UpdateGraphs(gameObject.GetComponent<Collider2D>().bounds);
        }

        // TODO: changee from Vector2.zero check to using a bool flag for rare case of 0,0 being a valid target
        if (gravityTarget != Vector2.zero)
        {
            Vector2 gravityDirection = gravityTarget - (Vector2)transform.position;
            rb.AddForce(gravityDirection, ForceMode2D.Impulse);
        }
    }
}
