using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class UnitStatsEffect : Effect
    {
        #region Private Fields

        string _name;
        string _description;
        int _attackModifier;
        int _defenceModifier;
        bool _friendly; // adhoc gallore. True: target friendly, False: target hostile;

        #endregion

        #region Override Properties

        public override string CardName => _name;

        public override string CardDescription => _description;

        public override CardCornerIcon CardCornerIcon => _friendly ? CardCornerIcon.SelfUnit : CardCornerIcon.EnemyUnit;

        public override CardTier CardTier => CardTier.Tier1;

        public override int? AttackBonus => _attackModifier;

        public override int? DefenceBonus => _defenceModifier;

        #endregion

        #region Constructor

        public UnitStatsEffect(CardType type)
        {
            switch (type)
            {
                case CardType.Kuda:
                    _name = "Kuda";
                    _description = "Increase the attack of a unit by 1";
                    _attackModifier = 1;
                    _friendly = true;
                    break;
                case CardType.Breadcrumbs:
                    _name = "Breadcrumbs";
                    _description = "Increase the defence of a unit by 1";
                    _defenceModifier = 1;
                    _friendly = true;
                    break;
                case CardType.FirstYearInTheLibrary:
                    _name = "First Year In The Library";
                    _description = "Decrease the attack of a unit by 1";
                    _attackModifier = -1;
                    _friendly = false;
                    break;
                case CardType.NightBeforeExams:
                    _name = "Night Before Exams";
                    _description = "Decrease the defence of a unit by 1";
                    _defenceModifier = -1;
                    _friendly = false;
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = _friendly ?
                    // get friendly units
                    game.CurrentPlayer.Units :
                    // get enemy units
                    game.Map.Sectors.Select(s => s.Unit).Where(u => u != null && u.Owner != game.CurrentPlayer)

        };

        #endregion

        #region Helper Methods

        protected override void ApplyToUnit()
        { }

        #endregion
    }
}
