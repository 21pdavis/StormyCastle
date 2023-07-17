using UnityEngine;

// TODO: Healthbars and damage numbers
public abstract class EnemyStatsController : MonoBehaviour
{
    public EnemyStats stats { protected set; get; }

    public void TakeDamage(int amount)
    {
        stats.ModifyHealth(-amount);

        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        stats.ModifyHealth(amount);
    }

    public void Die()
    {
        // TODO
    }
}
