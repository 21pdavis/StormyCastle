using UnityEngine;

public class SnakeAI : EnemyAI
{
    private EnemyStateMachine stateMachine;

    // Start is called before the first frame update
    new protected void Start()
    {
        base.Start();
        stateMachine = new();
    }

    // Update is called once per frame
    protected void Update()
    {
        switch (stateMachine.CurrentState)
        {
            case EnemyStateMachine.EnemyState.Patrolling:
                break;
            case EnemyStateMachine.EnemyState.Fleeing:
                break;
            case EnemyStateMachine.EnemyState.Chasing:
                break;
            case EnemyStateMachine.EnemyState.Attacking:
                break;
            default:
                break;
        }
    }
}
