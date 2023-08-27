using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public UnitStats ShooterStats { get; set; }
    public UnitStats TargetStats { get; set; }
    public float speed = 5f;
    public float impulseForce = 5f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Environment"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Environment Object"))
        {
            // neat little syntax here to check if the component exists and assign it to a variable
            if (collision.TryGetComponent<Rigidbody2D>(out var objectRb))
            {
                objectRb.AddForce(rb.velocity.normalized * impulseForce, ForceMode2D.Impulse);
            }
            Destroy(gameObject);
            return;
        }

        // convention: use capsule colliders for hitboxes, circles for environment interactions
        CapsuleCollider2D capsuleCollider = collision as CapsuleCollider2D;
        if (capsuleCollider != null && TargetStats != null && TargetStats.CompareTag(collision.tag))
        {
            Destroy(gameObject);
            TargetStats.TakeDamage(ShooterStats.damage);
        }
    }
}
