using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class LandmarkWrapperEffect : Effect
    {
        #region Private Fields

        [NonSerialized]
        Player _player;

        #endregion

        #region Override Properties

        public override int? AttackBonus =>
        // get landmark attack bonus
        _player.OwnedLandmarkSectors.Select(s => s.Landmark).Where(l => l.Resource == ResourceType.Attack).Sum(l => l.Amount) +
        // get all temp landmarks attack bonus
        _player.OwnedSectors.SelectMany(s => s.Stats.GetEffects<TemporaryLandmarkEffect>()).Sum(ef => ef.PlayerAttackBonus);

        public override int? DefenceBonus =>
        // get landmark defence bonus
        _player.OwnedLandmarkSectors.Select(s => s.Landmark).Where(l => l.Resource == ResourceType.Defence).Sum(l => l.Amount) +
        // get all temp landmarks defence bonus
        _player.OwnedSectors.SelectMany(s => s.Stats.GetEffects<TemporaryLandmarkEffect>()).Sum(ef => ef.PlayerDefenceBonus);

        #endregion

        #region Helper Methods

        void SetAsignedPlayer(Player player) => _player = player;

        protected override void ApplyToPlayer(Player player) => SetAsignedPlayer(player);

        protected override void RestorePlayer(Player player) => SetAsignedPlayer(player);

        #endregion
    }
}
