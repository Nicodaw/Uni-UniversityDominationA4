using System;

/// <summary>
/// The base for turn-based effects. It will automatically remove itself
/// from the manager when the turn ends with TurnsLeft at 0.
/// </summary>
public abstract class TurnedEffect : Effect
{
    #region Abstract Properties

    /// <summary>
    /// The number of turns left until the effect is removed.
    /// If 0, it will be removed on the next turn end.
    /// </summary>
    protected abstract int TurnsLeft { get; set; }

    /// <summary>
    /// The player to base the turns on.
    /// If <c>null</c>, then every player is counted for turns.
    /// If set to a player, then the turn end event is only processed when
    /// the given player's turn ends.
    /// </summary>
    /// <remarks>
    /// The main purpose of this property is to allow both player turns and
    /// turn cycles to be used for the turn-based effect.
    /// </remarks>
    protected virtual Player TurnedPlayer { get; } = null;

    #endregion

    #region Protected Methods

    public override void ProcessPlayerTurnEnd(object sender, EventArgs e)
    {
        // if the TurnedPlayer is set, only process turn end if the
        // set player's turn ended
        if (TurnedPlayer != null && (Player)sender != TurnedPlayer)
            return;
        // process the turn end
        if (TurnsLeft > 0)
            TurnsLeft--;
        else
            RemoveSelf();
    }

    #endregion
}
