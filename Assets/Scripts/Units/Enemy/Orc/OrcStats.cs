public class OrcStats : EnemyStats
{
    private int _aggroRange = 5;
    private int _attackRange = 1;
    private int _damage = 10;

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

    protected override void Die()
    {
        // TODO
    }
}