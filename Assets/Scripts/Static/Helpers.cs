﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;

static class Helpers
{
    public static void FlipSprite(Vector2 direction, Transform graphics, bool flipSpriteOnly=false)
    {
        // flip sprite
        if (direction.x < 0)
        {
            if (flipSpriteOnly)
            {
                graphics.gameObject.GetComponent<SpriteRenderer>().flipX = true;
            }
            else
            {
                graphics.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        else if (direction.x > 0)
        {
            if (flipSpriteOnly)
            {
                graphics.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }
            else
            {
                graphics.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

    public static AsyncOperationHandle<GameObject> InstantiatePrefabByKey(string key, Vector3 position, Quaternion rotation, Transform parent=null)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(
            key: key,
            position: position,
            rotation: rotation
        );

        handle.Completed += (obj) =>
        {
            // set parent after completion to avoid positional offset
            if (parent)
            {
                obj.Result.transform.SetParent(parent);
                obj.Result.transform.position = parent.transform.position;
            }
        };

        return handle;
    }

    /// <summary>
    /// Gets the closest to mouse.
    /// </summary>
    /// <param name="detectionRadius">The detection radius.</param>
    /// <param name="targetLayers">The target layers.</param>
    /// <returns>Returns the closest target or Collider2D's default if none detected</returns>
    public static Collider2D GetNearestToMouse(float detectionRadius, LayerMask targetLayers)
    {
        Vector2 mousePos = GetProjectedMousePos();
        Collider2D[] targets = Physics2D.OverlapCircleAll(mousePos, detectionRadius, targetLayers);
        return targets.OrderBy(x => Vector2.Distance(x.transform.position, mousePos)).FirstOrDefault();
    }

    /// <summary>
    /// takes two normalized Vector2's and returns the degree angle between them on the unit circle
    /// </summary>
    /// <param name="v1">The first normalized vector</param>
    /// <param name="v2">The second normalized vector</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Vectors passed to DistanceOnUnitCircle must be normalized</exception>
    public static float DistanceOnUnitCircle(Vector2 v1, Vector2 v2)
    {
        if (v1.magnitude != 1 || v2.magnitude != 1)
        {
            throw new System.ArgumentException("Vectors passed to DistanceOnUnitCircle must be normalized");
        }

        // The dot product of two normalized vectors is equal to the cosine of the angle between them
        return Mathf.Rad2Deg * Mathf.Acos(Vector2.Dot(v1, v2));
    }

    public static IEnumerator FadeInSound(AudioSource source, float fadeDuration=15f, float fadeStep=0.005f)
    {
        float targetVolume = source.volume;
        float elapsedTime = 0f;
        float initialVolume = 0f;
        source.volume = initialVolume;
        source.Play();
        while (elapsedTime < fadeDuration)
        {
            source.volume = Mathf.Lerp(initialVolume, targetVolume, elapsedTime / fadeDuration);
            elapsedTime += fadeStep;
            yield return new WaitForSecondsRealtime(fadeStep);
        }

        source.volume = targetVolume;
    }

    public static IEnumerator RbSmoothMovePosition(Rigidbody2D rb, Vector2 targetPosition, float moveDuration)
    {
        Vector3 initialPosition = rb.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            // Lerp returns the point (elapsedTime / moveDuration)% between initialPosition and targetPosition
            rb.MovePosition(Vector2.Lerp(initialPosition, targetPosition, elapsedTime / moveDuration));
            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // ensure we end up at the target position
        rb.MovePosition(targetPosition);
    }

    public static void MeleeAttack(Transform attacker, UnitStats attackerStats, LayerMask targetLayers, float pushForce=0f)
    {
        // detect enemies in range of attack
        List<Collider2D> hitTargets = Physics2D.OverlapCircleAll(attacker.position, attackerStats.attackRange, targetLayers)
            .Where(collider =>
            {
                // only hit enemies in the direction of the attack in a semicircle
                if (attacker.localScale.x < 0)
                {
                    return collider.transform.position.x <= attacker.position.x;
                }
                return collider.transform.position.x > attacker.position.x;
            })
            .ToList();

        foreach (var target in hitTargets)
        {
            var targetStats = target.GetComponent<UnitStats>();

            // get player's damage value
            int damageDealt = attackerStats.damage;

            if (pushForce > 0f)
            {
                Vector2 pushDirection = (Vector2)target.transform.position - (Vector2)attacker.position;

                if (target.CompareTag("Player"))
                {
                    target.GetComponent<PlayerMovement>().Knockback(pushDirection.normalized * pushForce);
                }
            }

            // actually deal damage
            targetStats.TakeDamage(damageDealt);
            Debug.Log($"Hit {target.name}, its health was {targetStats.currentHealth + damageDealt} and is now {targetStats.currentHealth}");
        }
    }

    public static Vector3 Rotate90(Vector3 vector)
    {
        return new Vector3(-vector.y, vector.x, vector.z);
    }

    // TODO: TEMPORARY, really not efficient, but also not triggered enough for it to be too bad
    public static void SwitchMusic(string musicObjectName, AudioSource source)
    {
        // stop current music
        List<string> songNames = (new string[] { "Outside Music", "Castle Music", "Boss Music" })
            .Where(name => name.CompareTo(musicObjectName) != 0)
            .ToList();
        foreach (string songName in songNames)
        {
            GameObject obj = GameObject.Find(songName);
            if (obj != null)
            {
                if (obj.TryGetComponent<AudioSource>(out var musicSource))
                {
                    musicSource.Stop();
                }
            }
        }

        // play new music
        source.Play();
    }

    public static int GetRandomExcluding(int minInclusive, int maxExclusive, int excludedNumber)
    {
        int randomValue;

        if (excludedNumber >= minInclusive && excludedNumber < maxExclusive)
        {
            // TODO: bad? slow?
            do
            {
                randomValue = Random.Range(minInclusive, maxExclusive);
            } while (randomValue == excludedNumber);
        }
        else
        {
            randomValue = Random.Range(minInclusive, maxExclusive);
        }
        

        return randomValue;
    }

    public static Vector2 GetProjectedMousePos()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        return new Vector2(mousePos.x, mousePos.y);
    }
}