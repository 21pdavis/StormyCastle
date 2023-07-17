using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Enemy/EnemyStats")]
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
}
