using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HumanPlayer : Player
{
    #region Private Fields

    Sector _selectedSector;

    #endregion

    #region Public Properties

    public override PlayerKind Kind => PlayerKind.Human;

    #endregion

    #region Override Methods

    public override void ProcessSectorClick(Sector clickedSector)
    {
        base.ProcessSectorClick(clickedSector);
        if (_selectedSector == null)
        {
            if (clickedSector.Unit != null && clickedSector.Unit.Owner == this)
                SelectSector(clickedSector);
        }
        else
        {
            _selectedSector.ApplyHighlightAdjacent(false);
            if (_selectedSector.AdjacentSectors.Contains(clickedSector))
                AttemptMove(_selectedSector, clickedSector);
            _selectedSector = null;
        }
    }

    #endregion

    #region Helper Methods

    void SelectSector(Sector sector)
    {
        _selectedSector = sector;
        _selectedSector.ApplyHighlightAdjacent(true);
    }

    #endregion
}
