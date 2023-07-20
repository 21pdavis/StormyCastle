using UnityEngine;
using UnityEditor;

using static Helpers;

public class OrcAI : EnemyAI<OrcStats>
{
    override protected void Start()
    {
        base.Start();
        stats = GetComponent<OrcStats>();
    }

    protected override void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        base.OnDrawGizmos();

        // create a semi-circle hitbox in front of the orc
        Color oldHandlesColor = Handles.color;

        Color newColor = Color.red; newColor.a = 0.05f;
        Handles.color = newColor;
        Handles.DrawSolidArc(transform.position, Vector3.forward, -transform.up, transform.localScale.x > 0 ? 180 : -180, stats.attackRange);

        Handles.color = oldHandlesColor;
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

        moving = false;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && Time.time > lastAttackTime + stats.attackInterval)
        {
            FlipSprite((Vector2)target.position - rb.position, transform);
            animator.SetTrigger("attackTrigger");
            lastAttackTime = Time.time;

            MeleeAttack(transform, stats, targetLayers);
        }
        else if (Time.time < lastAttackTime + stats.attackInterval)
        {
            // TODO
        }
    }

    public override void Flee()
    {
        Debug.Log("Orc fleeing");
    }
}
