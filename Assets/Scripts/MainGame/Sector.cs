using UnityEngine;

public class Sector : MonoBehaviour
{
    #region Unity Bindings

    public Map map;
    public Sector[] adjacentSectors;
    public Landmark landmark;

    #endregion

    #region Private fields

    Unit unit;
    Player owner;
    bool pvc = false;

    #endregion

    #region Public properties

    /// <summary>
    /// Whether the PVC is contained in this sector.
    /// </summary>
    public bool HasPVC
    {
        get { return pvc; }
        set { pvc = value; }
    }

    /// <summary>
    /// The Unit occupying this sector.
    /// </summary>
    public Unit Unit
    {
        get { return unit; }
        set { unit = value; }
    }

    /// <summary>
    /// The player who owns the sector.
    /// </summary>
    public Player Owner
    {
        get { return owner; }
        set
        {
            owner = value;

            // set sector color to the color of the given player
            // or gray if null
            if (owner == null)
                gameObject.GetComponent<Renderer>().material.color = Color.gray;
            else
                gameObject.GetComponent<Renderer>().material.color = owner.Color;
        }
    }

    /// <summary>
    /// The neighbouring sectors.
    /// </summary>
    public Sector[] AdjacentSectors
    {
        get { return adjacentSectors; }
    }

    /// <summary>
    /// The landmark on this sector.
    /// </summary>
    public Landmark Landmark
    {
        get { return landmark; }
        set { landmark = value; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes a sector.
    /// Determines if the sector contains a landmark.
    /// Sets owner and unit to null.
    /// </summary>
    public void Initialize()
    {
        // set no owner
        Owner = null;

        // clear unit
        unit = null;

        // get landmark (if any)
        Landmark = gameObject.GetComponentInChildren<Landmark>();
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Highlight a sector by increasing its RGB values by a specified amount.
    /// </summary>
    /// <param name="amount"></param>
    public void ApplyHighlight(float amount)
    {
        Renderer renderer = GetComponent<Renderer>();
        Color currentColor = renderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor + offset;
        renderer.material.color = newColor;
    }

    /// <summary>
    /// Unhighlight a sector by decreasing its RGB values by a specified amount.
    /// </summary>
    /// <param name="amount"></param>
    public void RevertHighlight(float amount)
    {
        Renderer renderer = GetComponent<Renderer>();
        Color currentColor = renderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor - offset;
        renderer.material.color = newColor;
    }

    /// <summary>
    /// Highlight each sector adjacent to this one.
    /// </summary>
    public void ApplyHighlightAdjacent()
    {
        foreach (Sector adjacentSector in adjacentSectors)
        {
            adjacentSector.ApplyHighlight(0.2f);
        }
    }

    /// <summary>
    /// Unhighlight each sector adjacent to this one.
    /// </summary>
    public void RevertHighlightAdjacent()
    {
        foreach (Sector adjacentSector in adjacentSectors)
        {
            adjacentSector.RevertHighlight(0.2f);
        }
    }

    /// <summary>
    /// Clear this sector of any unit.
    /// </summary>
    public void ClearUnit()
    {
        unit = null;
    }

    void OnMouseUpAsButton()
    {
        // when this sector is clicked, determine the context
        // and act accordingly
        OnMouseUpAsButtonAccessible();
    }

    public void OnMouseUpAsButtonAccessible()
    {
        // a method of OnMouseUpAsButton that is 
        // accessible to other objects for testing

        // if this sector contains a unit and belongs to the
        // current active player, and if no unit is selected
        if (unit != null && owner.Active && map.game.NoUnitSelected())
        {
            // select this sector's unit
            unit.Select();
        }

        // if this sector's unit is already selected
        else if (unit != null && unit.IsSelected)
        {
            // deselect this sector's unit           
            unit.Deselect();
        }

        // if this sector is adjacent to the sector containing
        // the selected unit
        else if (AdjacentSelectedUnit() != null)
        {
            // get the selected unit
            Unit selectedUnit = AdjacentSelectedUnit();

            // deselect the selected unit
            selectedUnit.Deselect();

            // if this sector is unoccupied
            if (unit == null)
                MoveIntoUnoccupiedSector(selectedUnit);

            // if the sector is occupied by a friendly unit
            else if (unit.Owner == selectedUnit.Owner)
                MoveIntoFriendlyUnit(selectedUnit);

            // if the sector is occupied by a hostile unit
            else if (unit.Owner != selectedUnit.Owner)
                MoveIntoHostileUnit(selectedUnit, this.unit);

            map.game.NextTurnState(); // adavance to next turn phase when action take (Modified by Dom 13/02/2018)
        }
    }

    /// <summary>
    /// Get the level of the unit on the sector.
    /// </summary>
    /// <returns>The level of the sector</returns>
    [System.Obsolete("Will be removed/reworked after memento pattern implementation.")]
    public int GetLevel()
    {
        if (unit == null)
        {
            return -1;
        }
        else
        {
            return unit.Level;
        }
    }

    /// <summary>
    /// Moves the passed unit onto this sector.
    /// Should only be used when this sector is unoccupied.
    /// </summary>
    /// <param name="unit">The unit to be moved onto this sector.</param>
    public void MoveIntoUnoccupiedSector(Unit unit)
    {
        // move the selected unit into this sector
        unit.MoveTo(this);
    }

    /// <summary>
    /// Switches the unit on this sector with the passed one.
    /// </summary>
    /// <param name="otherUnit">Unit object of the unit on the adjacent sector to be switched onto this sector.</param>
    public void MoveIntoFriendlyUnit(Unit otherUnit)
    {
        // swap the two units
        unit.SwapPlacesWith(otherUnit);
    }

    /// <summary>
    /// Initates a combat encounter between a pair of units.
    /// The losing is destroyed.
    /// If the attacker wins then they move onto the defending units territory.
    /// </summary>
    /// <param name="attackingUnit"></param>
    /// <param name="defendingUnit"></param>
    public void MoveIntoHostileUnit(Unit attackingUnit, Unit defendingUnit)
    {
        // if the attacking unit wins
        if (Conflict(attackingUnit, defendingUnit))
        {
            // destroy defending unit
            defendingUnit.DestroySelf();

            // move the attacking unit into this sector
            attackingUnit.MoveTo(this);
        }
        // if the defending unit wins
        else
        {
            // destroy attacking unit
            attackingUnit.DestroySelf();
        }

        // removed automatically end turn after attacking (Modified by Dom 13/02/18)
    }

    public Unit AdjacentSelectedUnit()
    {
        // return the selected unit if it is adjacent to this sector
        // return null otherwise

        // scan through each adjacent sector
        foreach (Sector adjacentSector in adjacentSectors)
        {
            // if the adjacent sector contains the selected unit,
            // return the selected unit
            if (adjacentSector.unit != null && adjacentSector.unit.IsSelected)
                return adjacentSector.unit;
        }

        // otherwise, return null
        return null;
    }

    /// <summary>
    /// Returns the outcome of a combat encounter between two units.
    /// Takes into consideration the units levels and the attack/defence bonus of the player.
    /// 
    /// Close match leads to uncertain outcome (i.e. could go either way).
    /// If one unit + bonuses is significantly more powerful than another then they are very likely to win.
    /// </summary>
    /// <param name="attackingUnit">Unit object of the attacking unit.</param>
    /// <param name="defendingUnit">Unit object of the defending unit.</param>
    /// <returns><c>true</c> if attacking unit wins or <c>false</c> if defending unit wins.</returns>
    bool Conflict(Unit attackingUnit, Unit defendingUnit)
    {
        // diff = +ve attacker advantage 
        // diff = -ve defender advantage
        int diff = (attackingUnit.Level + attackingUnit.Owner.AttackBonus + 1) - (defendingUnit.Level + defendingUnit.Owner.DefenceBonus);

        // determine uncertaincy in combat
        // small diff in troops small uncertaincy level
        float uncertaincy = -0.4f * (Mathf.Abs(diff)) + 0.5f;
        if (uncertaincy < 0.1f)
        {
            uncertaincy = 0.1f; // always at least 10% uncertaincy
        }

        if (Random.Range(0, 1) < uncertaincy)
        {
            if (diff < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            if (diff < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    #endregion
}