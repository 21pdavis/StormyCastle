using UnityEngine;

using static Helpers;

public class PlayerStats : UnitStats
{
    [SerializeField] private ManaBar manaBar;
    [SerializeField] private GameObject _gameOverText;

    [SerializeField] private float _interactRange;
    [SerializeField] private int _maxMana;
    [SerializeField] private int _currentMana;
    [SerializeField] private float _manaRegenInterval;
    [SerializeField] private int _healAmount;
    [SerializeField] private int _healCost;
    [SerializeField] private float _healInterval;
    [SerializeField] private GameObject _heldObject;
    [SerializeField] private float _telekinesisRadius;
    [SerializeField] private float _orbitRadius;
    [SerializeField] private float _orbitSpeed;
    [SerializeField] private float _orbitThrowForce;

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

    public GameObject heldObject
    {
        get { return _heldObject; }
        set { _heldObject = value; }
    }

    public float telekinesisRadius
    {
        get { return _telekinesisRadius; }
        set { _telekinesisRadius = value; }
    }

    public float orbitRadius
    {
        get { return _orbitRadius; }
        set { _orbitRadius = value; }
    }

    public float orbitSpeed
    {
        get { return _orbitSpeed; }
        set { _orbitSpeed = value; }
    }

    public float orbitThrowForce
    {
        get { return _orbitThrowForce; }
        set { _orbitThrowForce = value; }
    }

    public GameObject gameOverText
    {
        get { return _gameOverText; }
        set { _gameOverText = value; }
    }

    protected override void Start()
    {
        base.Start();
        manaBar.SetMaxMana(maxMana);
        manaBar.SetMana(currentMana);
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

    public void Win()
    {
        StartCoroutine(WinSequence());
    }

    protected override void Die()
    {
        GetComponent<PlayerMovement>().playerFreezeOverride = true;
        StartCoroutine(DieSequence());
    }
}