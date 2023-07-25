using UnityEngine;

using static Helpers;

public class OrcAI : EnemyAI<OrcStats>
{
    override protected void Start()
    {
        base.Start();
        stats = GetComponent<OrcStats>();
    }

#if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        base.OnDrawGizmos();

        // create a semi-circle hitbox in front of the orc
        Color oldHandlesColor = UnityEditor.Handles.color;

        Color newColor = Color.red; newColor.a = 0.05f;
        UnityEditor.Handles.color = newColor;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, -transform.up, transform.localScale.x > 0 ? 180 : -180, stats.attackRange);

        UnityEditor.Handles.color = oldHandlesColor;
    }
#endif

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
