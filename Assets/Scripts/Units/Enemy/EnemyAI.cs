using UnityEngine;
using Pathfinding;

using static Helpers;

public abstract class EnemyAI<Stats> : MonoBehaviour where Stats : EnemyStats
{
    public Stats stats { protected set; get; }

    protected Transform target;

    protected int currentWaypoint = 1;
    protected int lastWaypoint = 1;

    protected float lastAttackTime = 0f;

    protected Seeker seeker;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected EnemyStateMachine stateMachine;
    protected LayerMask targetLayers;

    // current path we are following
    protected Path path;
    // current targeted waypoint on path
    protected bool reachedEndOfPath = false;
    protected bool moving;
    protected bool tracking = false;
    protected float lastTrackTime = 0f;
    protected const float trackInterval = 0.25f;

    protected virtual void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.Find("Player");
            target = player.transform;
        }

        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        int layerNum = target.gameObject.layer;
        targetLayers = 1 << layerNum;

        stateMachine = new();
        moving = false;

        // invoke function UpdatePath every 0.5 seconds
        //InvokeRepeating(nameof(UpdatePath), 0f, .25f);
    }

    // Tip: abstract means no implementation in base class, must be implemented in derived class, and virtual means there is a default implementation in base class but can be overridden in derived class
    // TODO: sightlines + aggro on being attacked
    public virtual void Patrol()
    {
        if (Vector3.Distance(transform.position, target.position) < stats.aggroRange)
        {
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

        // TODO: check and account for getting stuck on wall/environment collider (maybe check for how far traveled in last movement?)
        if (Vector2.Distance(rb.position, (Vector2)path.vectorPath[currentWaypoint]) < stats.speed * Time.fixedDeltaTime * 5f && currentWaypoint + 1  < path.vectorPath.Count)
        {
            currentWaypoint++;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 positionDelta = stats.speed * Time.fixedDeltaTime * direction;

        if (positionDelta.magnitude > 0f)
        {
            moving = true;
            FlipSprite(direction, transform);
            // push in direction and flip sprite
            rb.MovePosition(rb.position + positionDelta);
        }
        else
        {
            moving = false;
        }
    }

    public abstract void Attack();

    public abstract void Flee();

    protected virtual void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Color oldColor = Gizmos.color;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stats.aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);

        // the below two gizmo draws are for tracking current waypoint relative to current maximum move distance
        //Gizmos.color = Color.magenta;
        //Gizmos.DrawWireSphere(transform.position, stats.speed * Time.fixedDeltaTime * 50);

        //Gizmos.color = Color.green;
        //Gizmos.DrawWireSphere(path.vectorPath[currentWaypoint], stats.speed * Time.fixedDeltaTime * 50);

        Gizmos.color = oldColor;
    }

    protected virtual void Update()
    {
        AnimateMove(animator, moving);

        if (Time.time > lastTrackTime + trackInterval)
        {
            UpdatePath();
            lastTrackTime = Time.time;
        }
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
