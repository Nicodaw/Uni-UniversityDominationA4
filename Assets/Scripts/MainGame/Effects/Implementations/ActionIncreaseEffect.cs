using System;

namespace EffectImpl
{
    [Serializable]
    public class ActionIncreaseEffect : Effect
    {
        #region Override Properties

        public override string CardDescription => "Increase amount of actions you can perform this turn by one";

        public override int? ActionBonus => 1;

        #endregion

        #region Concrete methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Players = new[] { game.CurrentPlayer }
        };

        #endregion

        #region Helper Methods

        protected override void ApplyToPlayer(Player player)
        { }

        #endregion
    }
}
