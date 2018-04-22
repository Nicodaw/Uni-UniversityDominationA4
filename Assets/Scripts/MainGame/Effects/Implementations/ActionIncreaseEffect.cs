using System;

namespace EffectImpl
{
    [Serializable]
    public class ActionIncreaseEffect : Effect
    {
        #region Override Properties

        public override string CardName => "Adderall Supply";

        public override string CardDescription => "Increase amount of actions you can perform this turn by one";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.SelfPlayer;

        public override CardTier CardTier => CardTier.Tier1;

        public override int? ActionBonus => 1;

        #endregion

        #region Concrete methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Players = new[] { game.CurrentPlayer }
        };

        public override void ProcessPlayerTurnEnd(object sender, EventArgs e) => RemoveSelf();

        #endregion

        #region Helper Methods

        protected override void ApplyToPlayer()
        { }

        #endregion
    }
}
