using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EffectManager))]
public class Unit : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Text m_statText;
    [SerializeField] Text m_levelText;
    [SerializeField] GameObject m_undergrad;
    [SerializeField] GameObject m_postgrad;
    [SerializeField] Renderer[] m_renderers;

    #endregion

    #region Private Fields

    const string StatsFormat = "{0}/{1}";

    EffectManager _effects;
    int? _owner;
    int _sector;
    int _level;
    bool _usingPostgrad;
    bool _destroyed;

    #endregion

    #region Public Properties

    /// <summary>
    /// The stats of the current unit.
    /// </summary>
    public EffectManager Stats => _effects;

    /// <summary>
    /// Player that owns this unit
    /// </summary>
    public Player Owner
    {
        get { return _owner.HasValue ? Game.Instance.Players[_owner.Value] : null; }
        private set
        {
            EventOwnerBinder(false);
            _owner = value?.Id;
            EventOwnerBinder(true);

            // set the material color to the player color
            foreach (Renderer rend in m_renderers)
                rend.material.color = Color;
        }
    }

    /// <summary>
    /// The Sector the unit is occupying
    /// </summary>
    public Sector Sector
    {
        get { return Game.Instance.Map.Sectors[_sector]; }
        set { _sector = value.Id; }
    }

    /// <summary>
    /// The unit's level
    /// </summary>
    public int Level
    {
        get { return _level; }
        private set
        {
            if (value > Stats.LevelCap)
                throw new InvalidOperationException();
            _level = value;
            UpdateStats();
        }
    }

    /// <summary>
    /// Gets the unit's attack stat from its level.
    /// </summary>
    public int AttackFromLevel => Level;

    /// <summary>
    /// Gets the unit's defence stat from its level.
    /// </summary>
    public int DefenceFromLevel => Level;

    /// <summary>
    /// Gets the unit's total attack value.
    /// </summary>
    public int TotalAttack => AttackFromLevel + Stats.Attack + Owner.Stats.Attack;

    /// <summary>
    /// Gets the unit's total defence value.
    /// </summary>
    public int TotalDefence => DefenceFromLevel + Stats.Defence + Owner.Stats.Defence;

    /// <summary>
    /// The colour of the unit
    /// </summary>
    public Color Color => Owner.Color;

    /// <summary>
    /// Whether the unit should use the post-grad model or not.
    /// </summary>
    public bool UsePostGradModel
    {
        get { return _usingPostgrad; }
        set
        {
            _usingPostgrad = value;
            m_undergrad.SetActive(!_usingPostgrad);
            m_postgrad.SetActive(_usingPostgrad);
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the unit dies.
    /// </summary>
    public event EventHandler<EliminatedEventArgs> OnDeath;

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the unit on the passed sector and assigns it to the passed player.
    /// The unit is set to level 1.
    /// The unit's colour is set to the colour of the player that owns it.
    /// </summary>
    /// <param name="player">The player the unit belongs to.</param>
    public void Init(Player player)
    {
        // set the owner and level
        Owner = player;
        _level = 1;

        // update stats
        UpdateStats();
    }

    #endregion

    #region Serialization

    public SerializableUnit CreateMemento()
    {
        return new SerializableUnit
        {
            effectManager = _effects.CreateMemento(),
            owner = _owner,
            sector = _sector,
            level = Level,
            usingPostgrad = UsePostGradModel
        };
    }

    public void RestoreMemento(SerializableUnit memento)
    {
        _effects.RestoreMemento(memento.effectManager);
        _owner = memento.owner;
        _sector = memento.sector;
        Level = memento.level;
        UsePostGradModel = memento.usingPostgrad;
        UpdateStats();
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _effects = GetComponent<EffectManager>();
        _effects.Init(this);
    }

    void OnEnable()
    {
        Stats.OnEffectAdd += EffectManager_OnEffectChange;
        Stats.OnEffectRemove += EffectManager_OnEffectChange;
        Game.Instance.OnSectorCaptured += Game_OnSectorCapture;
        EventOwnerBinder(true);
    }

    void OnDisable()
    {
        Stats.OnEffectAdd -= EffectManager_OnEffectChange;
        Stats.OnEffectRemove -= EffectManager_OnEffectChange;
        Game.Instance.OnSectorCaptured -= Game_OnSectorCapture;
        EventOwnerBinder(false);
    }

    #endregion

    #region Handlers

    void EffectManager_OnEffectChange(object sender, EventArgs e) => UpdateStats();

    void Game_OnSectorCapture(object sender, UpdateEventArgs<Player> e) => UpdateStats();

    #endregion

    #region Helper Methods

    void DestroyedCheck()
    {
        if (_destroyed)
            throw new NullReferenceException("Unit already destoryed");
    }

    void UpdateStats()
    {
        m_levelText.text = _level.ToString();
        m_statText.text = string.Format(StatsFormat, TotalAttack, TotalDefence);
    }

    void EventOwnerBinder(bool bind)
    {
        if (Owner == null)
            return;
        if (bind)
        {
            Owner.Stats.OnEffectAdd += EffectManager_OnEffectChange;
            Owner.Stats.OnEffectRemove += EffectManager_OnEffectChange;
        }
        else
        {
            Owner.Stats.OnEffectAdd -= EffectManager_OnEffectChange;
            Owner.Stats.OnEffectRemove -= EffectManager_OnEffectChange;
        }
    }

    /// <summary>
    /// Increase this units level and update the unit model to display the new level.
    /// Leveling up is capped at level 5.
    /// </summary>
	public void LevelUp()
    {
        DestroyedCheck();
        if (Level < Stats.LevelCap)
            Level++;
    }
    /// <summary>
    /// Damage this unit by decreasing its level by the given amount.
    /// If a unit reaches level 0 or below, it is considered destroyed
    /// </summary>
    public void Damage(int amount, Player doneBy)
    {
        DestroyedCheck();
        Level -= amount;
        if (Level <= 0)
            Kill(doneBy);
        else
            SoundManager.Instance.PlaySingle(Sound.UnitAttackSound);
    }

    /// <summary>
    /// Returns the outcome of a combat encounter between two units.
    /// Takes into consideration the units levels and the attack/defence bonus of the player.
    /// 
    /// Close match leads to uncertain outcome (i.e. could go either way).
    /// If one unit + bonuses is significantly more powerful than another then they are very likely to win.
    /// </summary>
    /// <returns>The attack.</returns>
    /// <param name="other">The defending unit.</param>
    public void Attack(Unit other)
    {
        DestroyedCheck();
        // diff = +ve: attacker was stronger so potentially more levels will be reduced
        // diff = 0: defender was stronger so only a single level will be decreased
        int diff = Mathf.Max(TotalAttack - other.TotalDefence, 0);

        //apply damage to target
        other.Damage(CalculateAttackDamage(diff), Owner);

        //play a sound
    }

    /// <summary>
    /// Calculates the levels that are going to be reduced.
    /// Uses the function explained here:
    /// https://www.desmos.com/calculator/taaldm6iod.
    /// </summary>
    /// <returns>The attack damage.</returns>
    /// <param name="x">The difference value.</param>
    int CalculateAttackDamage(int x)
    {
        // function parameters
        const float z = 2.5f;
        const float a = 0.08f;
        const float b = 0.35f;
        const float c = 2f;

        // calculation
        return (int)(Mathf.Ceil(a * (Mathf.Pow(z, (b * x) + c))));
    }

    public void Kill(Player eliminator)
    {
        SoundManager.Instance.PlaySingle(Sound.UnitDieSound);
        DestroyedCheck();
        Destroy(gameObject);
        _destroyed = true;
        OnDeath?.Invoke(this, new EliminatedEventArgs(eliminator));
    }

    #endregion

    #region Operators

    // allows for immediate null checking

    static UnityEngine.Object[] ActualObjects(Unit x, Unit y)
    {
        return new UnityEngine.Object[]
        {
            x?._destroyed == true ? null : x,
            y?._destroyed == true ? null : y
        };
    }

    public static bool operator ==(Unit x, Unit y)
    {
        var actual = ActualObjects(x, y);
        return actual[0] == actual[1];
    }

    public static bool operator !=(Unit x, Unit y)
    {
        var actual = ActualObjects(x, y);
        return actual[0] != actual[1];
    }

    public override bool Equals(object other)
    {
        if (typeof(Unit).IsAssignableFrom(other.GetType()))
            return this == (Unit)other;
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #endregion
}
