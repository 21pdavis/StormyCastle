using System.Collections;
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
    private PlayerMovement movement;

    private AsyncOperationHandle<GameObject> telekenesisLightHandle;
    private List<OrbitingObject> orbitingObjects;
    // TODO: Left off here, need to rework "orbiting" into a different word - distinguish between objects stored for later throwing object actively circling player
    private bool orbiting;

    private void Start()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();

        orbiting = false;
        orbitingObjects = new List<OrbitingObject>();
    }

    private void Update()
    {
        if (GetComponent<PlayerMovement>().moving)
        {
            orbiting = false;
        }
    }

    private void FixedUpdate()
    {
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
                if (nearestTarget == default || !(nearestTarget.CompareTag("Environment Object") || nearestTarget.CompareTag("Enemy"))) return;

                telekenesisLightHandle = InstantiatePrefabByKey(
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

    private IEnumerator ShakeObject(Rigidbody2D rb, float shakeDuration, float maxDistance, float frequency, float intensity = 1f, float intensityRampingFactor = 0f)
    {
        Vector2 initialPosition = rb.position;
        float startTime = Time.time;

        while (Time.time < startTime + shakeDuration)
        {
            float elapsedTime = Time.time - startTime;

            float displacement = Mathf.Sin(elapsedTime * frequency * Mathf.PI * 2f) * maxDistance * intensity;
            // TODO: make this work in 2D only instead of generating sinusoid in 3D and then projecting
            float randomDirectionAngle = Random.Range(0f, 360f);
            Vector2 newPosition = initialPosition + (Vector2)(Quaternion.AngleAxis(randomDirectionAngle, Const.rotationAxis) * new Vector3(displacement, displacement, 0f));
            // TODO: RbSmoothMovePosition
            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator ShakeAndThrowObject(OrbitingObject thrownObject, Vector2 throwDirection)
    {
        // keep reference to handle so we can clean up the light prefab later
        AsyncOperationHandle<GameObject> handleForCleanup = thrownObject.handle;
        // remove from orbiting objects list so the object does not continue trying to orbit
        orbitingObjects.RemoveAt(0);

        Rigidbody2D thrownRb = thrownObject.obj.GetComponent<Rigidbody2D>();

        // find position off to the left or right of the player to place the object at before throwing
        // TODO: check for collisions with other objects (i.e., walls), if so, move less far out
        Vector2 leftLaunchPosition = transform.position + (Quaternion.AngleAxis(75, Const.rotationAxis) * throwDirection * 2.5f);
        Vector2 rightLaunchPosition = transform.position + (Quaternion.AngleAxis(-75, Const.rotationAxis) * throwDirection * 2.5f);
        
        Vector2 closestLaunchPos = Vector2.Distance(thrownObject.obj.transform.position, leftLaunchPosition)
            < Vector2.Distance(thrownObject.obj.transform.position, rightLaunchPosition) 
            ? leftLaunchPosition : rightLaunchPosition;

        float moveDuration = 0.2f;
        StartCoroutine(RbSmoothMovePosition(thrownRb, closestLaunchPos, moveDuration));

        // wait until object is in position OR until 1 second has passed
        float startTime = Time.time;
        while (Time.time < startTime + moveDuration && Mathf.Abs(((Vector2)thrownRb.transform.position - closestLaunchPos).magnitude) > 0.1f)
        {
            yield return new WaitForFixedUpdate();
        }

        // shake object in place before throwing with a building intensity
        StartCoroutine(ShakeObject(rb: thrownRb, shakeDuration: 1f, maxDistance: 0.1f, frequency: 10f));

        // clean up orbiting object's light prefab
        Addressables.ReleaseInstance(handleForCleanup);

        //! throw object (temp)
        thrownRb.AddForce(throwDirection * stats.orbitThrowForce, ForceMode2D.Impulse);
    }

    public void ThrowOrbitingObject(CallbackContext context)
    {
        if (context.performed)
        {
            if (orbitingObjects.Count == 0) return;

            //! should it be least recent object? Closest to mouse? unsure... playtest/see what other games do
            Vector2 throwDirection = (GetProjectedMousePos() - (Vector2)orbitingObjects[0].obj.transform.position).normalized;
            StartCoroutine(ShakeAndThrowObject(orbitingObjects[0], throwDirection));

            // TODO: move object out to the side, maybe shake it a little, then throw for better visual clarity
            //leastRecentOrbitingObject.GetComponent<Rigidbody2D>().AddForce(throwDirection * stats.orbitThrowForce, ForceMode2D.Impulse);
        }
    }
}
