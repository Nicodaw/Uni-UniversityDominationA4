using System;

namespace EffectImpl
{
    [Serializable]
    public class PVCEffect : Effect
    {
        #region Private Fields

        [NonSerialized]
        Sector _appliedSector;

        #endregion

        #region Handlers

        public override void ProcessSectorCaptured(object sender, UpdateEventArgs<Player> e)
        {
            if ((Sector)sender == _appliedSector)
                Game.Instance.TriggerMinigame(e.NewValue);
        }

        #endregion

        #region Helper Methods

        protected override void ApplyToSector(Sector sector)
        {
            _appliedSector = sector;
        }

        protected override void RestoreSector(Sector sector)
        {
            _appliedSector = sector;
        }

        #endregion
    }
}
