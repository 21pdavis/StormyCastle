using UnityEngine;

public class UnitStats : ScriptableObject
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    // Risk of Rain style damage, each unit has an assigned "damage" value and other damages are based on percentages of that
    public const int damage = 5;

    public void ModifyHealth(int delta)
    {
        // Ensure that the current health doesn't exceed the maximum
        currentHealth = Mathf.Clamp(currentHealth + delta, 0, maxHealth);
    }
}
