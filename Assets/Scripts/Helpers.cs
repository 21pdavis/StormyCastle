﻿using UnityEngine;

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
    /// takes two normalized Vector2's and returns the angle between them on the unit circle
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
}