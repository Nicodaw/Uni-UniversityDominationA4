using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class GraduateEffect : Effect
    {
        #region Private Fields
        int _postgradAttack;
        int _postgradDefence;
        int _postgradMove;
        int _postgradLevelCap;
        #endregion

        #region Override Properties

        public override string CardDescription { get { return "Upgrade an undergraduate to a postgraduate";}}

        public override int? AttackBonus => _postgradAttack;

        public override int? DefenceBonus => _postgradDefence;

        public override int? MoveRangeBonus => _postgradMove;

        public override int? LevelCapBonus => _postgradLevelCap;

        #endregion

        #region Constructor
        public GraduateEffect(Unit unit)
        {
            _postgradAttack = 2;
            _postgradDefence = 2;
            _postgradMove = 2;
            _postgradLevelCap = 8;
        }

        #endregion

        #region Concrete Methods
        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            //get all sectors that have a unit and that unit is controlled by the current player which is playing the card
            IEnumerable<Sector> playerControlledSectors = game.Map.Sectors.ToList().Where(sector => sector.Unit != null && sector.Unit.Owner == game.CurrentPlayer);
            List<Unit> availableUnits = new List<Unit>();
            foreach (Sector sector in playerControlledSectors)
            {
                availableUnits.Add(sector.Unit);
            }
            EffectAvailableSelection available = new EffectAvailableSelection
            {
                Units = availableUnits
            };
            return available;
        }
        #endregion


        #region Helper Methods

        protected override void ApplyToUnit(Unit unit)
        {

        }

        #endregion


    }
}