using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(EffectManager))]
public class Sector : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] Sector[] m_adjacentSectors;
    [SerializeField] Landmark m_landmark;

    #endregion

    #region Private fields

    const float DefaultHighlightAmount = 0.2f;

    int _id;
    Transform _unitStore;
    EffectManager _effects;
    bool _pvc; // default: false
    Unit _unit;
    int? _owner;
    bool _highlighed; // default: false

    #endregion

    #region Public properties

    /// <summary>
    /// The ID of the current sector.
    /// </summary>
    public int Id => _id;

    /// <summary>
    /// The stats of the current sector.
    /// </summary>
    public EffectManager Stats => _effects;

    /// <summary>
    /// Whether the PVC is contained in this sector.
    /// </summary>
    public bool HasPVC
    {
        get { return _pvc; }
        set { _pvc = value; }
    }

    /// <summary>
    /// Whether the sector allows the PVC to be hidden on it.
    /// </summary>
    public bool AllowPVC => Landmark == null && Unit == null;

    /// <summary>
    /// The Unit occupying this sector.
    /// </summary>
    public Unit Unit
    {
        get { return _unit; }
        set
        {
            if (_unit != null)
                _unit.OnDeath -= Unit_OnDeath;
            
            if (value != null)
            {
                Sector prev = _unit?.Sector; // store previous sector

                _unit = value;
                _unit.transform.parent = transform; //.SetParent(transform, false);
                //transform.position = targetTransform.position;

                // if the target sector belonged to a different 
                // player than the unit, capture it and level up
                if (Owner != _unit.Owner)
                {
                    Debug.Log("capuring sector");

                    Owner = _unit.Owner;
                    _unit.LevelUp();
                }

                OnUnitMove?.Invoke(_unit, new UpdateEventArgs<Sector>(prev, Unit.Sector));
                _unit.OnDeath += Unit_OnDeath;
            }
            else
                _unit = null;
        }
    }

    /// <summary>
    /// The player who owns the sector.
    /// </summary>
    public Player Owner
    {
        get { return _owner.HasValue ? Game.Instance.Players[_owner.Value] : null; }
        set
        {
            Player prev = Owner;
            _owner = value?.Id;

            // set sector color to the color of the given player
            // or gray if null
            if (_owner.HasValue)
                gameObject.GetComponent<Renderer>().material.color = Owner.Color;
            else
                gameObject.GetComponent<Renderer>().material.color = Color.gray;


            if (prev != Owner)
                OnCaptured?.Invoke(this, new UpdateEventArgs<Player>(prev, Owner));
        }
    }

    /// <summary>
    /// The neighbouring sectors.
    /// </summary>
    public IEnumerable<Sector> AdjacentSectors => m_adjacentSectors.Where(s => s.Stats.Traversable);

    /// <summary>
    /// The landmark on this sector.
    /// </summary>
    public Landmark Landmark => m_landmark;

    /// <summary>
    /// Gets or sets the highlight on the current sector.
    /// </summary>
    public bool Highlighted
    {
        get { return _highlighed; }
        set
        {
            if (value != _highlighed)
            {
                Renderer rend = GetComponent<Renderer>();
                Color currentColor = rend.material.color;
                Color offset = new Color(DefaultHighlightAmount, DefaultHighlightAmount, DefaultHighlightAmount);
                Color newColor;
                if (value && !_highlighed)
                    newColor = currentColor + offset;
                else if (!value && _highlighed)
                    newColor = currentColor - offset;
                else
                    throw new InvalidOperationException();
                rend.material.color = newColor;
            }
            _highlighed = value;
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Raised when the sector is clicked.
    /// </summary>
    public event EventHandler OnClick;

    /// <summary>
    /// Raised when the sector is captured (i.e. when ownership changes).
    /// </summary>
    public event EventHandler<UpdateEventArgs<Player>> OnCaptured;

    /// <summary>
    /// Raised when a unit moves sector.
    /// </summary>
    public event EventHandler<UpdateEventArgs<Sector>> OnUnitMove;

    /// <summary>
    /// Raised when the unit in the sector dies.
    /// This event is a pure pass-through to reduce the need to dynamically
    /// re-bind events on unit create/move/death.
    /// </summary>
    public event EventHandler OnUnitDeath;

    #endregion

    #region Initialization

    public void Init(int id)
    {
        _id = id;
    }

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        _unitStore = gameObject.transform.Find("Units");
        _effects = GetComponent<EffectManager>();
        _effects.Init(this);
    }

    void Start()
    {
        Owner = null;
        _unit = null;
    }

    internal void OnMouseUpAsButton() => OnClick?.Invoke(this, new EventArgs());

    #endregion

    #region Handlers

    void Unit_OnDeath(object sender, EventArgs e)
    {
        OnUnitDeath?.Invoke(sender, e);
        Unit.OnDeath -= Unit_OnDeath;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Set the highlight of all the adjacent sectors.
    /// </summary>
    [Obsolete("Will remove in favour of range-based highlighting")]
    public void ApplyHighlightAdjacent(bool highlight)
    {
        foreach (Sector adjacentSector in AdjacentSectors)
            adjacentSector.Highlighted = highlight;
    }

    /// <summary>
    /// Swaps the units of the current and other sector.
    /// </summary>
    /// <param name="other">The sector to transfer the unit with.</param>
    public void TransferUnits(Sector other)
    {
        Unit tmp = Unit;
        Unit = other.Unit;
        other.Unit = tmp;
    }

    #endregion
}