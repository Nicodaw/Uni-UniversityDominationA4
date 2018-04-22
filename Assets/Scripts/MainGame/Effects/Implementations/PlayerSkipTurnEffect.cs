using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class PlayerSkipTurnEffect : Effect
    {
        #region Private Fields

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

        public override void ProcessEffectRemove()
        {
            RemoveSelf();
        }

        #endregion

        #region Handlers

        public override void ProcessPlayerTurnEnd(object sender, EventArgs e)
        {
            if ((Player)sender == _appliedPlayer) //Release the lock after the turn of the player who owns the locked unit ends
                ProcessEffectRemove();
        }

        #endregion

        #region Helper Methods

        void SkipNextTurn(Player player)
        {
            _appliedPlayer = player;
            player.EndTurn();
        }

        protected override void ApplyToPlayer(Player player) => SkipNextTurn(player);

        protected override void RestorePlayer(Player player) => SkipNextTurn(player);

        #endregion
    }
}
