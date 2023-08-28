using System.Collections;
using UnityEngine;

using static Helpers;

public class OgreAI : EnemyAI<OgreStats>
{
    // TODO: not efficient, just leaving this note everywhere to remind myself to make a music manager script
    private AudioSource roarSource;
    private GameObject bossMusicObject;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        stats = GetComponent<OgreStats>();

        roarSource = GetComponent<AudioSource>();
        bossMusicObject = GameObject.Find("Boss Music");
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

    public override void Patrol()
    {
        if (Vector3.Distance(transform.position, target.position) < stats.aggroRange)
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Chasing);
            transform.Find("Canvas").gameObject.SetActive(true);
            SwitchMusic(bossMusicObject.name, bossMusicObject.GetComponent<AudioSource>());
        }
    }

    public override void Attack()
    {
        if (
            Vector3.Distance(transform.position, target.position) > stats.attackRange
            &&
            Vector3.Distance(transform.position, target.position) < stats.aggroRange
            &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack Pre Damage")
            &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack Damage")
            &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack Post Damage")
        )
        {
            stateMachine.SetState(EnemyStateMachine.EnemyState.Chasing);
        }

        moving = false;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack Pre Damage") && Time.time > lastAttackTime + stats.attackInterval)
        {
            animator.SetTrigger("attackTrigger");
            StartCoroutine(DelayedAttack());
            FlipSprite((Vector2)target.position - rb.position, transform);
        }
        else if (Time.time < lastAttackTime + stats.attackInterval)
        {
            // TODO
        }
    }

    // TODO: this is a hacky way to delay the attack (relying on a fixed time value instead of frame data), but it works for now
    private IEnumerator DelayedAttack()
    {
        // TODO: WaitForSecondsRealtime?
        yield return new WaitForSeconds(1f);
        roarSource.Play();
        MeleeAttack(transform, stats, targetLayers, pushForce: 20f);
        lastAttackTime = Time.time;
    }

    public override void Flee()
    {
        Debug.Log("Ogre fleeing");
    }
}
