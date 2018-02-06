using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour {

    [SerializeField] private Map map;
    [SerializeField] private Unit unit;
    [SerializeField] private Player owner;
    [SerializeField] private Sector[] adjacentSectors;
	[SerializeField] private Landmark landmark;
    [SerializeField] private bool VC = false;
    
    //added by Peter
    public bool isVC() {
        return VC;   
    }

    //added by Peter
    public void setVC(bool VC) {
        this.VC = VC;
    }

    public Map GetMap() {
        return map;
    }

    public void SetMap(Map map) {
        this.map = map;
    }

    public Unit GetUnit() {
        return unit;
    }

    public void SetUnit(Unit unit) {
        this.unit = unit;
    }

    public Player GetOwner() {
        return owner;
    }

    public void SetOwner (Player owner) {

        // set the owner of the sector to the specified player,
        // and change the sector's color to match the new owner


        // set sector owner to the given player
        this.owner = owner;

        // set sector color to the color of the given player
        // or gray if null
        if (owner == null) {
            gameObject.GetComponent<Renderer> ().material.color = Color.gray;
        } else {
            gameObject.GetComponent<Renderer> ().material.color = owner.GetColor();
        }
    }

    public Sector[] GetAdjacentSectors() {
        return adjacentSectors;
    }

	public Landmark GetLandmark() {
        return landmark;
    }

	public void SetLandmark(Landmark landmark) {
        this.landmark = landmark;
    }
        
	
	public void Initialize() {

        // initialize the sector by setting its owner and unit to null
        // and determining if it contains a landmark or not


		// reset owner
		SetOwner(null);

		// clear unit
		unit = null;

		// get landmark (if any)
		landmark = gameObject.GetComponentInChildren<Landmark>();



	}

    public void ApplyHighlight(float amount) {

        // highlight a sector by increasing its RGB values by a specified amount

        Renderer renderer = GetComponent<Renderer>();
        Color currentColor = renderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor + offset;

        renderer.material.color = newColor;
    }

    public void RevertHighlight(float amount) {

        // unhighlight a sector by decreasing its RGB values by a specified amount

        Renderer renderer = GetComponent<Renderer>();
        Color currentColor = renderer.material.color;
        Color offset = new Vector4(amount, amount, amount, 1);
        Color newColor = currentColor - offset;

        renderer.material.color = newColor;
    }

    public void ApplyHighlightAdjacent() {

        // highlight each sector adjacent to this one

        foreach (Sector adjacentSector in adjacentSectors)
        {
            adjacentSector.ApplyHighlight(0.2f);
        }
    }

    public void RevertHighlightAdjacent() {

        // unhighlight each sector adjacent to this one
        
        foreach (Sector adjacentSector in adjacentSectors)
        {
            adjacentSector.RevertHighlight(0.2f);
        }
    }

    public void ClearUnit() {

        // clear this sector of any unit

        unit = null;
    }

    void OnMouseUpAsButton () {

        // when this sector is clicked, determine the context
        // and act accordingly

		OnMouseUpAsButtonAccessible();

    }

    public void OnMouseUpAsButtonAccessible() {

        // a method of OnMouseUpAsButton that is 
        // accessible to other objects for testing


        // if this sector contains a unit and belongs to the
        // current active player, and if no unit is selected
        if (unit != null && owner.IsActive() && map.game.NoUnitSelected())
        {
            // select this sector's unit
            unit.Select();
        }

        // if this sector's unit is already selected
        else if (unit != null && unit.IsSelected())
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
            else if (unit.GetOwner() == selectedUnit.GetOwner())
                MoveIntoFriendlyUnit(selectedUnit);

            // if the sector is occupied by a hostile unit
            else if (unit.GetOwner() != selectedUnit.GetOwner())
                MoveIntoHostileUnit(selectedUnit, this.unit);
        }
    }

    public void MoveIntoUnoccupiedSector(Unit unit) {
        
        // move the selected unit into this sector
        unit.MoveTo(this);

        // advance turn state
        map.game.NextTurnState();
    }

    public void MoveIntoFriendlyUnit(Unit otherUnit) {

        // swap the two units
        this.unit.SwapPlacesWith(otherUnit);

        // advance turn state
        map.game.NextTurnState();
    }

    public void MoveIntoHostileUnit(Unit attackingUnit, Unit defendingUnit) {

        // start and resolve a conflict


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

        // end the turn
        map.game.EndTurn();
    }
        
    public Unit AdjacentSelectedUnit() {

        // return the selected unit if it is adjacent to this sector
        // return null otherwise


        // scan through each adjacent sector
        foreach (Sector adjacentSector in adjacentSectors)
        {
            // if the adjacent sector contains the selected unit,
            // return the selected unit
            if (adjacentSector.unit != null && adjacentSector.unit.IsSelected())
                return adjacentSector.unit;
        }

        // otherwise, return null
        return null;
    }

    private bool Conflict(Unit attackingUnit, Unit defendingUnit) {

        // return 'true' if attacking unit wins;
        // return 'false' if defending unit wins


        /*
         * Conflict resolution is done by comparing a random roll 
         * from each unit involved. The roll is weighted based on
         * the unit's level and the amount of the associated 
         * resource the unit's owner has. Beer is associated with
         * attacking, and Knowledge is associated with defending.
         * 
         * The formula is:
         * 
         *     roll = [ a random integer with a lowerbound of 1
         *              and an upperbound of 5 + the unit's level ] 
         *           + [ the amount of the associated resource the
         *               unit's owner has ]
         * 
         * In the event of a tie, the defending unit wins the conflict
         */

        // calculate the rolls of each unit
        int attackingUnitRoll = Random.Range(1, (5 + attackingUnit.GetLevel())) + attackingUnit.GetOwner().GetBeer();
        int defendingUnitRoll = Random.Range(1, (5 + defendingUnit.GetLevel())) + defendingUnit.GetOwner().GetKnowledge();

        return (attackingUnitRoll > defendingUnitRoll);
    }
        
}