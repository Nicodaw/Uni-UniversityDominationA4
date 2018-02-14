using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public List <Sector> ownedSectors;
    public List <Unit> units;

    [SerializeField] private Game game;
    [SerializeField] private GameObject unitPrefab;
	[SerializeField] private PlayerUI gui;
    [SerializeField] private int attack = 0;
    [SerializeField] private int defence = 0;
    [SerializeField] private Color color;
    [SerializeField] private bool human;
    [SerializeField] private bool neutral; //added by Peter
    [SerializeField] private bool active = false;

    public Game GetGame() {
        return game;
    }

    public void SetGame(Game game) {
        this.game = game;
    }

    public GameObject GetUnitPrefab() {
        return unitPrefab;
    }

	public PlayerUI GetGui() {
		return gui;
	}

	public void SetGui(PlayerUI gui) {
		this.gui = gui;
	}

    public int GetAttack() {
        return attack;
    }

    public void SetAttack(int beer) {
        this.attack = beer;
    }

    public int GetDefence() {
        return defence;
    }

    public void SetDefence(int knowledge) {
        this.defence = knowledge;
    }

    public Color GetColor() {
        return color;
    }

    public void SetColor(Color color) {
        this.color = color;
    }

    //added by Peter
    public bool IsHuman() {
        return human;
    }
    //added by Peter
    public void SetHuman(bool human) {
        this.human = human;
    }

    public bool IsNeutral()
    {
        return neutral;
    }

    public void SetNeutral(bool neutral)
    {
        this.neutral = neutral;
    }

    public bool IsActive() {
        return active;
    }

    public void SetActive(bool active) {
        this.active = active;
    }

    /// <summary>
    /// Store who controlls the player in the save game
    /// </summary>
    /// <returns>String "human"/"neutral"/"none" depending on the player properties</returns>
    public string GetController()
    {
        if (this.IsHuman())
        {
            return "human";
        }
        else if (this.IsNeutral())
        {
            return "neutral";
        }
        else
        {
            return "none";
        }
    }

    #region Function which gives all owned sectors to the player who defeated this player (Added by Jack 01/02/2018)

    public void Defeat(Player player)
    {
        if (!IsEliminated())
            return; // Incase the player hasn't lost
        foreach (Sector sector in ownedSectors)
        {
            sector.SetOwner(player); // Reset all the sectors
        }
    }

    #endregion


    public void Capture(Sector sector) {

        // capture the given sector


        // store a copy of the sector's previous owner
        Player previousOwner = sector.GetOwner();

        // add the sector to the list of owned sectors
        ownedSectors.Add(sector);

        // remove the sector from the previous owner's
        // list of sectors
        if (previousOwner != null)
            previousOwner.ownedSectors.Remove(sector);

        // set the sector's owner to this player
        sector.SetOwner(this);

        // if the sector contains a landmark
        if (sector.GetLandmark() != null)
        {
            Landmark landmark = sector.GetLandmark();

            // remove the landmark's resource bonus from the previous
            // owner and add it to this player
            if (landmark.GetResourceType() == Landmark.ResourceType.Beer)
            {
                this.attack += landmark.GetAmount();
                if (previousOwner != null)
                    previousOwner.attack -= landmark.GetAmount();
            }
            else if (landmark.GetResourceType() == Landmark.ResourceType.Knowledge)
            {
                this.defence += landmark.GetAmount();
                if (previousOwner != null)
                    previousOwner.defence -= landmark.GetAmount();
            }
        }

        if (sector.isVC())
        {
            // Save current states
            SceneManager.LoadScene(2);
        }

    }

    public void SpawnUnits() {

        // spawn a unit at each unoccupied landmark


        // scan through each owned sector
		foreach (Sector sector in ownedSectors) 
		{
            // if the sector contains a landmark and is unoccupied
            if (sector.GetLandmark() != null && sector.GetUnit() == null)
            {
                // instantiate a new unit at the sector
                Unit newUnit = Instantiate(unitPrefab).GetComponent<Unit>();

                // initialize the new unit
                newUnit.Initialize(this, sector);

                // add the new unit to the player's list of units and 
                // the sector's unit parameters
                units.Add(newUnit);
                sector.SetUnit(newUnit);
            }
		}
	}

    public bool IsEliminated() {

        // returns true if the player is eliminated, false otherwise;
        // a player is considered eliminated if it has no units left
        // and does not own a landmark

        if (units.Count == 0 && !OwnsLandmark())
            return true;
        else
            return false;
    }
    
    private bool OwnsLandmark() {

        // returns true if the player owns at least one landmark,
        // false otherwise

        
        // scan through each owned sector
        foreach (Sector sector in ownedSectors)
        {
            // if a landmarked sector is found, return true
            if (sector.GetLandmark() != null)
                return true;
        }

        // otherwise, return false
        return false;
    }

}