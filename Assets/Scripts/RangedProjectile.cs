using System.Collections;
using UnityEngine;

public class RangedProjectile : MonoBehaviour
{

    public float damage;
    public GameObject target;

    public bool targetSet;
    public string targetType;
    public float velocity = 5;
    public bool stopProjectile;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, velocity * Time.deltaTime);

        if (!stopProjectile)
        {

        }
    }
}
