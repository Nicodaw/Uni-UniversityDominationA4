using System;
using System.Runtime.Serialization;

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
            RemoveSelf(); // we don't need the effect any more
                          // save game into static var
                          // trigger minigame
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
