using UnityEngine;

public class OgreStats : EnemyStats
{
    [SerializeField] private float _knockbackForce;

    public float knockbackForce
    {
        get => _knockbackForce;
        set => _knockbackForce = value;
    }
}
