public class SnakeStats : EnemyStats
{
    private int _aggroRange = 5;
    private int _attackRange = 4;
    private int _damage = 10;
    private int _attackInterval = 2;

    public override int aggroRange
    {
        get { return _aggroRange; }
        set { _aggroRange = value; }
    }

    public override int attackRange
    {
        get { return _attackRange; }
        set { _attackRange = value; }
    }

    public override int damage
    {
        get { return _damage; }
        set { _damage = value; }
    }

    /// <summary>
    /// The attack interval in seconds between when snake can shoot a projectile.
    /// </summary>
    public int attackInterval
    {
        get { return _attackInterval; }
        set { _attackInterval = value; }
    }

    protected override void Die()
    {
        // TODO
    }
}
