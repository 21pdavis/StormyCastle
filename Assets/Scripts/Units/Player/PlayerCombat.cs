using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerStats stats;
    private Rigidbody2D rb;

    public LayerMask enemyLayers;

    private float lastHealTime = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // create a semi-circle hitbox in front of the orc
        Color oldHandlesColor = UnityEditor.Handles.color;

        Color newColor = Color.red; newColor.a = 0.05f;
        UnityEditor.Handles.color = newColor;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, -transform.up, transform.localScale.x > 0 ? 180 : -180, stats.attackRange);

        UnityEditor.Handles.color = oldHandlesColor;

    }
#endif

    public void Attack(CallbackContext context)
    {
        //! rb velocity check for monster knockback, should probably be moved to a separate check/bool flag
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && context.performed && !(rb.velocity.magnitude > 0.4f))
        {
            // play attack animation
            animator.SetTrigger("attackTrigger");

            // determine direction of attack (normalized vector) and flip sprite accordingly
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 projectedMousePos = new Vector2(mousePos.x, mousePos.y);

            Vector2 directionFromCharacter = (projectedMousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
            FlipSprite(directionFromCharacter, transform);

            MeleeAttack(transform, stats, enemyLayers);
        }
    }

    public void Heal(CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time < lastHealTime + stats.healInterval) return;

            stats.Heal(stats.healAmount);
            stats.SpendMana(stats.healCost);

            GameObject particles = Instantiate(stats.healParticles, transform.position, Quaternion.identity);
            float animTime = particles.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            Destroy(particles, animTime);

            //StartCoroutine(SpawnAndDespawnHealParticles());
        }
    }
}
