using UnityEngine;

using static Helpers;

public class SnakeAI : EnemyAI<SnakeStats>
{
    override protected void Start()
    {
        base.Start();
        stats = GetComponent<SnakeStats>();
    }

    public override void Attack()
    {
        if (
            Vector3.Distance(transform.position, target.position) > stats.attackRange
            &&
            Vector3.Distance(transform.position, target.position) < stats.aggroRange
            &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
        )
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Chasing);
        }

        moving = false;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && Time.time >= lastAttackTime + stats.attackInterval)
        {
            FlipSprite((Vector2)target.position - rb.position, transform);
            animator.SetTrigger("attackTrigger");
            lastAttackTime = Time.time;

            // spawn and shoot projectile
            Transform shootPoint = transform.Find("ShootPoint");
            Quaternion towardsTarget = Quaternion.LookRotation(Vector3.forward, target.GetComponent<Collider2D>().bounds.center - shootPoint.position);
            // TODO: maybe use Addressable instead of Instantiate, but this is fine for now
            GameObject projectile = Instantiate(stats.projectile, shootPoint.position, towardsTarget);
            ProjectileController projectileController = projectile.GetComponent<ProjectileController>();

            projectileController.ShooterStats = stats;
            projectileController.TargetStats = target.GetComponent<UnitStats>();
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            Vector2 direction = target.position - transform.position;
            projectileRb.velocity = direction.normalized * projectile.GetComponent<ProjectileController>().speed;
        }
        else if (Time.time < lastAttackTime + stats.attackInterval)
        {
            // TODO
        }
    }

    public override void Flee()
    {
        Debug.Log("Snake fleeing");
    }
}
