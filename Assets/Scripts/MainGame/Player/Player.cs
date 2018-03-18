using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EffectManager))]
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
    int _actionsRemaining;

    #endregion

    #region Public Properties

    /// <summary>
    /// The ID of the current player.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// The <see cref="Unit"/> prefab object attached to this player.
    /// </summary>
    public GameObject UnitPrefab => m_unitPrefab;

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
    /// The units the player owns.
    /// </summary>
    /// <value>The units.</value>
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
    public int ActionsRemaining => _actionsRemaining;

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
        _actionsRemaining = Stats.Actions;
        SpawnUnits();
        OnTurnStart?.Invoke(this, new EventArgs());
    }

    public virtual void ProcessSectorClick(Sector clickedSector)
    { }

    public virtual void EndTurn()
    {
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
            actionsRemaining = _actionsRemaining
        };
    }

    public virtual void RestoreMemento(SerializablePlayer memento)
    {
        // kind is used for instantiation
        // id, color and gui are set on Init()
        _effects.RestoreMemento(memento.effectManager);
        _actionsRemaining = memento.actionsRemaining;
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
    /// Attempt to move the unit in the <c>from</c> sector to the
    /// <c>target</c> sector. If it is not empty, the follow is done:
    /// 
    /// If occupying unit is friendly, units are swapped.
    /// If occupying unit is an enemy unit, conflict occurs.
    /// 
    /// Upon conflict, if the current unit wins, it takes over the sector,
    /// destroying the enemy unit. If the enemy unit wins, the current
    /// unit is destroyed.
    /// </summary>
    /// <param name="from">The starting sector.</param>
    /// <param name="target">The target sector.</param>
    internal void AttemptMove(Sector from, Sector target)
    {
        // assert that we can actually do a move
        if (from.Unit?.Owner != this) // if unit is null or not owned, then crash i guess ¯\_(ツ)_/¯
            throw new InvalidOperationException();

        if (target.Unit == null || target.Unit.Owner == this)
        {
            // transfer units between sectors
            // if unit is null, is will be swapped as well
            from.TransferUnits(target);
        }
        else // the only other option is that target contains an enemy unit
        {
            // do attack
            // destroy loser
            // move from unit if it won
            bool won = from.Unit.Attack(target.Unit);
            if (won)
            {
                target.Unit.Kill(); // destroy enemy unit, making it null
                from.TransferUnits(target); // since target unit is now null,
            }
            else
                from.Unit.Kill(); // destroy current unit, making it null
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
        _actionsRemaining--;
        OnActionPerformed?.Invoke(this, new EventArgs());
        if (_actionsRemaining == 0)
            EndTurn();
    }

    #endregion

    #region Overrides

    public override string ToString() => string.Format("Player({0})", Id);

    #endregion
}