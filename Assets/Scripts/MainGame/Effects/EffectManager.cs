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

    public int Attack
    {
        get
        {
            return 0 + // default value
                _effects.Values
                        .Where(ef => ef.AttackBonus.HasValue)
                        .Sum(ef => ef.AttackBonus.Value);
        }
    }

    public int Defence
    {
        get
        {
            return 0 + // default value
                _effects.Values.Where(ef => ef.DefenceBonus.HasValue).Sum(ef => ef.DefenceBonus.Value);
        }
    }

    public int Actions
    {
        get
        {
            return 2 + // default value
                _effects.Values.Where(ef => ef.ActionBonus.HasValue).Sum(ef => ef.ActionBonus.Value);
        }
    }

    public bool Traversable
    {
        get
        {
            bool traversable = true; // default value
            foreach (bool trav in _effects.Values.Where(ef => ef.Traversable.HasValue).Select(ef => ef.Traversable.Value))
                traversable &= trav;
            return traversable;
        }
    }

    public int MoveRange
    {
        get
        {
            return 1 + // default value
                _effects.Values.Where(ef => ef.MoveRangeBonus.HasValue).Sum(ef => ef.MoveRangeBonus.Value);
        }
    }

    public int LevelCap
    {
        get
        {
            return 5 + // default value
                _effects.Values.Where(ef => ef.LevelCapBonus.HasValue).Sum(ef => ef.LevelCapBonus.Value);
        }
    }

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
            Debug.LogFormat("effect id: {0}", effect.Id);
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

    void Game_OnUnitDeath(object sender, EventArgs e) => InternalHandler(ef => ef.ProcessUnitDeath(sender, e));

    void InternalHandler(Action<Effect> handler)
    {
        foreach (Effect effect in _effects.Values)
            handler(effect);
    }

    #endregion

    #region Helper Methods

    public void ApplyEffect(Effect effect)
    {
        _effects.Add(_nextEffectId, effect);
        effect.ApplyTo(_owner, this, _nextEffectId);
        _nextEffectId++;
    }

    public bool HasEffect<T>() => GetEffects<T>().Any();

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

    IEnumerable<Effect> GetEffects<T>() => _effects.Values.Where(ef => ef.GetType() == typeof(T));

    #endregion
}
