using System.Collections;
using UnityEngine;

/// <summary>
/// Concrete AI player implementation.
/// </summary>
public class AiPlayer : Player
{
    #region Private Fields

    const float MoveWaitTime = 0.5f;

    #endregion

    #region Public Properties

    public override PlayerKind Kind => PlayerKind.AI;

    #endregion

    #region Override Methods

    public override void ProcessTurnStart()
    {
        base.ProcessTurnStart();
        StartCoroutine(PerformTurn());
    }

    #endregion

    #region Serialization

    public override void RestoreMemento(SerializablePlayer memento)
    {
        base.RestoreMemento(memento);
        StartCoroutine(OnRestoreCheck());
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Called when the player is retored (to allow user to save during AI turn).
    /// </summary>
    IEnumerator OnRestoreCheck()
    {
        // wait until end of frame
        // this is done to wait until memento restore is completed
        yield return null;
        // if current player is this on, then we need to restart the
        // coroutine process
        StartCoroutine(PerformTurn());
    }

    /// <summary>
    /// Performs the AI player's turn.
    /// </summary>
    IEnumerator PerformTurn()
    {
        while (CanPerformActions)
        {
            yield return new WaitForSeconds(MoveWaitTime);
            DoUnitMove();
        }
        EndTurn();
    }

    /// <summary>
    /// Does the actual movement logic.
    /// </summary>
    void DoUnitMove()
    {
        Sector selection = Units.Random().Sector; // select random unit
        Sector moveTo = selection.AdjacentSectors
                                 .RandomOrDefault(s => // select random out of available moves
                                                  (s.Owner == null || s.Owner.Kind == PlayerKind.AI) // unowned or owned by AI
                                                  && !s.HasPVC); // doesn't have PVC
        if (moveTo != null)
            AttemptMove(selection, moveTo); // move unit
        else // if we failed to move, consume the action anyway
            ConsumeAction();
    }

    #endregion
}
