using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class LandmarkWrapperEffect : Effect
    {
        #region Override Properties

        public override int? AttackBonus =>
        // get landmark attack bonus
        AppliedPlayer.OwnedLandmarkSectors.Select(s => s.Landmark).Where(l => l.Resource == ResourceType.Attack).Sum(l => l.Amount) +
        // get all temp landmarks attack bonus
        AppliedPlayer.OwnedSectors.SelectMany(s => s.Stats.GetEffects<TemporaryLandmarkEffect>()).Sum(ef => ef.PlayerAttackBonus);

        public override int? DefenceBonus =>
        // get landmark defence bonus
        AppliedPlayer.OwnedLandmarkSectors.Select(s => s.Landmark).Where(l => l.Resource == ResourceType.Defence).Sum(l => l.Amount) +
        // get all temp landmarks defence bonus
        AppliedPlayer.OwnedSectors.SelectMany(s => s.Stats.GetEffects<TemporaryLandmarkEffect>()).Sum(ef => ef.PlayerDefenceBonus);

        #endregion

        #region Helper Methods

        protected override void ApplyToPlayer()
        { }

        #endregion
    }
}
