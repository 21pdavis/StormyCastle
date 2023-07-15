using UnityEngine;

public class SnakeStatsController : EnemyStatsController
{
    void Start()
    {
        // cast stats as SnakeStats
        stats = ScriptableObject.CreateInstance<SnakeStats>();
    }

    public void SpecialMethod() { }
}
