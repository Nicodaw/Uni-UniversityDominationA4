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
            Game.MementoToRestore = Game.Instance.CreateMemento(); // save game into static var
            MinigameManager.CurrentPlayerId = e.NewValue.Id; // store the ID of the player who will get the reward
            SceneManager.LoadScene("Minigame"); // trigger scene change
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
