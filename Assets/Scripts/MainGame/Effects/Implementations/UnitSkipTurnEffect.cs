using System;
using System.Linq;
using UnityEngine;

namespace EffectImpl
{
    [Serializable]
    public class UnitSkipTurnEffect : Effect
    {
        #region Private Fields

        Unit _appliedUnit;
        GameObject _hangoverModel; //tbd: add UI indication for locked unit

        #endregion

        #region Override Properties

        public override string CardName => "Hangover";

        public override string CardDescription => "Target enemy unit cannot move for 1 turn";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.EnemyUnit;

        public override CardTier CardTier => CardTier.Tier1;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = game.Map.Sectors.Select(s => s.Unit).Where(u => u != null && u.Owner != game.CurrentPlayer)
        };

        public override void ProcessEffectRemove()
        {
            //TBD: restore normal movement
        }

        #endregion

        #region Handlers

        public override void ProcessPlayerTurnEnd(object sender, EventArgs e)
        {
            if ((Player)sender == _appliedUnit.Owner) //Release the lock after the turn of the player who owns the locked unit ends
                ProcessEffectRemove();
        }

        #endregion

        #region Helper Methods

        void ForbidMovement(Unit unit)
        {
            //TBD: Lock up a unit to not be interactable
            _appliedUnit = unit;
        }


        protected override void ApplyToUnit(Unit unit) => ForbidMovement(unit);

        protected override void RestoreUnit(Unit unit) => ForbidMovement(unit);

        #endregion
    }
}
