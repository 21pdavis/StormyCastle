public abstract class EnemyStats : UnitStats
{
    private int _aggroRange = 5;
    private int _attackRange = 1;

    public virtual int aggroRange
    {
        get { return _aggroRange; }
        set { _aggroRange = value; }
    }

    public virtual int attackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
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
