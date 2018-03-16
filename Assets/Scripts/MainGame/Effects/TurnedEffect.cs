using System;

/// <summary>
/// The base for turn-based effects. It will automatically remove itself
/// from the manager when the turn ends with TurnsLeft at 0.
/// </summary>
public abstract class TurnedEffect : Effect
{
    #region Abstract Properties

    protected abstract int TurnsLeft { get; set; }

    #endregion

    #region Protected Methods

    public override void ProcessPlayerTurnEnd(object sender, EventArgs e)
    {
        if (TurnsLeft > 0)
            TurnsLeft--;
        else
            RemoveSelf();
    }

    #endregion
}
