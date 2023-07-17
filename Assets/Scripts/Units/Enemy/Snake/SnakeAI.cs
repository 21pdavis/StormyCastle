using UnityEngine;

using static Helpers;

public class SnakeAI : EnemyAI<SnakeStats>
{
    private float lastAttackTime = 0f;

    override protected void Start()
    {
        base.Start();
        stats = GetComponent<SnakeStats>();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Color oldColor = Gizmos.color;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, stats.aggroRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stats.attackRange);

        Gizmos.color = oldColor;
    }

    // TODO: sightlines, when enemy is attacked, immediately aggro it
    public override void Patrol()
    {
        if (Vector3.Distance(transform.position, target.position) < stats.aggroRange)
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Chasing);
        }

        //Debug.Log("Snake patrolling");
    }

    public override void Chase()
    {
        if (Vector3.Distance(transform.position, target.position) < stats.attackRange)
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Attacking);
        }

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

        //Debug.Log("Snake chasing");
    }

    public override void Attack()
    {
        if (
            Vector3.Distance(transform.position, target.position) > stats.attackRange
            &&
            Vector3.Distance(transform.position, target.position) < stats.aggroRange
        )
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Chasing);
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && Time.time >= lastAttackTime + stats.attackInterval)
        {
            animator.SetTrigger("attackTrigger");
            lastAttackTime = Time.time;
        }
        else if (Time.time < lastAttackTime + stats.attackInterval)
        {

        }
    }

    public override void Flee()
    {
        Debug.Log("Snake fleeing");
    }
}
