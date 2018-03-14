using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region Unity Bindings

    public Material level1Material;
    public Material level2Material;
    public Material level3Material;
    public Material level4Material;
    public Material level5Material;

    #endregion

    #region Private Fields

    Player owner;
    Sector sector;
    int level;
    Color color;
    bool selected = false;

    #endregion

    #region Public Properties

    /// <summary>
    /// Player that owns this unit
    /// </summary>
    public Player Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    /// <summary>
    /// The Sector the unit is occupying
    /// </summary>
    public Sector Sector
    {
        get { return sector; }
        set { sector = value; }
    }

    /// <summary>
    /// The unit's level
    /// </summary>
    public int Level
    {
        get { return (this == null) ? -1 : level; }
        set { level = value; }
    }

    /// <summary>
    /// The colour of the unit
    /// </summary>
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    /// <summary>
    /// Whether the unit is currently selected.
    /// </summary>
    public bool IsSelected
    {
        get { return selected; }
        set { selected = value; }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Initializes the unit on the passed sector and assigns it to the passed player.
    /// The unit is set to level 1.
    /// The unit's colour is set to the colour of the player that owns it.
    /// </summary>
    /// <param name="player">The player the unit belongs to.</param>
    /// <param name="sector">The sector the unit is on.</param>
    public void Initialize(Player player, Sector sector)
    {
        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        color = owner.Color;

        // set the material color to the player color
        GetComponent<Renderer>().material.color = color;

        // place the unit in the sector
        MoveTo(sector);
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Moves this unit to the passed sector.
    /// If the unit moves to a sector they do not own then LevelUp is called on it.
    /// </summary>
    /// <param name="targetSector">The sector to move this unit to</param>
    public void MoveTo(Sector targetSector)
    {
        // clear the unit's current sector
        if (this.sector != null)
        {
            this.sector.ClearUnit();
        }

        // set the unit's sector to the target sector
        // and the target sector's unit to the unit
        this.sector = targetSector;
        targetSector.Unit = this;
        Transform targetTransform = targetSector.transform.Find("Units").transform;

        // set the unit's transform to be a child of
        // the target sector's transform
        transform.SetParent(targetTransform);

        // align the transform to the sector
        transform.position = targetTransform.position;

        // if the target sector belonged to a different 
        // player than the unit, capture it and level up
        if (targetSector.Owner != this.owner)
        {
            Debug.Log("capuring sector");
            // level up
            LevelUp();

            // capture the target sector for the owner of this unit
            owner.Capture(targetSector);
        }
    }

    /// <summary>
    /// Switch the position of this unit and the passed unit.
    /// </summary>
    /// <param name="otherUnit">The unit to be swapped with this one.</param>
    public void SwapPlacesWith(Unit otherUnit)
    {
        // swap the sectors' references to the units
        this.sector.Unit = otherUnit;
        otherUnit.sector.Unit = this;

        // get the index of this unit's sector in the map's list of sectors
        int tempSectorIndex = -1;
        for (int i = 0; i < this.owner.Game.gameMap.GetComponent<Map>().Sectors.Length; i++)
        {
            if (this.sector == this.owner.Game.gameMap.GetComponent<Map>().Sectors[i])
                tempSectorIndex = i;
        }

        // swap the units' references to their sectors
        this.sector = otherUnit.sector;
        otherUnit.sector = this.owner.Game.gameMap.GetComponent<Map>().Sectors[tempSectorIndex];

        // realign transforms for each unit
        this.transform.SetParent(this.sector.transform.Find("Units").transform);
        this.transform.position = this.sector.transform.Find("Units").position;

        otherUnit.transform.SetParent(otherUnit.sector.transform.Find("Units").transform);
        otherUnit.transform.position = otherUnit.sector.transform.Find("Units").position;
    }

    /// <summary>
    /// Increase this units level and update the unit model to display the new level.
    /// Leveling up is capped at level 5.
    /// </summary>
	public void LevelUp()
    {
        if (level < 5)
        {
            // increase level
            level++;
            UpdateUnitMaterial();
        }
    }

    public void UpdateUnitMaterial()
    {
        switch (level)
        {
            case 2:
                this.gameObject.GetComponent<MeshRenderer>().material = level2Material;
                break;
            case 3:
                this.gameObject.GetComponent<MeshRenderer>().material = level3Material;
                break;
            case 4:
                this.gameObject.GetComponent<MeshRenderer>().material = level4Material;
                break;
            case 5:
                this.gameObject.GetComponent<MeshRenderer>().material = level5Material;
                break;
            default:
                this.gameObject.GetComponent<MeshRenderer>().material = level1Material;
                break;
        }

        // set material color to match owner color
        GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// Select the unit and highlight the sectors adjacent to it.
    /// </summary>
    public void Select()
    {
        selected = true;
        sector.ApplyHighlightAdjacent();
    }

    /// <summary>
    /// Deselect the unit and unhighlight the sectors adjacent to it.
    /// </summary>
    public void Deselect()
    {

        selected = false;
        sector.RevertHighlightAdjacent();
    }

    /// <summary>
    /// Safely destroy the unit by removing it from its owner's list of units before destroying.
    /// </summary>
    public void DestroySelf()
    {
        sector.ClearUnit();
        owner.Units.Remove(this);
        Destroy(this.gameObject);
    }

    #endregion
}

