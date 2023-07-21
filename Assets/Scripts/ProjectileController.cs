using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public UnitStats ShooterStats { get; set; }
    public UnitStats TargetStats { get; set; }
    public float speed = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Environment"))
        {
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

    private void FixedUpdate()
    {
        
    }
}
