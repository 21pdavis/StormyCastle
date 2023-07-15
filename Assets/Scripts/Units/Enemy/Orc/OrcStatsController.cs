using UnityEngine;

public class OrcStatsController : EnemyStatsController
{
    void Start()
    {
        // cast stats as SnakeStats
        stats = ScriptableObject.CreateInstance<OrcStats>();
    }
}
