using UnityEngine;
using Pathfinding;

using static Helpers;

public abstract class EnemyAI<Stats> : MonoBehaviour where Stats : EnemyStats
{
    public Transform target;

    public float speed = 200f;
    public float nextWaypointDistance = 3f;

    public Stats stats { protected set; get; }

    protected Seeker seeker;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected EnemyStateMachine stateMachine;

    // current path we are following
    protected Path path;
    // current targeted waypoint on path
    protected int currentWaypoint = 0;
    protected bool reachedEndOfPath = false;
    protected bool moving;

    public abstract void Patrol();
    public abstract void Chase();
    public abstract void Attack();
    public abstract void Flee();

    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        stateMachine = new();
        moving = false;

        // invoke function UpdatePath every 0.5 seconds
        InvokeRepeating(nameof(UpdatePath), 0f, .3f);
    }

    protected void Update()
    {
        //AnimateMove();
        AnimateMove(animator, moving);
    }

    private void FixedUpdate()
    {
        switch (stateMachine.CurrentState)
        {
            case EnemyStateMachine.EnemyState.Patrolling:
                Patrol();
                break;
            case EnemyStateMachine.EnemyState.Chasing:
                Chase();
                break;
            case EnemyStateMachine.EnemyState.Attacking:
                Attack();
                break;
            case EnemyStateMachine.EnemyState.Fleeing:
                Flee();
                break;
            default:
                break;
        }

        //if (path == null)
        //    return;

        //if (currentWaypoint >= path.vectorPath.Count)
        //{
        //    reachedEndOfPath = true;
        //    return;
        //}
        //else
        //{
        //    reachedEndOfPath = false;
        //}

        //if (rb.velocity.magnitude > 0f)
        //{
        //    moving = true;
        //}
        //else
        //{
        //    moving = false;
        //}

        //// get vector in direction of next waypoint (vector subtraction, https://www.varsitytutors.com/hotmath/hotmath_help/topics/adding-and-subtracting-vectors)
        //Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        //Vector2 force = speed * Time.fixedDeltaTime * direction;

        //// push in direction and flip sprite
        //rb.AddForce(force);
        //FlipSprite(force, transform);

        //float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        //if (distance < nextWaypointDistance)
        //{
        //    currentWaypoint++;
        //}
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
}
