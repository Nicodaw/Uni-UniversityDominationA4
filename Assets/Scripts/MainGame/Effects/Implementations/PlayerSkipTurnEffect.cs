using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class PlayerSkipTurnEffect : Effect
    {
        #region Private Fields

        [NonSerialized]
        Player _appliedPlayer;

        #endregion

        #region Override Properties

        public override string CardName => "Summer break";

        public override string CardDescription => "Target player skips a turn";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.EnemyPlayer;

        public override CardTier CardTier => CardTier.Tier2;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Players = game.Players.Where(p => p != game.CurrentPlayer)
        };

		#endregion

		#region Handlers

		public override void ProcessPlayerTurnStart(object sender, EventArgs e)
		{
            // if the applied player's turn starts, end it and remove
            if ((Player)sender == _appliedPlayer)
            {
                _appliedPlayer.EndTurn();
                RemoveSelf();
			}
        }

		#endregion

		#region Helper Methods

		void SetAppliedPlayer(Player player) => _appliedPlayer = player;

        protected override void ApplyToPlayer(Player player) => SetAppliedPlayer(player);

        protected override void RestorePlayer(Player player) => SetAppliedPlayer(player);

        #endregion
    }
}
