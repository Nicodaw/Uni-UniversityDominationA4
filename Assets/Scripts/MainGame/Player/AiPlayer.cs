using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    IEnumerator OnRestoreCheck()
    {
        // wait until end of frame
        // this is done to wait until memento restore is completed
        yield return null;
        // if current player is this on, then we need to restart the
        // coroutine process
        StartCoroutine(PerformTurn());
    }

    IEnumerator PerformTurn()
    {
        for (; ActionsRemaining > 0; ConsumeAction())
        {
            yield return new WaitForSeconds(MoveWaitTime);
            DoUnitMove();
        }
    }

    void DoUnitMove()
    {
        Sector selection = Units.Random().Sector; // select random unit
        Sector moveTo = selection.AdjacentSectors
                                 .Random(s => // select random out of available moves
                                         (s.Owner == null || s.Owner.Kind == PlayerKind.AI) // unowned or owned by AI
                                         && !s.HasPVC); // doesn't have PVC
        AttemptMove(selection, moveTo); // move unit
    }

    #endregion
}
