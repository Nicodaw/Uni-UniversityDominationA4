using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization;

namespace EffectImpl
{
    [Serializable]
    public class MinigameRewardEffect : Effect
    {
        #region Private Fields

        int _playerId;
        int _attackBonus;
        int _defenceBonus;

        #endregion

        #region Properties

        public int ApplyPlayer => _playerId;

        #endregion

        #region Override Properties

        public override int? AttackBonus => _attackBonus;

        public override int? DefenceBonus => _defenceBonus;

        #endregion

        #region Constructor

        public MinigameRewardEffect(int playerId, int attackBonus, int defenceBonus)
        {
            _playerId = playerId;
            _attackBonus = attackBonus;
            _defenceBonus = defenceBonus;
        }

        #endregion

        #region Serialization

        public MinigameRewardEffect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _playerId = info.GetInt32("playerId");
            _attackBonus = info.GetInt32("attackBonus");
            _defenceBonus = info.GetInt32("defenceBonus");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("playerId", _playerId);
            info.AddValue("attackBonus", _attackBonus);
            info.AddValue("defenceBonus", _defenceBonus);
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
