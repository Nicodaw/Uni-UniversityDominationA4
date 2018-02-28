using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{

    #region Unity Bindings

    public Game game;
    public GameObject unitPrefab;
    public PlayerUI gui;
    public Color color;

    #endregion

    #region Private Fields

    List<Sector> ownedSectors;
    List<Unit> units;
    int attack = 0;
    int defence = 0;
    bool human;
    bool neutral;
    bool active = false;

    #endregion

    #region Public Properties

    /// <summary>
    /// The reference to the current <see cref="Game"/> object.
    /// </summary>
    public Game Game
    {
        get { return game; }
        set { game = value; }
    }

    /// <summary>
    /// The <see cref="Unit"/> prefab object attached to this player.
    /// </summary>
    public GameObject UnitPrefab
    {
        get { return unitPrefab; }
    }

    /// <summary>
    /// The <see cref="PlayerUI"/> object attached to this player.
    /// </summary>
    public PlayerUI Gui
    {
        get { return gui; }
        set { gui = value; }
    }

    public List<Sector> OwnedSectors
    {
        get { return ownedSectors; }
    }

    public List<Unit> Units
    {
        get { return units; }
    }

    /// <summary>
    /// The amount of Attack bonus the player currently has.
    /// </summary>
    public int AttackBonus
    {
        get { return attack; }
        set { attack = value; }
    }

    /// <summary>
    /// The amount of Defence bonus the player currently has.
    /// </summary>
    public int DefenceBonus
    {
        get { return defence; }
        set { defence = value; }
    }

    /// <summary>
    /// The player's color.
    /// </summary>
    public Color Color
    {
        get { return color; }
        set { color = value; }
    }

    /// <summary>
    /// Whether this player is human or AI.
    /// </summary>
    public bool Human
    {
        get { return human; }
        set { human = value; }
    }

    /// <summary>
    /// Whether the neutral player is enabled.
    /// </summary>
    public bool Neutral
    {
        get { return neutral; }
        set { neutral = value; }
    }

    /// <summary>
    /// Whether the player is currently active.
    /// </summary>
    public bool Active
    {
        get { return active; }
        set { active = value; }
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Store who controlls the player in the save game.
    /// </summary>
    /// <returns>String "human"/"neutral"/"none" depending on the player properties.</returns>
    [Obsolete("Will be removed/reworked after memento pattern implementation.")]
    public string GetController()
    {
        if (Human)
        {
            return "human";
        }
        else if (Neutral)
        {
            return "neutral";
        }
        else
        {
            return "none";
        }
    }

    /// <summary>
    /// Sets how this player is controlled.
    /// </summary>
    /// <param name="controller">'human' if player controlled by human; 'neutral' if player controlled by neutral AI; all other values have no contoller</param>
    [Obsolete("Will be removed/reworked after memento pattern implementation.")]
    public void SetController(String controller)
    {
        if (controller.Equals("human"))
        {
            human = true;
            neutral = false;
        }
        else if (controller.Equals("neutral"))
        {
            human = false;
            neutral = true;
        }
        else
        {
            human = false;
            neutral = false;
        }
    }

    /// <summary>
    /// Called when this player is eliminated in order to pass all sectors owned
    /// by this player to the player that eliminated this player.
    /// </summary>
    /// <param name="player">The player to recieve all of this player's sectors.</param>
    public void Defeat(Player player)
    {
        if (!IsEliminated())
            return; // Incase the player hasn't lost
        foreach (Sector sector in OwnedSectors)
        {
            sector.Owner = player; // Reset all the sectors
        }
    }

    /// <summary>
    /// Called when this player captures a sector.
    /// Updates this players attack and defence bonuses.
    /// Sets the sectors owner and updates its colour.
    /// </summary>
    /// <param name="sector">The sector that is being captured by this player.</param>
    public void Capture(Sector sector)
    {
        // store a copy of the sector's previous owner
        Player previousOwner = sector.Owner;

        // add the sector to the list of owned sectors
        OwnedSectors.Add(sector);

        // remove the sector from the previous owner's
        // list of sectors
        if (previousOwner != null)
            previousOwner.OwnedSectors.Remove(sector);

        // set the sector's owner to this player
        sector.Owner = this;

        // if the sector contains a landmark
        if (sector.Landmark != null)
        {
            Landmark landmark = sector.Landmark;

            // remove the landmark's resource bonus from the previous
            // owner and add it to this player
            if (landmark.Resource == ResourceType.Attack)
            {
                this.attack += landmark.Amount;
                if (previousOwner != null)
                    previousOwner.attack -= landmark.Amount;
            }
            else if (landmark.Resource == ResourceType.Defence)
            {
                this.defence += landmark.Amount;
                if (previousOwner != null)
                    previousOwner.defence -= landmark.Amount;
            }
        }

        if (sector.HasPVC)
        {
            game.NextTurnState(); // update turn mode before game is saved
            sector.HasPVC = false; // set VC to false so game can only be triggered once
            SavedGame.Save("_tmp", game);
            SceneManager.LoadScene(2);
        }
    }

    /// <summary>
    /// Spawns a unit at each unoccupied sector containing a landmark owned by this player.
    /// </summary>
    public void SpawnUnits()
    {
        // scan through each owned sector
        foreach (Sector sector in OwnedSectors)
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
                Units.Add(newUnit);
                sector.Unit = newUnit;
            }
        }
    }

    /// <summary>
    /// Checks if the player has any units or if they own any landmarks.
    /// If they have neither then they have been eliminated.
    /// </summary>
    /// <returns>True if the player has no units and landmarks else false.</returns>
    public bool IsEliminated()
    {
        if (Units.Count == 0 && !OwnsLandmark())
            return true;
        return false;
    }

    public bool HasUnits()
    {
        return Units.Count > 0;
    }

    /// <summary>
    /// Returns true if any of the sectors the player owns contains a landmark.
    /// </summary>
    /// <returns>True if the player owns 1 or more landmarks else false</returns>
    bool OwnsLandmark()
    {
        // scan through each owned sector
        foreach (Sector sector in OwnedSectors)
        {
            // if a landmarked sector is found, return true
            if (sector.Landmark != null)
                return true;
        }
        // otherwise, return false
        return false;
    }

    #endregion
}