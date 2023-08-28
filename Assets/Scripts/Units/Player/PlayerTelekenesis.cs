using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using static UnityEngine.InputSystem.InputAction;
using static Helpers;

public class PlayerTelekenesis : MonoBehaviour
{
    private struct OrbitingObject
    {
        public GameObject obj;
        public AsyncOperationHandle<GameObject> handle;
    }

    public LayerMask telekenesisLayers;

    private PlayerStats stats;

    private AsyncOperationHandle<GameObject> telekenesisLightHandle;
    private List<OrbitingObject> orbitingObjects = new List<OrbitingObject>();

    private void FixedUpdate()
    {
        stats = GetComponent<PlayerStats>();

        UpdateOrbits();
    }

    private void UpdateOrbits()
    {
        // TODO: apply additional force to make objects stay at constant distance from player (probably want to combine forces into one (+?))
        foreach (OrbitingObject orbitingObject in orbitingObjects)
        {
            Rigidbody2D orbitingRb = orbitingObject.obj.GetComponent<Rigidbody2D>();
            Vector2 differenceVector = orbitingObject.obj.transform.position - transform.position;
            Vector2 desiredDifferenceVector = differenceVector.normalized * stats.orbitRadius;

            // if too close, find vector needed to push back into orbit first
            Vector2 orbitAlignmentDelta = Vector2.zero;
            if (Mathf.Abs(desiredDifferenceVector.magnitude - differenceVector.magnitude) > 0.01f)
            {
                orbitAlignmentDelta = desiredDifferenceVector - differenceVector;
            }

            // update after move
            differenceVector = orbitingObject.obj.transform.position - transform.position;
            Vector2 normalizedTangentVector = Quaternion.AngleAxis(90, Const.rotationAxis) * differenceVector.normalized;

            //! room for optimization in caching rb's
            Vector2 orbitPositionDelta = Vector2.zero;
            if (!GetComponent<PlayerMovement>().moving)
            {
                orbitPositionDelta = Time.fixedDeltaTime * stats.orbitSpeed * normalizedTangentVector;
            }
            orbitingRb.AddForce(orbitPositionDelta + orbitAlignmentDelta, ForceMode2D.Impulse);
        }
    }

    public void Telekenesis(CallbackContext context)
    {
        if (context.performed)
        {
            // two cases: if we're already holding an object, we drop it, otherwise we pick up object nearest to cursor
            if (stats.heldObject == null)
            {
                // TODO: revisit: should player be unable to move while holding an object? Don't see why. Maybe just slow down instead?
                //GetComponent<PlayerMovement>().playerFreezeOverride = true;

                Collider2D nearestTarget = GetNearestToMouse(stats.telekenesisRadius, telekenesisLayers);
                if (nearestTarget == default) return;

                telekenesisLightHandle = InstantiatePrefabByKey(
                    "Prefabs/Telekenesis Glow",
                    nearestTarget.transform.position,
                    Quaternion.identity,
                    nearestTarget.transform
                );

                // TODO: special case for projectiles, change isFriendly tag
                if (nearestTarget.CompareTag("Projectile"))
                {

                }

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
        // slow Any() iteration is fine here, it's only 3 objects max, can optimize later if needed
        if (orbitingObjects.Count >= 3 || orbitingObjects.Any((obj) => ReferenceEquals(obj, target)))
            return;

        AsyncOperationHandle<GameObject> orbitHandle = InstantiatePrefabByKey(
            "Prefabs/Telekenesis Glow",
            target.transform.position,
            Quaternion.identity,
            target.transform
        );

        orbitingObjects.Add(new OrbitingObject { obj = target, handle = orbitHandle });
    }

    public void Orbit(CallbackContext context)
    {
        // TODO: different max orbiting objects based on level? Q: is wider orbit better? not necessarily right?
        if (context.performed && stats.heldObject == null)
        {
            Collider2D nearestTarget = GetNearestToMouse(stats.telekenesisRadius, telekenesisLayers);
            if (nearestTarget != default)
            {
                AddToOrbit(nearestTarget.gameObject);
            }
        }
    }

    public void ThrowOrbitingObject(CallbackContext context)
    {
        if (context.performed)
        {
            if (orbitingObjects.Count == 0) return;

            //! should it be least recent object? Closest to mouse? unsure... playtest/see what other games do
            GameObject leastRecentOrbitingObject = orbitingObjects[0].obj;
            Vector2 throwDirection = (GetProjectedMousePos() - (Vector2)leastRecentOrbitingObject.transform.position).normalized;

            Addressables.ReleaseInstance(orbitingObjects[0].handle);

            orbitingObjects.RemoveAt(0);

            // TODO: move object out to the side, maybe shake it a little, then throw for better visual clarity
            leastRecentOrbitingObject.GetComponent<Rigidbody2D>().AddForce(throwDirection * stats.orbitThrowForce, ForceMode2D.Impulse);
        }
    }
}
