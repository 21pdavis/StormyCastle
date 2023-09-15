using UnityEngine;

using static Helpers;

public class OgreStats : EnemyStats
{
    [SerializeField] private float _knockbackForce;

    private PlayerStats playerStats;
    private GameObject castleMusicObject;

    protected override void Start()
    {
        base.Start();
        castleMusicObject = GameObject.Find("Castle Music");
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
    }

    public float knockbackForce
    {
        get => _knockbackForce;
        set => _knockbackForce = value;
    }

    protected override void Die()
    {
        playerStats.Win();

        base.Die();
        SwitchMusic(castleMusicObject.name, castleMusicObject.GetComponent<AudioSource>());
    }
}
