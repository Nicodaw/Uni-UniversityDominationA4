using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Manages all the effects and base stats of an object.
/// </summary>
public class EffectManager : MonoBehaviour
{
    #region Private Fields

    object _owner;
    int _nextEffectId;
    readonly Dictionary<int, Effect> _effects = new Dictionary<int, Effect>();

    #endregion

    #region Public Properties

    public int EffectCount => _effects.Count;

    #region Property Helpers

    /// <summary>
    /// Does the standard effect sum.
    /// </summary>
    /// <returns>The total value.</returns>
    /// <param name="val">The function used to get the value.</param>
    /// <param name="def">The default value.</param>
    int EffectSum(Func<Effect, int?> val, int def) =>
    // default value
    def +
    // sum up valid values
    _effects.Values
            .Where(ef => val(ef).HasValue)
            .Sum(ef => val(ef).Value);

    bool EffectAnd(Func<Effect, bool?> val, bool def = true)
    {
        bool final = def;
        foreach (bool b in _effects.Values.Where(ef => val(ef).HasValue).Select(ef => val(ef).Value))
            final &= b;
        return final;
    }

    #endregion

    public int Attack => EffectSum(ef => ef.AttackBonus, 0);

    public int Defence => EffectSum(ef => ef.DefenceBonus, 0);

    public int Actions => EffectSum(ef => ef.ActionBonus, 2);

    public bool Traversable => EffectAnd(ef => ef.Traversable);

    public int MoveRange => EffectSum(ef => ef.MoveRangeBonus, 1);

    public bool CanMove => EffectAnd(ef => ef.CanMove);

    public int LevelCap => EffectSum(ef => ef.LevelCapBonus, 5);

    #endregion

    #region Initialization

    public void Init(object owner)
    {
        Type ownerType = owner.GetType();
        if (typeof(Player).IsAssignableFrom(ownerType) ||
            typeof(Sector).IsAssignableFrom(ownerType) ||
            typeof(Unit).IsAssignableFrom(ownerType))
            _owner = owner;
        else
            throw new ArgumentException();
    }

    #endregion

    #region Serialization

    public SerializableEffectManager CreateMemento()
    {
        return new SerializableEffectManager
        {
            effects = _effects.Values.ToArray()
        };
    }

    public void RestoreMemento(SerializableEffectManager memento)
    {
        foreach (Effect effect in memento.effects)
        {
            effect.Restore(_owner, this, _nextEffectId);
            _effects.Add(_nextEffectId, effect);
            _nextEffectId++;
        }
    }

    #endregion

    #region MonoBehaviour

    void OnEnable()
    {
        Game.Instance.OnPlayerTurnStart += Game_OnPlayerTurnStart;
        Game.Instance.OnPlayerTurnEnd += Game_OnPlayerTurnEnd;
        Game.Instance.OnPlayerActionPerformed += Game_OnPlayerActionPerformed;
        Game.Instance.OnPlayerEliminated += Game_OnPlayerEliminated;
        Game.Instance.OnSectorCaptured += Game_OnSectorCaptured;
        Game.Instance.OnUnitMove += Game_OnUnitMove;
        Game.Instance.OnUnitDeath += Game_OnUnitDeath;
    }

    void OnDisable()
    {
        Game.Instance.OnPlayerTurnStart -= Game_OnPlayerTurnStart;
        Game.Instance.OnPlayerTurnEnd -= Game_OnPlayerTurnEnd;
        Game.Instance.OnPlayerActionPerformed -= Game_OnPlayerActionPerformed;
        Game.Instance.OnPlayerEliminated -= Game_OnPlayerEliminated;
        Game.Instance.OnSectorCaptured -= Game_OnSectorCaptured;
        Game.Instance.OnUnitMove -= Game_OnUnitMove;
        Game.Instance.OnUnitDeath -= Game_OnUnitDeath;
    }

    #endregion

    #region Handlers

    // handlers to apply game events to all managed effects

    void Game_OnPlayerTurnStart(object sender, EventArgs e) => InternalHandler(ef => ef.ProcessPlayerTurnStart(sender, e));

    void Game_OnPlayerTurnEnd(object sender, EventArgs e) => InternalHandler(ef => ef.ProcessPlayerTurnEnd(sender, e));

    void Game_OnPlayerActionPerformed(object sender, EventArgs e) => InternalHandler(ef => ef.ProcessPlayerActionPerformed(sender, e));

    void Game_OnPlayerEliminated(object sender, EliminatedEventArgs e) => InternalHandler(ef => ef.ProcessPlayerEliminated(sender, e));

    void Game_OnSectorCaptured(object sender, UpdateEventArgs<Player> e) => InternalHandler(ef => ef.ProcessSectorCaptured(sender, e));

    void Game_OnUnitMove(object sender, UpdateEventArgs<Sector> e) => InternalHandler(ef => ef.ProcessUnitMove(sender, e));

    void Game_OnUnitDeath(object sender, EliminatedEventArgs e) => InternalHandler(ef => ef.ProcessUnitDeath(sender, e));

    void InternalHandler(Action<Effect> handler)
    {
        foreach (Effect effect in _effects.Values)
            handler(effect);
    }

    #endregion

    #region Helper Methods

    public IEnumerable<T> GetEffects<T>() where T : Effect => _effects.Values.Where(ef => typeof(T).IsAssignableFrom(ef.GetType())).Select(ef => (T)ef);

    public void ApplyEffect(Effect effect)
    {
        _effects.Add(_nextEffectId, effect);
        effect.ApplyTo(_owner, this, _nextEffectId);
        _nextEffectId++;
    }

    public bool HasEffect<T>() where T : Effect => GetEffects<T>().Any();

    public void RemoveEffect(Effect effect)
    {
        if (!_effects.ContainsKey(effect.Id) ||
            _effects[effect.Id] != effect)
            throw new ArgumentException("Effect not in manager.");

        _effects.Remove(effect.Id);
        effect.ProcessEffectRemove();
    }

    public void RemoveEffect<T>() where T : Effect
    {
        Stack<Effect> toRemove = new Stack<Effect>();
        foreach (Effect effect in GetEffects<T>())
            toRemove.Push(effect);
        while (toRemove.Count > 0)
            RemoveEffect(toRemove.Pop());
    }

    public void RemoveAllEffects() => RemoveEffect<Effect>();

    #endregion
}
