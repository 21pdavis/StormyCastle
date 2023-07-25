using UnityEngine;

using static Helpers;

public class OgreStats : EnemyStats
{
    [SerializeField] private float _knockbackForce;

    private GameObject castleMusicObject;

    private void Start()
    {
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
