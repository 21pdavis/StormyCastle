using UnityEngine;
using Pathfinding;

using static Helpers;

public abstract class EnemyAI<Stats> : MonoBehaviour where Stats : EnemyStats
{
    public Transform target;

    public Stats stats { protected set; get; }

    protected int currentWaypoint = 1;
    protected int lastWaypoint = 1;

    protected float lastAttackTime = 0f;

    protected Seeker seeker;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected EnemyStateMachine stateMachine;

    // current path we are following
    protected Path path;
    // current targeted waypoint on path
    protected bool reachedEndOfPath = false;
    protected bool moving;

    // Tip: abstract means no implementation in base class, must be implemented in derived class, and virtual means there is a default implementation in base class but can be overridden in derived class
    // TODO: sightlines + aggro on being attacked
    public virtual void Patrol()
    {
        if (Vector3.Distance(transform.position, target.position) < stats.aggroRange)
        {
            Debug.Log($"Patrol: switching {name} to chasing");
            stateMachine.SetState(EnemyStateMachine.EnemyState.Chasing);
        }
    }
    public virtual void Chase()
    {
        if (Vector3.Distance(transform.position, target.position) < stats.attackRange)
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Attacking);
        }

        if (path == null || path.vectorPath.Count < 2)
            return;

        // get vector in direction of next waypoint (vector subtraction, https://www.varsitytutors.com/hotmath/hotmath_help/topics/adding-and-subtracting-vectors)

        // TODO: check for getting stuck on wall/environment collider
        if (Vector2.Distance(rb.position, (Vector2)path.vectorPath[currentWaypoint]) < stats.speed * Time.fixedDeltaTime)
        {
            Debug.Log("too close, incrementing");
            currentWaypoint++;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 positionDelta = stats.speed * Time.fixedDeltaTime * direction;

        if (positionDelta.magnitude > 0f)
        {
            FlipSprite(direction, transform);
            // push in direction and flip sprite
            rb.MovePosition(rb.position + positionDelta);
        }
    }

    public abstract void Attack();

    public abstract void Flee();

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Color oldColor = Gizmos.color;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stats.aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, stats.speed * Time.fixedDeltaTime * 50);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(path.vectorPath[currentWaypoint], stats.speed * Time.fixedDeltaTime * 50);

        Gizmos.color = oldColor;
    }

    protected virtual void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        stateMachine = new();
        moving = false;

        // invoke function UpdatePath every 0.5 seconds
        InvokeRepeating(nameof(UpdatePath), 0f, .25f);
    }

    protected virtual void Update()
    {
        AnimateMove(animator, moving);
    }

    protected virtual void FixedUpdate()
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
    }

    protected virtual void AnimateMove(Animator animator, bool moving)
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

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            currentWaypoint = 1;
            path = p;
        }
    }
}
