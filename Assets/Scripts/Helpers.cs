using UnityEngine;

static class Helpers
{
    public static void FlipSprite(Vector2 movement, Transform graphics)
    {
        // flip sprite
        if (movement.x <= 0.01)
        {
            graphics.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (movement.x >= 0.01)
        {
            graphics.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}