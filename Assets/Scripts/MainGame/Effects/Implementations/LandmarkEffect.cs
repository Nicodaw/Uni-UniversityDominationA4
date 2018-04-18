using System;

namespace EffectImpl
{
    [Serializable]
    public class LandmarkEffect : Effect
    {
        #region Private Fields

        int _sector;
        int _attackBonus;
        int _defenceBonus;

        #endregion

        #region Override Properties

        public override int? AttackBonus => _attackBonus;

        public override int? DefenceBonus => _defenceBonus;

        #endregion

        #region Constructor

        public LandmarkEffect(int sector, int attackBonus, int defenceBonus)
        {
            _sector = sector;
            _attackBonus = attackBonus;
            _defenceBonus = defenceBonus;
        }

        #endregion

        #region Helper Methods

        protected override void ApplyToPlayer(Player player)
        { }

		protected override void RestorePlayer(Player player)
		{
            // register the effect so it can be managed by landmark
            Game.Instance.Map.Sectors[_sector].Landmark.RegisterPlayerEffect(this);
		}

		#endregion
	}
}
