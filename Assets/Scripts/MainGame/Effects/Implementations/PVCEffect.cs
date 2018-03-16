using System;
using System.Runtime.Serialization;
using UnityEngine.SceneManagement;

namespace EffectImpl
{
    public class PVCEffect : Effect
    {
        #region Constructor

        public PVCEffect()
        { }

        #endregion

        #region Serialization

        public PVCEffect(SerializationInfo info, StreamingContext context) : base(info, context)
        { }

        #endregion

        #region Handlers

        public override void ProcessSectorCaptured(object sender, UpdateEventArgs<Player> e)
        {
            // save game into static var
            Game.MementoToRestore = Game.Instance.CreateMemento();
            // trigger scene change
            SceneManager.LoadScene("Minigame");
        }

        #endregion

        #region Helper Methods

        public override EffectAvailableSelection AvailableSelection(Game game)
        {
            throw new InvalidOperationException();
        }

        #endregion
    }
}
