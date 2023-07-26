using UnityEngine;

public class PlayerStats : UnitStats
{
    [SerializeField] private ManaBar manaBar;

    [SerializeField] private float _interactRange;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _currentMana;
    [SerializeField] private float _manaRegenInterval;
    [SerializeField] private int _healAmount;
    [SerializeField] private int _healCost;
    [SerializeField] private float _healInterval;

    private float lastRegenedMana;

    /// <summary>
    /// The heal particle prefab
    /// </summary>
    public GameObject healParticles;

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

    public float interactRange
    {
        get { return _interactRange; }
        set { _interactRange = value; }
    }

    public int healAmount
    {
        get { return _healAmount; }
        set { _healAmount = value; }
    }

    public int healCost
    {
        get { return _healCost; }
        set { _healCost = value; }
    }

    public float healInterval
    {
        get { return _healInterval; }
        set { _healInterval = value; }
    }

    public float manaRegenInterval
    {
        get { return _manaRegenInterval; }
        set { _manaRegenInterval = value; }
    }

    private void Update()
    {
        if (Time.time > lastRegenedMana + manaRegenInterval)
        {
            lastRegenedMana = Time.time;
            GainMana(1);
        }
    }

    private void ModifyMana(int delta)
    {
        // Ensure that the current mana doesn't exceed the maximum
        currentMana = Mathf.Clamp(currentMana + delta, 0, maxMana);
    }

    public void SpendMana(int amount)
    {
        ModifyMana(-amount);
        manaBar.SetMana(currentMana);
    }

    public void GainMana(int amount)
    {
        ModifyMana(amount);
        manaBar.SetMana(currentMana);
    }

    protected override void Die()
    {
        // TODO
    }
}