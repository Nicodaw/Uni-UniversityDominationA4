using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;

namespace EffectImpl
{
    [Serializable]
    public class LandmarkEffect : Effect
    {
        #region Private Fields

        int _attackBonus;
        int _defenceBonus;

        #endregion

        #region Override Properties

        public override int? AttackBonus => _attackBonus;

        public override int? DefenceBonus => _defenceBonus;

        #endregion

        #region Constructor

        public LandmarkEffect(int attackBonus, int defenceBonus)
        {
            _attackBonus = attackBonus;
            _defenceBonus = defenceBonus;
        }

        #endregion

        #region Helper Methods

        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            throw new InvalidOperationException();
        }

        protected override void ApplyToPlayer(Player player)
        { }

        #endregion
    }
}
