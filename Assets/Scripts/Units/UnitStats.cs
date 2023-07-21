using UnityEngine;

public abstract class UnitStats : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _currentHealth = 100;
    // Risk of Rain style damage, each unit has an assigned "damage" value and other damages are based on percentages of that
    [SerializeField] private int _damage = 25;
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _attackRange = 1f;

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

    public virtual float speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public virtual float attackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
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

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}
