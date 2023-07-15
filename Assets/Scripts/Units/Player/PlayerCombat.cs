using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerStatsController playerStatsController;
    private PlayerStats playerStats;

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerStatsController = GetComponent<PlayerStatsController>();
        playerStats = playerStatsController.playerStats;
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

            // detect enemies in range of attack
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

            foreach (var enemy in hitEnemies)
            {
                var enemyStatsController = enemy.GetComponent<EnemyStatsController>();

                //if (enemyStatsController is SnakeStatsController) // leaving this here as a note on how to do this
                //{
                //    Debug.Log("It's a snake!");
                //}

                // get player's damage value
                int damageDealt = playerStats.damage;

                // actually deal damage
                enemyStatsController.TakeDamage(damageDealt);
                Debug.Log($"Hit {enemy.name}, its health was {enemyStatsController.stats.currentHealth + damageDealt} and is now {enemyStatsController.stats.currentHealth}");
            }
        }
    }
}
