using System;
using System.Collections;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement;

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

        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            throw new InvalidOperationException();
        }

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
