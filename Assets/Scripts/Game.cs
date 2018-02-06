using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {

    public Player[] players;
	public GameObject gameMap;
    public Player currentPlayer;

    public enum TurnState { Move1, Move2, EndOfTurn, NULL };
    [SerializeField] private TurnState turnState;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private bool testMode = false;


    public TurnState GetTurnState() {
        return turnState;
    }

    public void SetTurnState(TurnState turnState) {
        this.turnState = turnState;
    }

    public bool IsFinished() {
        return gameFinished;
    }

    public void EnableTestMode() {
        testMode = true;
    }

    public void DisableTestMode() {
        testMode = false;
    }


    //Re-done by Peter
    public void CreatePlayers(int numberOfPlayers, bool neutralPlayer){

        // ensure that the specified number of players
        // is at least 2 and does not exceed 4
        if (numberOfPlayers < 2)
            numberOfPlayers = 2;

        if (numberOfPlayers > 4) 
            numberOfPlayers = 4;

        // mark the specified number of players as human
        if (!neutralPlayer)
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i].SetHuman(true);
            }
            GameObject.Find("PlayerNeutralUI").SetActive(false);
            players[numberOfPlayers + 1] = GameObject.Find("Player4").GetComponent<Player>();
        }
        else
        {
            for (int i = 0; i < (numberOfPlayers - 1); i++)
            {
                players[i].SetHuman(true);
            }
            players[numberOfPlayers + 1] = GameObject.Find("PlayerNeutral").GetComponent<Player>();
            GameObject.Find("Player4UI").SetActive(false);
            players[numberOfPlayers + 1].SetNeutral(true);
        }


        // give all players a reference to this game
        // and initialize their GUIs
        for (int i = 0; i < 4; i++)
		{
			players[i].SetGame(this);
			players[i].GetGui().Initialize(players[i], i + 1);
		}

    }
    //Modified by Peter to randomly allocate the Vice Chancellor 
	public void InitializeMap() {

        // initialize all sectors, allocate players to landmarks,
        // and spawn units


		// get an array of all sectors
        Sector[] sectors = gameMap.GetComponentsInChildren<Sector>();

		// initialize each sector
        foreach (Sector sector in sectors)
		{
            sector.Initialize();
		}
            
		// get an array of all sectors containing landmarks
        Sector[] landmarkedSectors = GetLandmarkedSectors(sectors);
            
        // ensure there are at least as many landmarks as players
        if (landmarkedSectors.Length < players.Length)
        {
            throw new System.Exception("Must have at least as many landmarks as players; only " + landmarkedSectors.Length.ToString() + " landmarks found for " + players.Length.ToString() + " players.");
        }


		// randomly allocate sectors to players
        foreach (Player player in players) 
		{
			bool playerAllocated = false;
            while (!playerAllocated) {
                
				// choose a landmarked sector at random
                int randomIndex = Random.Range (0, landmarkedSectors.Length);
				
                // if the sector is not yet allocated, allocate the player
                if (((Sector) landmarkedSectors[randomIndex]).GetOwner() == null)
				{
                    player.Capture(landmarkedSectors[randomIndex]);
					playerAllocated = true;
				}

                // retry until player is allocated
			}
		}

		// spawn units for each player
        foreach (Player player in players)
        {
            player.SpawnUnits();
        }

        //set Vice Chancellor
        int rand = Random.Range(0, sectors.Length);
        while (sectors[rand].GetLandmark() != null)
        {
            if (sectors[rand].GetLandmark() == null)
            {
                sectors[rand].setVC(true);
            }
            else
            {
                rand = Random.Range(0, sectors.Length);
            }
        }
    }

    private Sector[] GetLandmarkedSectors(Sector[] sectors) {

        // return a list of all sectors that contain landmarks from the given array

        List<Sector> landmarkedSectors = new List<Sector>();
        foreach (Sector sector in sectors)
        {
            if (sector.GetLandmark() != null)
            {
                landmarkedSectors.Add(sector);
            }
        }

        return landmarkedSectors.ToArray();
    }

    public bool NoUnitSelected() {
        
        // return true if no unit is selected, false otherwise


        // scan through each player
        foreach (Player player in players)
        {
            // scan through each unit of each player
            foreach (Unit unit in player.units)
            {
                // if a selected unit is found, return false
                if (unit.IsSelected() == true)
                    return false;
            }
        }

        // otherwise, return true
        return true;
    }
    //Modified by Peter
    public void NextPlayer() {

        // set the current player to the next player in the order


        // deactivate the current player
        currentPlayer.SetActive(false);
		currentPlayer.GetGui().Deactivate();

        // find the index of the current player
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentPlayer)
            {
                // set the next player's index
                int nextPlayerIndex = i + 1;

                // if end of player list is reached, loop back to the first player
                if (nextPlayerIndex == players.Length)
                {
                    currentPlayer = players[0];
                    players[0].SetActive(true);
					players[0].GetGui().Activate();
                }

                // otherwise, set the next player as the current player
                else
                {
                    currentPlayer = players[nextPlayerIndex];
                    players[nextPlayerIndex].SetActive(true);
					players[nextPlayerIndex].GetGui().Activate();
                    if (currentPlayer.IsNeutral()) {
                        NeutralPlayerTurn();
                        NeutralPlayerTurn(); //Horrible i know
                    }
                    break;
                }
            }
        }
    }
    //Created by Peter
    public void NeutralPlayerTurn() {
        NextTurnState();
        List <Unit> units = currentPlayer.units;
        Unit selectedUnit = units[Random.Range(0, units.Count)];
        Sector[] adjacentSectors = selectedUnit.GetSector().GetAdjacentSectors();
        for (int i = 0; i < adjacentSectors.Length; i++) {
           if (adjacentSectors[i].GetUnit() != null)
                adjacentSectors = adjacentSectors.Where(w => w != adjacentSectors[i]).ToArray();
        }
        selectedUnit.MoveTo(adjacentSectors[Random.Range(0, adjacentSectors.Length)]);
    }
       
    public void NextTurnState() {

        // change the turn state to the next in the order,
        // or to initial turn state if turn is completed

        switch (turnState)
        {
            case TurnState.Move1:
                turnState = TurnState.Move2;
                break;

            case TurnState.Move2:
                turnState = TurnState.EndOfTurn;
                break;

            case TurnState.EndOfTurn:
                turnState = TurnState.Move1;
                break;

            default:
                break;
        }

		UpdateGUI();
    }

    public void EndTurn() {

        // end the current turn

        turnState = TurnState.EndOfTurn;
    }

    public Player GetWinner() {

        // return the winning player, or null if no winner yet

        Player winner = null;

        // scan through each player
        foreach (Player player in players)
        {
            // if the player hasn't been eliminated
            if (!player.IsEliminated())
            {
                // if this is the first player found that hasn't been eliminated,
                // assume the player is the winner
                if (winner == null)
                    winner = player;

                // if another player that was not eliminated was already,
                // found, then return null
                else
                    return null;
            }
        }

        // if only one player hasn't been eliminated, then return it as the winner
        return winner;
    }

    public void EndGame() {
        gameFinished = true;
        currentPlayer.SetActive(false);
        currentPlayer = null;
        turnState = TurnState.NULL;
        Debug.Log("GAME FINISHED");
    }

	public void UpdateGUI() {

		// update all players' GUIs
		for (int i = 0; i < 4; i++) {
			players [i].GetGui ().UpdateDisplay ();
		}
	}
        
    //Modified by Peter
    public void Initialize () {
        
        // initialize the game


        // create a specified number of human players
        // *** currently hard-wired to 3 for testing ***
        CreatePlayers(3, true);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move1;

        // set Player 1 as the current player
        currentPlayer = players[0];
		currentPlayer.GetGui().Activate();
        players[0].SetActive(true);

		// update GUIs
		UpdateGUI();

	}

        
    void Update () {

        // at the end of each turn, check for a winner and end the game if
        // necessary; otherwise, start the next player's turn

		
        // if the current turn has ended and test mode is not enabled
        if (turnState == TurnState.EndOfTurn && !testMode)
        {
            
            // if there is no winner yet
            if (GetWinner() == null)
            {
                // start the next player's turn
                NextPlayer();
                NextTurnState();

                // skip eliminated players
                while (currentPlayer.IsEliminated())
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else
                if (!gameFinished)
                    EndGame();
        }
	}

    public void UpdateAccessible () {

        // copy of Update that can be called by other objects (for testing)

        if (turnState == TurnState.EndOfTurn)
        {
            // if there is no winner yet
            if (GetWinner() == null)
            {
                // start the next player's turn
                NextPlayer();
                NextTurnState();

                // skip eliminated players
                while (currentPlayer.IsEliminated())
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else
                if (!gameFinished)
                    EndGame();
        }
    }

}
