using System.Collections;

using UnityEngine;

// class to control damage dealt when an object hits another object
public class PhysicsDamageController : MonoBehaviour
{
    public bool canDealPhysicsDamage;
    public float impactMultiplier = 1f;
    // TODO: revisit potentially damaging player, too?
    public LayerMask damageableLayers;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        canDealPhysicsDamage = false;
    }

    /// <summary>
    /// Make object temporarily able to deal damage for 2 seconds, enough to travel and hit something
    /// </summary>
    /// <returns></returns>
    public IEnumerator MakeDamaging()
    {
        canDealPhysicsDamage = true;
        yield return new WaitForSecondsRealtime(2f);
        canDealPhysicsDamage = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canDealPhysicsDamage || (damageableLayers & (1 << collision.gameObject.layer)) == 0)
            return;
        
        if (collision.gameObject.TryGetComponent<UnitStats>(out var targetStats))
        {
            int damage = Mathf.FloorToInt(rb.velocity.magnitude * impactMultiplier);
            targetStats.TakeDamage(damage);
        }
    }
}
