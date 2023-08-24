using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerStats stats;
    private Rigidbody2D rb;
    private AsyncOperationHandle<GameObject> telekenesisLightHandle;
    private List<GameObject> orbitingObjects = new List<GameObject>();

    public LayerMask enemyLayers;
    public LayerMask telekenesisLayers;

    private float lastHealTime = 0f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // create a semi-circle hitbox in front of the orc
        Color oldHandlesColor = UnityEditor.Handles.color;

        Color newColor = Color.red; newColor.a = 0.05f;
        UnityEditor.Handles.color = newColor;
        UnityEditor.Handles.DrawSolidArc(transform.position, Vector3.forward, -transform.up, transform.localScale.x > 0 ? 180 : -180, stats.attackRange);

        Gizmos.DrawWireSphere(GetProjectedMousePos(), stats.telekenesisRadius);

        UnityEditor.Handles.color = oldHandlesColor;
    }
#endif

    private void FixedUpdate()
    {
        if (stats.heldObject != null)
        {
            // telekenesis object
            stats.heldObject.GetComponent<EnvironmentObjectController>().gravityTarget = GetProjectedMousePos();
        }
    }

    public void Attack(CallbackContext context)
    {
        //! rb velocity check for monster knockback, should probably be moved to a separate check/bool flag
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && context.performed && !(rb.velocity.magnitude > 0.4f))
        {
            // play attack animation
            animator.SetTrigger("attackTrigger");

            Vector2 directionFromCharacter = (GetProjectedMousePos() - new Vector2(transform.position.x, transform.position.y)).normalized;
            FlipSprite(directionFromCharacter, transform);

            MeleeAttack(transform, stats, enemyLayers);
        }
    }

    public void Heal(CallbackContext context)
    {
        if (context.performed)
        {
            if (Time.time < lastHealTime + stats.healInterval) return;

            stats.Heal(stats.healAmount);
            stats.SpendMana(stats.healCost);

            GameObject particles = Instantiate(stats.healParticles, transform.position, Quaternion.identity);
            float animTime = particles.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
            Destroy(particles, animTime);
        }
    }

    public void Telekenesis(CallbackContext context)
    {
        if (context.performed)
        {
            // two cases: if we're already holding an object, we drop it, otherwise we pick up object nearest to cursor
            if (stats.heldObject == null)
            {
                GetComponent<PlayerMovement>().playerFreezeOverride = true;

                Collider2D nearestTarget = GetNearestToMouse(stats.telekenesisRadius, telekenesisLayers);
                if (nearestTarget == default) return;

                InstantiatePrefabByKey(
                    ref telekenesisLightHandle,
                    "Prefabs/Telekenesis Glow",
                    nearestTarget.transform.position,
                    Quaternion.identity,
                    nearestTarget.transform
                );

                stats.heldObject = nearestTarget.gameObject;
            }
            else
            {
                stats.heldObject.GetComponent<EnvironmentObjectController>().gravityTarget = Vector2.zero;
                stats.heldObject = null;
                GetComponent<PlayerMovement>().playerFreezeOverride = false;

                Addressables.ReleaseInstance(telekenesisLightHandle);
            }
        }
    }

    private void AddToOrbit(GameObject target)
    {
        if (orbitingObjects.Count >= 3)
            return;

        orbitingObjects.Add(target);

        InstantiatePrefabByKey(
            ref telekenesisLightHandle,
            "Prefabs/Telekenesis Glow",
            target.transform.position,
            Quaternion.identity,
            target.transform
        );
    }

    public void Orbit(CallbackContext context)
    {
        // TODO: different max orbiting objects based on level?
        if (context.performed && stats.heldObject == null)
        {
            Collider2D nearestTarget = GetNearestToMouse(stats.telekenesisRadius, telekenesisLayers);
            AddToOrbit(nearestTarget.gameObject);
        }
    }
}
