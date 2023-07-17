using UnityEngine;

public abstract class UnitStats : MonoBehaviour
{
    private int _maxHealth = 100;
    private int _currentHealth = 100;

    // Risk of Rain style damage, each unit has an assigned "damage" value and other damages are based on percentages of that
    private int _damage = 5;

    public virtual int maxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }

    public virtual int currentHealth
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }

    public virtual int damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    private void ModifyHealth(int delta)
    {
        // Ensure that the current health doesn't exceed the maximum
        currentHealth = Mathf.Clamp(currentHealth + delta, 0, maxHealth);
    }

    public virtual void TakeDamage(int amount)
    {
        ModifyHealth(-amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int amount)
    {
        ModifyHealth(amount);
    }

    protected abstract void Die();
}
