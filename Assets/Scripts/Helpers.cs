﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

static class Helpers
{
    public static void FlipSprite(Vector2 movement, Transform graphics)
    {
        // flip sprite
        if (movement.x < 0)
        {
            graphics.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (movement.x > 0)
        {
            graphics.localScale = new Vector3(1f, 1f, 1f);
        }
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

    public static void MeleeAttack(Transform attacker, UnitStats attackerStats, LayerMask targetLayers)
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

            // actually deal damage
            targetStats.TakeDamage(damageDealt);
            Debug.Log($"Hit {target.name}, its health was {targetStats.currentHealth + damageDealt} and is now {targetStats.currentHealth}");
        }
    }

    public static Vector3 Rotate90(Vector3 vector)
    {
        return new Vector3(-vector.y, vector.x, vector.z);
    }
}