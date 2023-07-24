using UnityEngine;

public class EnvironmentObjectController : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (rb.velocity.magnitude > 0.01f)
        {
            //! NOTE: very minor ~5 fps performance hit here, but benefits are very much worth it to update paths, will see if it becomes a scaling issue
            AstarPath.active.UpdateGraphs(gameObject.GetComponent<Collider2D>().bounds);
        }
    }
}
