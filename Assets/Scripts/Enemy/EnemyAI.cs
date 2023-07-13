using UnityEngine;
using Pathfinding;

using static Helpers;

public class EnemyAI : MonoBehaviour
{
    public Transform target;
    public Transform enemyGraphics;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    // current path we are following
    Path path;
    // current targeted waypoint on path
    int currentWaypoint = 0;
    bool reachedEndOfPath = false;

    Seeker seeker;
    Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        // invoke function UpdaetPath every 0.5 seconds
        InvokeRepeating("UpdatePath", 0f, .5f);
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

    void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        // get vector in direction of next waypoint (vector subtraction, https://www.varsitytutors.com/hotmath/hotmath_help/topics/adding-and-subtracting-vectors)
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = speed * Time.deltaTime * direction;

        // push in direction and flip sprite
        rb.AddForce(force);
        FlipSprite(force, enemyGraphics);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }
}
