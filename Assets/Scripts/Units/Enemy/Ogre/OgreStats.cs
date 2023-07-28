using UnityEngine;

using static Helpers;

public class OgreStats : EnemyStats
{
    [SerializeField] private float _knockbackForce;

    private GameObject castleMusicObject;

    protected override void Start()
    {
        base.Start();
        castleMusicObject = GameObject.Find("Castle Music");
    }

    public float knockbackForce
    {
        get => _knockbackForce;
        set => _knockbackForce = value;
    }

    protected override void Die()
    {
        base.Die();
        SwitchMusic(castleMusicObject.name, castleMusicObject.GetComponent<AudioSource>());
    }
}
