using System;
using System.Linq;
using UnityEngine;

namespace EffectImpl
{
    [Serializable]
    public class UnitSkipTurnEffect : Effect
    {
        #region Private Fields

        int _playedBy;
        [NonSerialized]
        Unit _appliedUnit;
        [NonSerialized]
        GameObject _hangoverModel; //tbd: add UI indication for locked unit

        #endregion

        #region Override Properties

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

        #endregion

        #region Helper Methods

        void EnableHangover(Unit unit)
        {
            _appliedUnit = unit;
            // set hangover UI state
        }

        void DisableHangover()
        {
            // set handover UI state
        }

        protected override void ApplyToUnit(Unit unit) => EnableHangover(unit);

        protected override void RestoreUnit(Unit unit) => EnableHangover(unit);

        #endregion
    }
}
