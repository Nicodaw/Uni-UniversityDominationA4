using System;
using System.Linq;
using UnityEngine;

namespace EffectImpl
{
    [Serializable]
    public class BlockSectorEffect : Effect
    {
        #region Private Fields

        Player playedBy = Game.Instance.CurrentPlayer;
        int _turnsLeft = 2;
        Sector _appliedSector;
        GameObject _barricadeModel; //tbd: add barricade model

        #endregion

        #region Override Properties

        public override string CardName => "Industrial action";

        public override string CardDescription => "Target unoccupied sector becomes impassable for 2 turns";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.Sector;

        public override CardTier CardTier => CardTier.Tier1;

        public override bool? Traversable => false;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Sectors = game.Map.Sectors.Where(s => s.AllowPVC) //reuse of the PVC flag as it selects only non-occupied sectors that don't have a landmark
        };

        #endregion

        #region Handlers

        public override void ProcessPlayerTurnStart(object sender, EventArgs e)
        {
            if ((Player) sender == playedBy) //if at the start of the 
            {
                _turnsLeft = _turnsLeft - 1;
                if (_turnsLeft == 0)
                    UnBlock();
            }
        }

        #endregion

        #region Helper Methods

        void Block(Sector sector)
        {
            //apply barricade 
            _appliedSector = sector;
            UnityEngine.Object.Instantiate(_barricadeModel, _appliedSector.Unit.transform); //append the barricade as a child element on the Unit placeholder

        }

        void UnBlock()
        {
            UnityEngine.Object.Destroy(_appliedSector.Unit.transform.GetChild(0)); //remove the barricade
        }

        protected override void ApplyToSector(Sector sector) => Block(sector);

        protected override void RestoreSector(Sector sector) => Block(sector);

        #endregion
    }
}
