using UnityEngine;

public abstract class EnemyStats : UnitStats
{
    [SerializeField] private float _aggroRange = 5;
    [SerializeField] private float _attackRange = 1;
    [SerializeField] private float _attackInterval = 2;

    public virtual float aggroRange
    {
        get { return _aggroRange; }
        set { _aggroRange = value; }
    }

    public virtual float attackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }

    /// <summary>
    /// The attack interval in seconds between when enemy can attack
    /// </summary>
    public float attackInterval
    {
        get { return _attackInterval; }
        set { _attackInterval = value; }
    }

    private void Start()
    {
        // TODO: Set up health bar/other resource bars
    }

    public override void TakeDamage(int amount)
    {
        // TODO: update health bar
        base.TakeDamage(amount);
    }

    public override void Heal(int amount)
    {
        // TODO: update health bar
        base.Heal(amount);
    }
}
