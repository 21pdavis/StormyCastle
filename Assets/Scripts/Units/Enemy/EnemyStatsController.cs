using UnityEngine;

// TODO: Healthbars and damage numbers
public class EnemyStatsController : MonoBehaviour
{
    public EnemyStats enemyStats { private set; get; }

    // Start is called before the first frame update
    void Start()
    {
        enemyStats = ScriptableObject.CreateInstance<EnemyStats>();
    }

    public void TakeDamage(int amount)
    {
        enemyStats.ModifyHealth(-amount);

        if (enemyStats.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        enemyStats.ModifyHealth(amount);
    }

    public void Die()
    {
        // TODO
    }
}
