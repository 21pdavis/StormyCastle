using UnityEngine;
using Pathfinding;

using static Helpers;

public class EnemyAI : MonoBehaviour
{
    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    // current path we are following
    private Path path;
    // current targeted waypoint on path
    private int currentWaypoint = 0;
    //bool reachedEndOfPath = false;
    private bool moving;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;

    private void Start()
    {
        moving = false;
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // invoke function UpdatePath every 0.5 seconds
        InvokeRepeating(nameof(UpdatePath), 0f, .3f);
    }

    private void Update()
    {
        AnimateMove();
    }

    private void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            //reachedEndOfPath = true;
            return;
        }
        else
        {
            //reachedEndOfPath = false;
        }

        if (rb.velocity.magnitude > 0f)
        {
            moving = true;
        }
        else
        {
            moving = false;
        }

        // get vector in direction of next waypoint (vector subtraction, https://www.varsitytutors.com/hotmath/hotmath_help/topics/adding-and-subtracting-vectors)
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = speed * Time.fixedDeltaTime * direction;

        // push in direction and flip sprite
        rb.AddForce(force);
        FlipSprite(force, transform);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
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
}
