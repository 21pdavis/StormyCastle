using UnityEngine;

public class UnitStats : ScriptableObject
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public void ModifyHealth(int delta)
    {
        // Ensure that the current health doesn't exceed the maximum
        currentHealth = Mathf.Clamp(currentHealth + delta, 0, maxHealth);
    }
}
