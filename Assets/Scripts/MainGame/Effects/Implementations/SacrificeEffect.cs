using System;

namespace EffectImpl
{
    [Serializable]
    public class SacrificeEffect : Effect
    {
        #region Override Properties

        public override string CardName => "Drop Out";

        public override string CardDescription => "Sacrifice a unit to aquire a random card of higher power.";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.Sacrifice;

        public override CardTier CardTier => CardTier.Tier1;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = game.CurrentPlayer.Units
        };

        #endregion

        #region Helper Methods

        void DoSacrifice()
        {
            CardTier tier;
            if (AppliedUnit.Stats.HasEffect<GraduateEffect>())
                tier = CardTier.Tier3;
            else
                tier = CardTier.Tier2;
            Game.Instance.CurrentPlayer.Cards.AddCards(CardFactory.GetRandomEffect(tier));
            AppliedUnit.Kill(Game.Instance.CurrentPlayer);
        }

        protected override void ApplyToUnit() => DoSacrifice();

        #endregion
    }
}
