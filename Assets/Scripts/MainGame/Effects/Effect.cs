using System;
using System.Runtime.Serialization;

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

    #endregion

    #region Override Properties

    /// <summary>
    /// The description that this effect will display on the card it
    /// is on.
    /// </summary>
    public virtual string CardDescription { get; } = null;

    // the following effects are all mirrored from EffectManager
    // with the only difference being that they are nullable.
    // nulled properties are assumed to not be part of the effect
    // are are ignored.

    public virtual int? AttackBonus { get; } = null;

    public virtual int? DefenceBonus { get; } = null;

    public virtual int? ActionBonus { get; } = null;

    public virtual bool? Traversable { get; } = null;

    public virtual int? MoveRangeBonus { get; } = null;

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

    public virtual void ProcessUnitDeath(object sender, EventArgs e)
    { }

    #endregion

    #region Abstract Methods

    /// <summary>
    /// Returns the available selection that the effect can be applied to.
    /// </summary>
    /// <returns>The available selection the effect can be applied to.</returns>
    /// <param name="game">The current game object.</param>
    public abstract EffectAvailableSelection AvailableSelection(Game game);

    /// <summary>
    /// Called when the effect is removed from the manager.
    /// </summary>
    public virtual void ProcessEffectRemove()
    { }

    #endregion

    #region Helper Methods

    void Init(EffectManager manager, int id)
    {
        if (_manager != null)
            throw new InvalidOperationException();
        _manager = manager;
        _id = id;
    }

    void SwitchObjectType(object obj,
                          Action<Player> playerAction,
                          Action<Sector> sectorAction,
                          Action<Unit> unitAction)
    {
        if (obj as Player != null)
            playerAction((Player)obj);
        else if (obj as Sector != null)
            sectorAction((Sector)obj);
        else if (obj as Unit != null)
            unitAction((Unit)obj);
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

    protected virtual void RestorePlayer(Player player)
    { }

    protected virtual void RestoreSector(Sector sector)
    { }

    protected virtual void RestoreUnit(Unit unit)
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

    protected virtual void ApplyToPlayer(Player player)
    {
        throw new InvalidOperationException("Cannot apply effect to Player");
    }

    protected virtual void ApplyToSector(Sector sector)
    {
        throw new InvalidOperationException("Cannot apply effect to Sector");
    }

    protected virtual void ApplyToUnit(Unit unit)
    {
        throw new InvalidOperationException("Cannot apply effect to Unit");
    }

    /// <summary>
    /// Removes this effect from the effect manager it's in.
    /// </summary>
    protected void RemoveSelf() => Manager.RemoveEffect(this);

    #endregion
}
