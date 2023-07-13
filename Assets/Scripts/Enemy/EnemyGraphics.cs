using UnityEngine;
using Pathfinding;

public class EnemyGraphics : MonoBehaviour
{
    public AIPath aiPath;

    private void Start()
    {
        aiPath = GetComponentInParent<AIPath>();
    }

    void Update()
    {
        if (aiPath.desiredVelocity.x <= 0.01)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (aiPath.desiredVelocity.x >= 0.01)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}
