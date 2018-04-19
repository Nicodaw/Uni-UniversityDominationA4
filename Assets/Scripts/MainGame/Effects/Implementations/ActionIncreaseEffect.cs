using System.Collections;
using System.Collections.Generic;
using System;

namespace EffectImpl
{
    [Serializable]
    public class ActionIncreaseEffect : Effect
    {
        #region Private Fields
        int _bonusActions;
        #endregion

        #region Override Properties

        public override string CardDescription { get { return "Increase amount of actions you can perform this turn by one"; } }

        public override int? ActionBonus => _bonusActions;

        #endregion

        #region Constructor
        public ActionIncreaseEffect()
        {
            _bonusActions = 1;

        }
        #endregion

        #region Concrete methods
        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            EffectAvailableSelection available = new EffectAvailableSelection
            {
                Players = (IEnumerable<Player>)game.CurrentPlayer
            };
            return available;
        }
        #endregion


        #region Helper Methods

        protected override void ApplyToPlayer(Player player)
        {
            player.ActionsRemaining += _bonusActions;
        }


        #endregion


    }
}