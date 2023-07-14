using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerStatsController : MonoBehaviour
{
    private PlayerStats playerStats;

    public HealthBar healthBar;
    public ManaBar manaBar;

    private void Start()
    {
        playerStats = ScriptableObject.CreateInstance<PlayerStats>();
        healthBar.SetMaxHealth(playerStats.maxHealth);
        manaBar.SetMaxMana(playerStats.maxMana);
    }

    public void TakeDamage(int amount)
    {
        playerStats.ModifyHealth(-amount);
        healthBar.SetHealth(playerStats.currentHealth);

        if (playerStats.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        playerStats.ModifyHealth(amount);
        healthBar.SetHealth(playerStats.currentHealth);
    }

    public void TempSpace(CallbackContext context)
    {
        if (context.performed)
        {
            TakeDamage(20);
        }
    }

    public void SpendMana(int amount)
    {
        playerStats.ModifyMana(-amount);
    }

    public void GainMana(int amount)
    {
        playerStats.ModifyMana(amount);
    }

    public void Die()
    {
        // TODO
    }
}