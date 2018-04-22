using System;

namespace EffectImpl
{
    [Serializable]
    public class SacrificeEffect : Effect
    {
        #region Override Properties

        public override string CardName => "Drop Out";

        public override string CardDescription => "Sacrifice a unit to aquire a random card of higher power.";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.SelfUnit;

        public override CardTier CardTier => CardTier.Tier1;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = game.CurrentPlayer.Units
        };

        #endregion

        #region Helper Methods

        void GetCard()
        {
            //TBD
        }

        protected override void ApplyToUnit() => GetCard();

        #endregion
    }
}
