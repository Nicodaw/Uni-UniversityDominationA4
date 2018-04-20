using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class UnitStatsEffect : Effect
    {
        #region Private Fields
        int _attackModifier;
        int _defenceModifier;
        string _description;
        bool _target; // adhoc gallore. True: target friendly, False: target hostile;
        #endregion

        #region Override Properties

        public override string CardDescription { get { return _description; } }

        public override int? AttackBonus => _attackModifier;

        public override int? DefenceBonus => _defenceModifier;

        #endregion

        #region Constructor
        public UnitStatsEffect(CardType type)
        {
            switch (type)
            {
                case CardType.Kuda:
                    _attackModifier = 1;
                    _description = "Increase the attack of a unit by 1";
                    _target = true;
                    break;
                case CardType.Breadcrumbs:
                    _defenceModifier = 1;
                    _description = "Increase the defence of a unit by 1";
                    _target = true;
                    break;
                case CardType.FirstYearInTheLibrary:
                    _attackModifier = -1;
                    _description = "Decrease the attack of a unit by 1";
                    _target = false;
                    break;
                case CardType.NightBeforeExams:
                    _defenceModifier = -1;
                    _description = "Decrease the defence of a unit by 1";
                    _target = false;
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Concrete Methods
        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            //get all sectors that have a unit and that unit is controlled by the current player which is playing the card
            List<Sector> sectorDomain;
            List<Unit> availableUnits = new List<Unit>();

            if (_target) //if the card is targeting friendly units
            {
                sectorDomain = (List<Sector>)game.Map.Sectors.ToList().Where(sector => sector.Unit != null && sector.Unit.Owner == game.CurrentPlayer);
            }
            else //if not, the card is targeting hostile units
            {
                sectorDomain = (List<Sector>)game.Map.Sectors.ToList().Where(sector => sector.Unit != null && sector.Unit.Owner != game.CurrentPlayer);
            }

            foreach (Sector sector in sectorDomain)
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