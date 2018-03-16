using System;
using UnityEngine;

[RequireComponent(typeof(EffectManager))]
public class Unit : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Material m_level1Material;
    [SerializeField] Material m_level2Material;
    [SerializeField] Material m_level3Material;
    [SerializeField] Material m_level4Material;
    [SerializeField] Material m_level5Material;

    #endregion

    #region Private Fields

    EffectManager _effects;
    int? _owner;
    int _sector;
    int _level;

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
        set { _owner = value?.Id; }
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
    public int Level => _level;

    /// <summary>
    /// The colour of the unit
    /// </summary>
    public Color Color => Owner.Color;

    #endregion

    #region Events

    /// <summary>
    /// Raised when the unit dies.
    /// </summary>
    public event EventHandler OnDeath;

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
        // set the owner, level, and color of the unit
        Owner = player;
        _level = 1;

        // set the material color to the player color
        GetComponent<Renderer>().material.color = Color;
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _effects = GetComponent<EffectManager>();
        _effects.Init(this);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Increase this units level and update the unit model to display the new level.
    /// Leveling up is capped at level 5.
    /// </summary>
	public void LevelUp()
    {
        if (Level < Stats.LevelCap)
        {
            // increase level
            _level++;
            UpdateUnitMaterial();
        }
    }

    public void UpdateUnitMaterial()
    {
        switch (_level)
        {
            case 2:
                gameObject.GetComponent<MeshRenderer>().material = m_level2Material;
                break;
            case 3:
                gameObject.GetComponent<MeshRenderer>().material = m_level3Material;
                break;
            case 4:
                gameObject.GetComponent<MeshRenderer>().material = m_level4Material;
                break;
            case 5:
                gameObject.GetComponent<MeshRenderer>().material = m_level5Material;
                break;
            default:
                gameObject.GetComponent<MeshRenderer>().material = m_level1Material;
                break;
        }

        // set material color to match owner color
        GetComponent<Renderer>().material.color = Color;
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
    public bool Attack(Unit other)
    {
        // diff = +ve: attacker advantage 
        // diff = -ve or 0: defender advantage
        int diff =
            (Level + Stats.Attack + Owner.Stats.Attack) - // attacker value
            (other.Level + Stats.Defence + other.Owner.Stats.Defence); // defender value

        if (UnityEngine.Random.Range(0f, 1f) < CalculateAttackUncertainty(diff))
            // disadvantaged side won
            // if diff <= 0, defender had advantage, attack has disadvantage
            return diff <= 0;
        // advantaged side won
        // if diff > 0, attacker had advantage, defender has disadvantage
        return diff > 0;
    }

    /// <summary>
    /// Calculates the attack uncertainty.
    /// Uses the function explained here:
    /// https://www.desmos.com/calculator/taaldm6iod.
    /// </summary>
    /// <returns>The attack uncertainty.</returns>
    /// <param name="x">The difference value.</param>
    float CalculateAttackUncertainty(int x)
    {
        // function parameters
        const float z = 2.5f;
        const float a = 1f / 12.5f;
        const float b = 0.4f;
        const float c = 2f;

        // calculation
        return a * (Mathf.Pow(z, (-b * Mathf.Abs(x)) + c));
    }

    public void Kill()
    {
        Destroy(gameObject);
        OnDeath?.Invoke(this, new EventArgs());
    }

    #endregion
}

