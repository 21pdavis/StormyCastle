using UnityEngine;

public class PlayerStats : UnitStats
{
    [SerializeField] private ManaBar manaBar;

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

    private void ModifyMana(int delta)
    {
        // Ensure that the current mana doesn't exceed the maximum
        currentMana = Mathf.Clamp(currentMana + delta, 0, maxMana);
    }

    public void SpendMana(int amount)
    {
        ModifyMana(-amount);
    }

    public void GainMana(int amount)
    {
        ModifyMana(amount);
    }

    protected override void Die()
    {
        // TODO
    }
}