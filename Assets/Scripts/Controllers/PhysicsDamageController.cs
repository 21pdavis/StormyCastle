using UnityEngine;

// class to control damage dealt when an object hits another object
public class PhysicsDamageController : MonoBehaviour
{
    public float impactMultiplier = 1f;
    // TODO: revisit potentially damaging player, too?
    public LayerMask damageableLayers;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((damageableLayers & (1 << collision.gameObject.layer)) == 0)
            return;
        
        if (collision.gameObject.TryGetComponent<UnitStats>(out var targetStats))
        {
            int damage = Mathf.FloorToInt(rb.velocity.magnitude * impactMultiplier);
            targetStats.TakeDamage(damage);
        }
    }
}
