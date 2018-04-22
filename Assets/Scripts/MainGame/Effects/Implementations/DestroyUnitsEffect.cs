using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class DestroyUnitsEffect : Effect
    {
        #region Private Fields

        Player _appliedPlayer;

        #endregion

        #region Override Properties

        public override string CardName => "Student Debt";

        public override string CardDescription => "Destroy target player's postgraduate units";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.EnemyPlayer;

        public override CardTier CardTier => CardTier.Tier3;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Players = game.Players.Where(p => p != game.CurrentPlayer)
        };

        #endregion

        #region Helper Methods

        void DestroyPostgrads(Player player)
        {
            _appliedPlayer = player;
            IEnumerable<Unit> toBeDestroyed = _appliedPlayer.Units.Where(u => u.CompareTag("Postgrad"));
            foreach (Unit victim in toBeDestroyed)
                victim.Kill(Game.Instance.CurrentPlayer);
            RemoveSelf();
        }

        protected override void ApplyToPlayer(Player player) => DestroyPostgrads(player);

        #endregion
    }
}
