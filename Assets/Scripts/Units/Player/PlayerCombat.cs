using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;
using UnityEditor.Experimental.GraphView;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerStats stats;

    public LayerMask enemyLayers;

    private void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // create a semi-circle hitbox in front of the orc
        Color oldHandlesColor = Handles.color;

        Color newColor = Color.red; newColor.a = 0.05f;
        Handles.color = newColor;
        Handles.DrawSolidArc(transform.position, Vector3.forward, -transform.up, transform.localScale.x > 0 ? 180 : -180, stats.attackRange);

        Handles.color = oldHandlesColor;

    }

    public void Attack(CallbackContext context)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && context.performed)
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
}
