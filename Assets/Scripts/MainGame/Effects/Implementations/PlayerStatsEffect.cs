using System;
using System.Collections.Generic;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class PlayerStatsEffect : Effect
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

        public override CardCornerIcon CardCornerIcon => (_friendly)?CardCornerIcon.SelfPlayer:CardCornerIcon.EnemyPlayer;

        public override CardTier CardTier => CardTier.Tier2;

        public override int? AttackBonus => _attackModifier;

        public override int? DefenceBonus => _defenceModifier;

        #endregion

        #region Constructor

        public PlayerStatsEffect(CardType type)
        {
            switch (type)
            {
                case CardType.KudaWithTheLads:
                    _name = "Kuda With The Lads";
                    _description = "Increase the attack of your units by 1";
                    _attackModifier = 1;
                    _friendly = true;
                    break;
                case CardType.BreadcrumbFactory:
                    _name = "Breadcrumb factory";
                    _description = "Increase the defence of your units by 1";
                    _defenceModifier = 1;
                    _friendly = true;
                    break;
                case CardType.ArguingOverBars:
                    _name = "Arguing Over Bars";
                    _description = "Decrease target player's units attack by 1 ";
                    _attackModifier = -1;
                    _friendly = false;
                    break;
                case CardType.BadIntentionsSTYC:
                    _name = "Bad Intentions STYC";
                    _description = "Decrease target player's units defence by 1";
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
            Players = _friendly ?
                    // get friendly units
                    game.Players.Where(p => game.CurrentPlayer):
                    // get enemy units
                   game.Players.Where(p => !p.IsEliminated && p != game.CurrentPlayer)

        };

        #endregion

        #region Helper Methods

        protected override void ApplyToUnit(Unit unit)
        { }

        #endregion
    }
}
