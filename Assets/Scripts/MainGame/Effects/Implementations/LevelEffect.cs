using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class LevelEffect : Effect
    {
        #region Private Fields

        int _levelModifier;

        #endregion

        #region Override Properties

        public override string CardName => "Resits";

        public override string CardDescription => "Permanently reduce enemy unit's level by 2";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.EnemyUnit;

        public override CardTier CardTier => CardTier.Tier2;

        public override int? LevelCapBonus => -2;

        #endregion


        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = game.Map.Sectors.Select(s => s.Unit).Where(u => u != null && u.Owner != game.CurrentPlayer)

        };

        #endregion

        #region Helper Methods

        protected override void ApplyToUnit(Unit unit)
        { }

        #endregion
    }
}
