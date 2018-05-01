using System;

namespace EffectImpl
{
    [Serializable]
    public class PVCEffect : Effect
    {
        #region Handlers

        public override void ProcessSectorCaptured(object sender, UpdateEventArgs<Player> e)
        {
            if ((Sector)sender == AppliedSector)
                Game.Instance.TriggerMinigame();
        }

        #endregion

        #region Helper Methods

        protected override void ApplyToSector()
        { }

        #endregion
    }
}
