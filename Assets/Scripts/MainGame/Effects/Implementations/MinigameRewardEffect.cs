using System;

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

        #region Helper Methods

        protected override void ApplyToPlayer(Player player)
        { }

        #endregion
    }
}
