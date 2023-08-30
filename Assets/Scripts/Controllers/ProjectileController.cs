using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public LayerMask FriendlyLayers;
    public LayerMask EnemyLayers;
    public LayerMask StructureLayers;
    public UnitStats ShooterStats { get; set; }
    public UnitStats TargetStats { get; set; }
    public float speed;
    public float impulseForce;
    public bool destroyOnCollision;
    public bool isFriendly;

    private Rigidbody2D rb;

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        LayerMask targetLayers = (isFriendly ? EnemyLayers : FriendlyLayers) | StructureLayers;

        if ((targetLayers & (1 << collision.gameObject.layer)) != 0)
        {
            Destroy(gameObject);

            if (collision.CompareTag("Environment Object") && collision.TryGetComponent<Rigidbody2D>(out var objectRb))
            {
                // TODO: object damage/health for breaking barrels etc.?
                objectRb.AddForce(rb.velocity.normalized * impulseForce, ForceMode2D.Impulse);
            }

            // TODO: rework how TargetStats is set
            if (collision.TryGetComponent<UnitStats>(out var targetStats))
            {
                targetStats.TakeDamage(ShooterStats.damage);
            }
        }
    }
}
