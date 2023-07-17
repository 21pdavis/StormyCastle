public class EnemyStateMachine
{
    public enum EnemyState
    {
        Patrolling,
        Fleeing,
        Chasing,
        Attacking
    }

    public EnemyState CurrentState { get; private set; }

    public EnemyStateMachine()
    {
        SetState(EnemyState.Patrolling);
    }

    public void SetState(EnemyState newState)
    {
        CurrentState = newState;
        switch (newState)
        {
            case EnemyState.Patrolling:
                break;
            case EnemyState.Fleeing:
                break;
            case EnemyState.Chasing:
                break;
            case EnemyState.Attacking:
                break;
            default:
                break;
        }
    }
}
