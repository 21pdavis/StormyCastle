using UnityEngine;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;
using System.Linq;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCombat : MonoBehaviour
{
    private Animator animator;
    private PlayerStats stats;
    private Rigidbody2D rb;
    private AsyncOperationHandle<GameObject> telekenesisLightHandle;

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
            GetComponent<PlayerMovement>().canMove = false;
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
                Vector2 mousePos = GetProjectedMousePos();
                Collider2D[] targets = Physics2D.OverlapCircleAll(mousePos, stats.telekenesisRadius, telekenesisLayers);

                Collider2D nearestTarget = targets.OrderBy(x => Vector2.Distance(x.transform.position, mousePos)).FirstOrDefault();
                if (nearestTarget == default) return;

                telekenesisLightHandle = Addressables.InstantiateAsync(
                    key: "Prefabs/Telekenesis Glow",
                    position: nearestTarget.transform.position,
                    rotation: Quaternion.identity
                );

                telekenesisLightHandle.Completed += (obj) =>
                {
                    // set parent after completion to avoid positional offset
                    obj.Result.transform.position = nearestTarget.transform.position;
                    obj.Result.transform.SetParent(nearestTarget.transform);
                };

                stats.heldObject = nearestTarget.gameObject;
            }
            else
            {
                stats.heldObject.GetComponent<EnvironmentObjectController>().gravityTarget = Vector2.zero;
                stats.heldObject = null;
                GetComponent<PlayerMovement>().canMove = true;

                Addressables.ReleaseInstance(telekenesisLightHandle);
            }
        }
    }
}
