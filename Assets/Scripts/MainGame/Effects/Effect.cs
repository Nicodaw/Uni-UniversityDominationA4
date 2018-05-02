using System;

/// <summary>
/// The base class for all effects.
/// If an effect property is null, then it doesn't count to the overall
/// value <see cref="T:EffectManager"/> returns.
/// </summary>
[Serializable]
public abstract class Effect
{
    #region Private Fields

    [NonSerialized]
    Player _appliedPlayer;
    [NonSerialized]
    Sector _appliedSector;
    [NonSerialized]
    Unit _appliedUnit;
    [NonSerialized]
    int _id;
    [NonSerialized]
    EffectManager _manager;

    #endregion

    #region Public Properties

    /// <summary>
    /// The ID of the current effect.
    /// </summary>
    public int Id => _id;

    #endregion

    #region Protected Properties

    /// <summary>
    /// The effect manager the Effect is in.
    /// </summary>
    protected EffectManager Manager => _manager;

    /// <summary>
    /// The player the effect was applied to.
    /// </summary>
    protected Player AppliedPlayer => _appliedPlayer;

    /// <summary>
    /// The sector the effect was applied to.
    /// </summary>
    protected Sector AppliedSector => _appliedSector;

    /// <summary>
    /// The unit the effect was applied to.
    /// </summary>
    protected Unit AppliedUnit => _appliedUnit;

    #endregion

    #region Override Properties

    /// <summary>
    /// The name of the effect that will be displayed on the card.
    /// </summary>
    /// <value>The name of the card.</value>
    public virtual string CardName { get; } = null;

    /// <summary>
    /// The description that this effect will display on the card.
    /// </summary>
    public virtual string CardDescription { get; } = null;

    /// <summary>
    /// The corner icon that this effect will display on the card.
    /// </summary>
    public virtual CardCornerIcon CardCornerIcon { get; }

    /// <summary>
    /// The border that this effect will display on the card.
    /// </summary>
    public virtual CardTier CardTier { get; }

    // the following effects are all mirrored from EffectManager
    // with the only difference being that they are nullable.
    // nulled properties are assumed to not be part of the effect
    // are are ignored.

    public virtual int? AttackBonus { get; } = null;

    public virtual int? DefenceBonus { get; } = null;

    public virtual int? ActionBonus { get; } = null;

    public virtual bool? Traversable { get; } = null;

    public virtual int? MoveRangeBonus { get; } = null;

    public virtual bool? CanMove { get; } = null;

    public virtual int? LevelCapBonus { get; } = null;

    #endregion

    #region Handlers

    // event handlers which are called when Game events are raised

    public virtual void ProcessPlayerTurnStart(object sender, EventArgs e)
    { }

    public virtual void ProcessPlayerTurnEnd(object sender, EventArgs e)
    { }

    public virtual void ProcessPlayerActionPerformed(object sender, EventArgs e)
    { }

    public virtual void ProcessPlayerEliminated(object sender, EliminatedEventArgs e)
    { }

    public virtual void ProcessSectorCaptured(object sender, UpdateEventArgs<Player> e)
    { }

    public virtual void ProcessUnitMove(object sender, UpdateEventArgs<Sector> e)
    { }

    public virtual void ProcessUnitDeath(object sender, EliminatedEventArgs e)
    { }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Returns the available selection that the effect can be applied to.
    /// </summary>
    /// <returns>The available selection the effect can be applied to.</returns>
    /// <param name="game">The current game object.</param>
    public virtual EffectAvailableSelection AvailableSelection(Game game)
    {
        throw new InvalidOperationException();
    }

    /// <summary>
    /// Called when the effect is removed from the manager.
    /// </summary>
    public virtual void ProcessEffectRemove()
    { }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Initializes the effect.
    /// </summary>
    /// <param name="manager">The manager the effect is in.</param>
    /// <param name="id">The ID of the effect in the manager.</param>
    void Init(EffectManager manager, int id)
    {
        if (_manager != null)
            throw new InvalidOperationException();
        _manager = manager;
        _id = id;
    }

    /// <summary>
    /// Calls a given function depending on the type of the object.
    /// </summary>
    /// <param name="obj">The object to switch with.</param>
    /// <param name="playerAction">
    /// Called if <paramref name="obj"/> is of type <see cref="T:Player"/>.
    /// </param>
    /// <param name="sectorAction">
    /// Called if <paramref name="obj"/> is of type <see cref="T:Sector"/>.
    /// </param>
    /// <param name="unitAction">
    /// Called if <paramref name="obj"/> is of type <see cref="T:Unit"/>.
    /// </param>
    void SwitchObjectType(object obj,
                          Action playerAction,
                          Action sectorAction,
                          Action unitAction)
    {
        if (obj as Player != null)
        {
            _appliedPlayer = (Player)obj;
            playerAction();
        }
        else if (obj as Sector != null)
        {
            _appliedSector = (Sector)obj;
            sectorAction();
        }
        else if (obj as Unit != null)
        {
            _appliedUnit = (Unit)obj;
            unitAction();
        }
        else
            throw new ArgumentException("Invalid type");
    }

    /// <summary>
    /// Restores the effect. Used to set the manager and object variables.
    /// </summary>
    /// <param name="obj">Object.</param>
    /// <param name="manager">Manager.</param>
    public void Restore(object obj, EffectManager manager, int id)
    {
        Init(manager, id);
        SwitchObjectType(obj, RestorePlayer, RestoreSector, RestoreUnit);
    }

    protected virtual void RestorePlayer()
    { }

    protected virtual void RestoreSector()
    { }

    protected virtual void RestoreUnit()
    { }

    /// <summary>
    /// Applies the effect to the given object.
    /// </summary>
    /// <param name="obj">The object to apply the effect to.</param>
    /// <param name="manager">The manager that is managing the effect.</param>
    /// <param name="id">The ID the effect will have.</param>
    /// <remarks>
    /// This function can take in any of Player, Sector or Unit.
    /// Any other object passed in will cause an exception.
    /// By default, effects will throw <see cref="T:InvalidOperationException"/>
    /// on all inputs, however concrete classes will override at least 1 method
    /// corresponding to the object that can be applied to.
    /// </remarks>
    public void ApplyTo(object obj, EffectManager manager, int id)
    {
        Init(manager, id);
        SwitchObjectType(obj, ApplyToPlayer, ApplyToSector, ApplyToUnit);
    }

    protected virtual void ApplyToPlayer()
    {
        throw new InvalidOperationException("Cannot apply effect to Player");
    }

    protected virtual void ApplyToSector()
    {
        throw new InvalidOperationException("Cannot apply effect to Sector");
    }

    protected virtual void ApplyToUnit()
    {
        throw new InvalidOperationException("Cannot apply effect to Unit");
    }

    /// <summary>
    /// Removes this effect from the effect manager it's in.
    /// </summary>
    protected void RemoveSelf() => Manager.RemoveEffect(this);

    #endregion
}
