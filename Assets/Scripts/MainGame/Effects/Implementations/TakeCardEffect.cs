using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class TakeCardEffect : Effect
    {
        #region Private Fields

        #endregion

        #region Override Properties

        public override string CardName => "Copy Notes";

        public override string CardDescription => "Take a random card from a player's hand";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.EnemyPlayer;

        public override CardTier CardTier => CardTier.Tier1;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Players = game.Players.Where(p => p != game.CurrentPlayer && p.Cards.Count > 0)
        };

        #endregion

        #region Helper Methods

        void TakeCard() => Game.Instance.CurrentPlayer.Cards.AddCards(AppliedPlayer.Cards.RemoveRandomCard());

        protected override void ApplyToPlayer() => TakeCard();

        #endregion
    }
}
