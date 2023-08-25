using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;

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

        UnityEditor.Handles.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stats.orbitRadius);

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

        UpdateOrbits();
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

    // TODO: move this logic to script *on* the object, and just call a method on it
    private void UpdateOrbits()
    {
        // TODO: apply additional force to make objects stay at constant distance from player (probably want to combine forces into one (+?))
        foreach(GameObject orbitingObject in orbitingObjects)
        {
            Rigidbody2D orbitingRb = orbitingObject.GetComponent<Rigidbody2D>();
            Vector2 differenceVector = orbitingObject.transform.position - transform.position;
            Vector2 desiredDifferenceVector = differenceVector.normalized * stats.orbitRadius;

            // if too close, find vector needed to push back into orbit first
            if (Mathf.Abs(desiredDifferenceVector.magnitude - differenceVector.magnitude) > 0.01f)
            {
                Vector2 currentPosition = (Vector2)orbitingRb.transform.position;
                orbitingObject.transform.position = currentPosition + (desiredDifferenceVector - differenceVector);
                //orbitingRb.MovePosition(currentPosition + (desiredDifferenceVector - differenceVector) * Time.fixedDeltaTime);
            }

            // update after move
            differenceVector = orbitingObject.transform.position - transform.position;
            Vector2 normalizedTangentVector = Quaternion.AngleAxis(90, Const.rotationAxis) * differenceVector.normalized;

            //! room for optimization in caching rb's
            // TODO: interpolate positions more smoothly?

            Vector2 orbitPostionDelta = Time.fixedDeltaTime * stats.orbitSpeed * normalizedTangentVector;
            orbitingRb.MovePosition((Vector2)orbitingRb.transform.position + orbitPostionDelta);
        }
    }

    private void AddToOrbit(GameObject target)
    {
        //! slow search if needing to scale in future, should probably use a hashset or something else
        if (orbitingObjects.Count >= 3 || orbitingObjects.FirstOrDefault((obj) => ReferenceEquals(obj, target)) != default)
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
