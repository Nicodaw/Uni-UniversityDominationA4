using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class DestroyUnitsEffect : Effect
    {
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

        void DestroyPostgrads()
        {
            IEnumerable<Unit> toBeDestroyed = AppliedPlayer.Units.Where(u => u.Stats.HasEffect<GraduateEffect>());
            foreach (Unit victim in toBeDestroyed)
                victim.Kill(Game.Instance.CurrentPlayer);
            RemoveSelf();
        }

        protected override void ApplyToPlayer() => DestroyPostgrads();

        #endregion
    }
}
