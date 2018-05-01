using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EffectManager))]
[RequireComponent(typeof(CardManager))]
public abstract class Player : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] GameObject m_unitPrefab;

    #endregion

    #region Private Fields

    int _id;
    PlayerUI _gui;
    Color _color;
    EffectManager _effects;
    CardManager _cardManager;
    int _actionsPerformed;
    bool _hasHadTurn;

    #endregion

    #region Public Properties

    /// <summary>
    /// The ID of the current player.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// The <see cref="PlayerUI"/> object attached to this player.
    /// </summary>
    public PlayerUI Gui => _gui;

    /// <summary>
    /// The player's color.
    /// </summary>
    public Color Color => _color;

    /// <summary>
    /// The stats of the current player.
    /// </summary>
    public EffectManager Stats => _effects;

    /// <summary>
    /// The cards of the current player.
    /// </summary>
    public CardManager Cards => _cardManager;

    /// <summary>
    /// The units the player owns.
    /// </summary>
    public IEnumerable<Unit> Units => Game.Instance.Map.Sectors.Select(s => s.Unit).Where(u => u != null && u.Owner == this);

    /// <summary>
    /// Whether the player owns any units.
    /// </summary>
    public bool HasUnits => Units.Any();

    /// <summary>
    /// The sectors owned by this player.
    /// </summary>
    public IEnumerable<Sector> OwnedSectors => Game.Instance.Map.Sectors.Where(s => s.Owner == this);

    /// <summary>
    /// The landmarks that the player owns.
    /// </summary>
    public IEnumerable<Sector> OwnedLandmarkSectors => OwnedSectors.Intersect(Game.Instance.Map.LandmarkedSectors);

    /// <summary>
    /// Returns true if any of the sectors the player owns contains a landmark.
    /// </summary>
    public bool OwnsLandmark => OwnedLandmarkSectors.Any();

    /// <summary>
    /// Checks if the player has any units or if they own any landmarks.
    /// If they have neither then they have been eliminated.
    /// </summary>
    public bool IsEliminated => !HasUnits && !OwnsLandmark;

    /// <summary>
    /// The number of actions remaining for the current player.
    /// </summary>
    public int ActionsPerformed
    {
        get { return _actionsPerformed; }
        set { _actionsPerformed = value; }
    }

    /// <summary>
    /// Whether the player can perform an action.
    /// </summary>
    public bool CanPerformActions => ActionsPerformed < Stats.Actions;

    /// <summary>
    /// Whether the player has had at least 1 turn.
    /// </summary>
    public bool HasHadTurn => _hasHadTurn;

    #endregion

    #region Abstract Properties

    public abstract PlayerKind Kind { get; }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the player's turn starts.
    /// </summary>
    public event EventHandler OnTurnStart;

    /// <summary>
    /// Raised when the player's turn ends.
    /// </summary>
    public event EventHandler OnTurnEnd;

    /// <summary>
    /// Raised when an action has been performed.
    /// Using cards does not raise this event.
    /// </summary>
    public event EventHandler OnActionPerformed;

    #endregion

    #region Base Methods

    public virtual void ProcessTurnStart()
    {
        ActionsPerformed = 0;
        LevelUpLandmarkedUnits();
        SpawnUnits();
        OnTurnStart?.Invoke(this, new EventArgs());
    }

    public virtual void ProcessSectorClick(Sector clickedSector)
    { }

    public virtual void EndTurn()
    {
        _hasHadTurn = true;
        OnTurnEnd?.Invoke(this, new EventArgs());
    }

    #endregion

    #region Initialization

    public void Init(int id, Color color, PlayerUI gui)
    {
        _id = id;
        _color = color;
        _gui = gui;
    }

    #endregion

    #region Serialization

    public virtual SerializablePlayer CreateMemento()
    {
        return new SerializablePlayer
        {
            kind = Kind,
            id = _id,
            color = _color,
            effectManager = _effects.CreateMemento(),
            cards = _cardManager.CreateMemento(),
            actionsPerformed = _actionsPerformed,
            hasHadTurn = _hasHadTurn
        };
    }

    public virtual void RestoreMemento(SerializablePlayer memento)
    {
        // kind is used for instantiation
        // id, color and gui are set on Init()
        OnDisable(); // stop events from being picked up
        _effects.RemoveEffect<EffectImpl.LandmarkWrapperEffect>();
        _effects.RestoreMemento(memento.effectManager);
        OnEnable(); // start events being picked up
        _actionsPerformed = memento.actionsPerformed;
        _cardManager.RestoreMemento(memento.cards);
        _hasHadTurn = memento.hasHadTurn;
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _effects = GetComponent<EffectManager>();
        _effects.Init(this);
        if (!_effects.HasEffect<EffectImpl.LandmarkWrapperEffect>())
            _effects.ApplyEffect(new EffectImpl.LandmarkWrapperEffect());
        _cardManager = GetComponent<CardManager>();
    }

    void OnEnable()
    {
        Stats.OnEffectAdd += EffectManager_OnEffectChange;
        Stats.OnEffectRemove += EffectManager_OnEffectChange;
    }

    void OnDisable()
    {
        Stats.OnEffectAdd -= EffectManager_OnEffectChange;
        Stats.OnEffectRemove -= EffectManager_OnEffectChange;
    }

    #endregion

    #region Handlers

    void EffectManager_OnEffectChange(object sender, EventArgs e) => Game.Instance.UpdateGUI();

    #endregion

    #region Helper Methods

    /// <summary>
    /// Attempt to move the unit in the <c>from</c> sector to the
    /// <c>target</c> sector. If it is not empty, the follow is done:
    /// 
    /// If occupying unit is friendly, units are swapped.
    /// If occupying unit is an enemy unit, conflict occurs.
    /// 
    /// Upon conflict, the current unit attacks the enemy unit. If the attack
    /// kills the enemy unit, then the current unit moves into the sector.
    /// </summary>
    /// <param name="from">The starting sector.</param>
    /// <param name="target">The target sector.</param>
    internal void AttemptMove(Sector from, Sector target)
    {
        // ensure that we have an action available
        if (!CanPerformActions)
            throw new InvalidOperationException();

        // assert that we can actually do a move
        if (from.Unit?.Owner != this) // if unit is null or not owned, then crash i guess ¯\_(ツ)_/¯
            throw new InvalidOperationException();

        if (target.Unit == null || target.Unit.Owner == this)
        {
            SoundManager.Instance.PlaySingle(Sound.UnitMoveSound);
            // transfer units between sectors
            // if unit is null, is will be swapped as well
            from.TransferUnits(target);
        }
        else // the only other option is that target contains an enemy unit
        {
            // do attack
            // reduce levels from target
            // move from unit if target was destroyed
            from.Unit.Attack(target.Unit);

            if (target.Unit == null) // if unit was destroyed, it will now equal null
                from.TransferUnits(target); // since target unit is now null,
        }

        // whenever a move it attempted an action is consumed
        ConsumeAction();
    }

    /// <summary>
    /// Spawns a unit at each unoccupied sector containing a landmark owned by this player.
    /// </summary>
    public void SpawnUnits()
    {
        foreach (Sector sector in OwnedLandmarkSectors.Where(s => s.Unit == null))
            SpawnUnitAt(sector);
    }

    /// <summary>
    /// Levels up all owned units on landmarks.
    /// </summary>
    public void LevelUpLandmarkedUnits()
    {
        foreach (Unit unit in OwnedLandmarkSectors.Select(s => s.Unit).Where(u => u != null))
            unit.LevelUp();
    }

    /// <summary>
    /// Spawns a unit at the given sector.
    /// </summary>
    /// <param name="sector">The sector to spawn the unit in.</param>
    public void SpawnUnitAt(Sector sector)
    {
        Unit unit = Instantiate(m_unitPrefab).GetComponent<Unit>();
        unit.Init(this);
        sector.Unit = unit;
    }

    /// <summary>
    /// Consumes an action.
    /// </summary>
    protected void ConsumeAction()
    {
        ActionsPerformed++;
        OnActionPerformed?.Invoke(this, new EventArgs());
    }

    public void AssignRandomCard() => Cards.AddCards(CardFactory.GetRandomEffect(CardTier.Tier1));

    #endregion

    #region Overrides

    public override string ToString() => string.Format("Player {0}", Id + 1);

    #endregion
}
