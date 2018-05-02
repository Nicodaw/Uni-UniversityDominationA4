using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class UnitSkipTurnEffect : TurnedEffect
    {
        #region Private Fields

        int _playedBy;

        #endregion

        #region Override Properties

        protected override int TurnsLeft { get; set; } = 1;

        protected override Player TurnedPlayer => Game.Instance.Players[_playedBy];

        public override string CardName => "Hangover";

        public override string CardDescription => "Target enemy unit cannot move for 1 turn";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.EnemyUnit;

        public override CardTier CardTier => CardTier.Tier1;

        public override bool? CanMove => false;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = game.Map.Sectors.Select(s => s.Unit).Where(u => u != null && u.Owner != game.CurrentPlayer)
        };

        public override void ProcessEffectRemove() => DisableHangover();

        #endregion

        #region Helper Methods

        void EnableHangover() => AppliedUnit.ShowHangover = true;

        void DisableHangover() => AppliedUnit.ShowHangover = false;

        protected override void ApplyToUnit()
        {
            _playedBy = Game.Instance.CurrentPlayer.Id;
            EnableHangover();
        }

        protected override void RestoreUnit() => EnableHangover();

        #endregion
    }
}
