using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Player/PlayerStats")]
public class PlayerStats : UnitStats
{
    private int _maxMana = 5;
    private int _currentMana = 5;

    public int maxMana
    {
        get { return _maxMana; }
        set { _maxMana = value; }
    }

    public int currentMana
    {
        get { return _currentMana; }
        set { _currentMana = value; }
    }

    public void ModifyMana(int delta)
    {
        // Ensure that the current mana doesn't exceed the maximum
        currentMana = Mathf.Clamp(currentMana + delta, 0, maxMana);
    }
}