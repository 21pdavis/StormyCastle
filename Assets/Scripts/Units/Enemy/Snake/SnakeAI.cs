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
