using System;
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

    /// <summary>
    /// 
    /// gets this player's attack bonus
    /// 
    /// </summary>
    /// <returns>This player's attack bonus</returns>
    public int GetAttack() {
        return attack;
    }

    /// <summary>
    /// 
    /// sets this players attack bonus
    /// 
    /// </summary>
    /// <param name="attack">The player's attack bonus</param>
    public void SetAttack(int attack) {
        this.attack = attack;
    }

    /// <summary>
    /// 
    /// gets this player's defence
    /// 
    /// </summary>
    /// <returns>The player's defence bonus</returns>
    public int GetDefence() {
        return defence;
    }

    /// <summary>
    /// 
    /// sets this players defence bonus
    /// 
    /// </summary>
    /// <param name="defence">The player's defence bonus</param>
    public void SetDefence(int defence) {
        this.defence = defence;
    }
    
    /// <summary>
    /// 
    /// gets the colour associated with this player
    /// 
    /// </summary>
    /// <returns>The colour associated with this player</returns>
    public Color GetColor() {
        return color;
    }

    /// <summary>
    /// 
    /// sets the players colour to be used for sector colouring and setting the player's name colour
    /// 
    /// </summary>
    /// <param name="color">The colour to be used for this player</param>
    public void SetColor(Color color) {
        this.color = color;
    }

    //added by Peter
    /// <summary>
    /// 
    /// returns if this player is controlled by a human or not
    /// 
    /// </summary>
    /// <returns>True if the player is controlled by a human else false</returns>
    public bool IsHuman() {
        return human;
    }

    //added by Peter
    /// <summary>
    /// 
    /// sets if this player is to be controlled by a human
    /// 
    /// </summary>
    /// <param name="human">True if the player is to be contolled by a human else false</param>
    public void SetHuman(bool human) {
        this.human = human;
    }

    /// <summary>
    /// 
    /// returns true if this player is neutral else false
    /// 
    /// </summary>
    /// <returns>True if player is neutral else false</returns>
    public bool IsNeutral()
    {
        return neutral;
    }

    /// <summary>
    /// 
    /// Sets if this player should be controlled by the neutral AI
    /// 
    /// </summary>
    /// <param name="neutral">True if the player is neutral else false</param>
    public void SetNeutral(bool neutral)
    {
        this.neutral = neutral;
    }

    /// <summary>
    /// 
    /// returns if the player is active or not
    /// 
    /// </summary>
    /// <returns>True if player is active else false</returns>
    public bool IsActive() {
        return active;
    }

    /// <summary>
    /// 
    /// sets if this player is active
    /// if it is a player's turn they are active else they are not
    /// 
    /// </summary>
    /// <param name="active">True if player is active else false</param>
    public void SetActive(bool active) {
        this.active = active;
    }

    /// <summary>
    /// 
    /// Store who controlls the player in the save game
    /// 
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

    /// <summary>
    /// 
    /// sets how this player is controlled
    /// 
    /// </summary>
    /// <param name="controller">'human' if player controlled by human; 'neutral' if player controlled by neutral AI; all other values have no contoller</param>
    public void SetController(String controller)
    {
        if (controller.Equals("human"))
        {
            human = true;
            neutral = false;
        } else if (controller.Equals("neutral"))
        {
            human = false;
            neutral = true;
        } else
        {
            human = false;
            neutral = false;
        }
    }

    #region Function which gives all owned sectors to the player who defeated this player (Added by Jack 01/02/2018)

    /// <summary>
    /// 
    /// called when this player is eliminated in order to pass all sectors owned by this player to the player that eliminated this player
    /// 
    /// </summary>
    /// <param name="player">The player to recieve all of this player's sectors</param>
    public void Defeat(Player player)
    {
        if (!IsEliminated())
            return; // Incase the player hasn't lost
        foreach (Sector sector in ownedSectors)
        {
            sector.Owner = player; // Reset all the sectors
        }
    }

    #endregion


    /// <summary>
    /// 
    /// called when this player captures a sector 
    /// updates this players attack and defence bonuses
    /// sets the sectors owner and updates its colour
    /// 
    /// </summary>
    /// <param name="sector">The sector that is being captured by this player</param>
    public void Capture(Sector sector) {

        // capture the given sector


        // store a copy of the sector's previous owner
        Player previousOwner = sector.Owner;

        // add the sector to the list of owned sectors
        ownedSectors.Add(sector);

        // remove the sector from the previous owner's
        // list of sectors
        if (previousOwner != null)
            previousOwner.ownedSectors.Remove(sector);

        // set the sector's owner to this player
        sector.Owner = this;

        // if the sector contains a landmark
        if (sector.Landmark != null)
        {
            Landmark landmark = sector.Landmark;

            // remove the landmark's resource bonus from the previous
            // owner and add it to this player
            if (landmark.GetResourceType() == Landmark.ResourceType.Attack)
            {
                this.attack += landmark.GetAmount();
                if (previousOwner != null)
                    previousOwner.attack -= landmark.GetAmount();
            }
            else if (landmark.GetResourceType() == Landmark.ResourceType.Defence)
            {
                this.defence += landmark.GetAmount();
                if (previousOwner != null)
                    previousOwner.defence -= landmark.GetAmount();
            }
        }

        if (sector.PVC)
        {
            game.NextTurnState(); // update turn mode before game is saved
            sector.PVC = false; // set VC to false so game can only be triggered once
            SavedGame.Save("_tmp", game);
            SceneManager.LoadScene(2); 

        }


    }

    /// <summary>
    /// 
    /// spawns a unit at each unoccupied sector containing a landmark owned by this player
    /// 
    /// </summary>
    public void SpawnUnits() {
        // scan through each owned sector
		foreach (Sector sector in ownedSectors) 
		{
            // if the sector contains a landmark and is unoccupied
            if (sector.Landmark != null && sector.Unit == null)
            {
                // instantiate a new unit at the sector
                Unit newUnit = Instantiate(unitPrefab).GetComponent<Unit>();

                // initialize the new unit
                newUnit.Initialize(this, sector);

                // add the new unit to the player's list of units and 
                // the sector's unit parameters
                units.Add(newUnit);
                sector.Unit = newUnit;
            }
		}
	}

    /// <summary>
    /// 
    /// checks if the player has any units or if they own any landmarks
    /// if they have neither then they have been eliminated
    /// 
    /// </summary>
    /// <returns>true if the player has no units and landmarks else false</returns>
    public bool IsEliminated() {
        if (units.Count == 0 && !OwnsLandmark())
            return true;
        else
            return false;
    }
    
    public bool hasUnits()
    {
        return units.Count > 0;
    }

    /// <summary>
    /// 
    /// returns true if any of the sectors the player owns contains a landmark
    /// 
    /// </summary>
    /// <returns>true if the player owns 1 or more landmarks else false</returns>
    private bool OwnsLandmark() {

        // scan through each owned sector
        foreach (Sector sector in ownedSectors)
        {
            // if a landmarked sector is found, return true
            if (sector.Landmark != null)
                return true;
        }

        // otherwise, return false
        return false;
    }
    
}