using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* GAME EXECUTABLE http://riskydevelopments.co.uk/ud/UniversityDominationAss3.zip */

[System.Serializable]
public class Game : MonoBehaviour {

    public Player[] players;
    public GameObject gameMap;
    public Player currentPlayer;
    public Sector[] sectors;

    public const int NUMBER_OF_PLAYERS = 4;
    [SerializeField] private TurnState turnState;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private bool testMode = false;
    public string saveFilePath;
    private bool isSaveQuitMenuOpen = false;

    [SerializeField] private UnityEngine.UI.Text actionsRemaining;

    [SerializeField] Dialog dialog;

    public bool triggerDialog = false;

    public bool[] eliminatedPlayers;


    #region Public Properties
    public TurnState TurnState { get { return turnState; } set { turnState = value; } }

    /// <summary>
    /// Returns if the game is finished
    /// </summary>
    public bool IsFinished { get { return IsFinished; } }

    public bool TestModeEnabled { get { return testMode; } set { testMode = value; } }

    public Player[] Players { get { return players; } set { players = value; } }

    public Player CurrentPlayer { get { return currentPlayer; } }

    public Sector[] Sectors { get { return sectors; } }

    public int PVCSectorID { get
        {
            foreach (Sector sector in sectors)
            {
                if (sector.PVC)
                {
                    return Array.IndexOf(sectors, sector);
                }
            }
            return -1;
        } }


    #endregion



    /// <summary>
    /// Triggers the Save and quit dialog
    /// </summary>
    public void OpenSaveQuitMenu()
    {
        if (isSaveQuitMenuOpen)
        {
            dialog.Close();
            isSaveQuitMenuOpen = false;
            return;
        }
        isSaveQuitMenuOpen = true;
        dialog.SetDialogType(Dialog.DialogType.SaveQuit);
        dialog.Show();
    }


    /// <summary>
    /// Initializes a new game
    /// </summary>
    public void Initialize(bool neutralPlayer)
    {
        if (testMode) return;
        #region Setup GUI components (Added by Dom 06/02/2018)
        // initialize the game
        actionsRemaining = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();

        UnityEngine.UI.Button endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        #endregion

        // create a specified number of human players
        // *** currently hard-wired to 2 for testing ***
        CreatePlayers(neutralPlayer);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move1;

        // set Player 1 as the current player
        currentPlayer = players[0];
        currentPlayer.Gui.Activate();
        players[0].Active = true;

        // update GUIs
        UpdateGUI();

    }
    //Re-done by Peter
    /// <summary>
    /// 
    /// Sets up the players in the game
    /// 3 human + 1 neutral if neutral player enabled
    /// 4 human if no neutal player
    /// 
    /// </summary>
    /// <param name="neutralPlayer">True if neutral player enabled else false</param>
    public void CreatePlayers(bool neutralPlayer)
    {
        // mark the specified number of players as human
        if (!neutralPlayer)
        {
            for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
            {
                players[i].Human = true;
            }
            GameObject.Find("PlayerNeutralUI").SetActive(false);
            players[NUMBER_OF_PLAYERS - 1] = GameObject.Find("Player4").GetComponent<Player>();
        }
        else
        {
            for (int i = 0; i < (NUMBER_OF_PLAYERS - 1); i++)
            {
                players[i].Human = true;
            }
            players[NUMBER_OF_PLAYERS - 1] = GameObject.Find("PlayerNeutral").GetComponent<Player>();
            GameObject.Find("Player4UI").SetActive(false);
            players[NUMBER_OF_PLAYERS - 1].Neutral = true;
        }


        // give all players a reference to this game
        // and initialize their GUIs
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i].Game = this;
            players[i].Gui.Initialize(players[i], i + 1);
        }

        eliminatedPlayers = new bool[NUMBER_OF_PLAYERS]; // always 4 players in game
        for (int i = 0; i < eliminatedPlayers.Length; i++)
        {
            eliminatedPlayers[i] = false;
        }
     }

    //modified by Peter
    /// <summary>
    /// 
    /// initialize all sectors, allocate players to landmarks, and spawn units
    /// 
    /// </summary>
    public void InitializeMap() {

		// get an array of all sectors
        sectors = gameMap.GetComponentsInChildren<Sector>();

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
                int randomIndex = UnityEngine.Random.Range (0, landmarkedSectors.Length);
				
                // if the sector is not yet allocated, allocate the player
                if (((Sector) landmarkedSectors[randomIndex]).Owner == null)
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
        int rand = UnityEngine.Random.Range(0, sectors.Length);
        while (sectors[rand].Landmark != null)
            rand = UnityEngine.Random.Range(0, sectors.Length);
        sectors[rand].PVC = true;
        Debug.Log(sectors[rand].name);
    }

    /// <summary>
    /// 
    /// Returns an array of sectors that contain landmarks from the passed array of sectors
    /// 
    /// </summary>
    /// <param name="sectors">Sectors to search though</param>
    /// <returns>Subset of sectors from sectors that contain landmarks</returns>
    private Sector[] GetLandmarkedSectors(Sector[] sectors) {

        // return a list of all sectors that contain landmarks from the given array

        List<Sector> landmarkedSectors = new List<Sector>();
        foreach (Sector sector in sectors)
        {
            if (sector.Landmark != null)
            {
                landmarkedSectors.Add(sector);
            }
        }

        return landmarkedSectors.ToArray();
    }

    /// <summary>
    /// 
    /// Saves this game to file
    /// 
    /// </summary>
    /// <param name="fileName">Name of save game file</param>
    public void SaveGame(string fileName)
    {
        SavedGame.Save(fileName, this);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// 
    /// Returns if there is a unit currently selected
    /// 
    /// </summary>
    /// <returns>True if no unit is seleced else false</returns>
    public bool NoUnitSelected() {
        
        // return true if no unit is selected, false otherwise


        // scan through each player
        foreach (Player player in players)
        {
            // scan through each unit of each player
            foreach (Unit unit in player.units)
            {
                // if a selected unit is found, return false
                if (unit.IsSelected == true)
                    return false;
            }
        }

        // otherwise, return true
        return true;
    }

    /// <summary>
    /// Gets the index of the passed player object in the players array
    /// </summary>
    /// <param name="player">The player to find the index of</param>
    /// <returns>The player objects index in players</returns>
    public int GetPlayerID(Player player)
    {
        return System.Array.IndexOf(players, player);
    }

    /// <summary>
    /// 
    /// Sets the active player to the next player
    /// If it is a neutral player's turn then carries out their actions
    /// 
    /// </summary>
    public void NextPlayer() {
        //SavedGame.Load("test7");

        // set the current player to the next player in the order


        // deactivate the current player
        currentPlayer.Active = false;
        currentPlayer.Gui.Deactivate();

        // find the index of the current player
       
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentPlayer)
            {
                // set the next player's index
                int nextPlayerIndex = (i + 1) % NUMBER_OF_PLAYERS; // set index to next player, loop if end reached
                
                currentPlayer = players[nextPlayerIndex];
                players[nextPlayerIndex].Active = true;
                players[nextPlayerIndex].Gui.Activate();
                if (currentPlayer.Neutral && !currentPlayer.IsEliminated())
                {
                    NeutralPlayerTurn();
                    NeutralPlayerTurn(); 
                }
                break;                
            }
        }
    }

    //Created by Peter
    /// <summary>
    /// 
    /// Carries out the neutral player turn
    /// The neutral player units cannot move to a sector with a unit on
    /// 
    /// </summary>
    public void NeutralPlayerTurn()
    {
        NextTurnState();
        List<Unit> units = currentPlayer.units;
        Unit selectedUnit = units[UnityEngine.Random.Range(0, units.Count)];
        Sector[] adjacentSectors = selectedUnit.Sector.AdjacentSectors;
        List<Sector> possibleSectors = new List<Sector>();
        for (int i = 0; i < adjacentSectors.Length; i++)
        {
            bool neutralOrEmpty = adjacentSectors[i].Owner == null || adjacentSectors[i].Owner.Neutral;
            if (neutralOrEmpty && !adjacentSectors[i].PVC)
                possibleSectors.Add(adjacentSectors[i]);
        }
        if (possibleSectors.Count > 0)
        {
            selectedUnit.MoveTo(possibleSectors[UnityEngine.Random.Range(0, possibleSectors.Count -1)]);
        }
    }

    /// <summary>
    /// 
    /// Advances the turn state to the next state
    /// 
    /// </summary>
    public void NextTurnState() {

        // change the turn state to the next in the order,
        // or to initial turn state if turn is completed

        switch (turnState)
        {
            case TurnState.Move1:
                if (!currentPlayer.hasUnits())
                {
                    turnState = TurnState.EndOfTurn;
                    break;
                }
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

        #region Remove defeated players and check if the game was won (Added by Jack 01/02/2018)

        CheckForDefeatedPlayers();

        Player winner = GetWinner();
        if (winner != null)
        {
            EndGame();
        }

        #endregion

        UpdateGUI();
    }
    //Added by Dom (15/02/2018)
    /// <summary>
    /// 
    /// Updates the text of the Actions Remaining label
    /// 
    /// </summary>
    private void UpdateActionsRemainingLabel()
    {
        switch (turnState)
        {
            case TurnState.Move1:
                actionsRemaining.text = "2";
                break;

            case TurnState.Move2:
                actionsRemaining.text = "1";
                break;

            case TurnState.EndOfTurn:
                actionsRemaining.text = "0";
                break;

            default:
                break;
        }
    }

    #region Function to check for defeated players and notify the others (Added by Jack 01/02/2018)

    /// <summary>
    /// 
    /// Checks if any players were defeated that turn and removes them 
    /// Displays a dialog box showing which players have been defeated this turn
    /// 
    /// </summary>
    public void CheckForDefeatedPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].IsEliminated() && eliminatedPlayers[i] == false)
            {
                // Set up the dialog box and show it
                dialog.SetDialogType(Dialog.DialogType.PlayerElimated);
                dialog.SetDialogData(players[i].name);
                dialog.Show();
                eliminatedPlayers[i] = true; // ensure that the dialog is only shown once
                players[i].Defeat(currentPlayer); // Releases all owned sectors
            }
        }
    }

    #endregion

    /// <summary>
    /// 
    /// sets the turn phase to the EndOfTurn state
    /// 
    /// </summary>
    public void EndTurn() {

        // end the current turn

        turnState = TurnState.EndOfTurn;
    }

    /// <summary>
    /// 
    /// checks if there is a winner in the game
    /// a winner is found when there is only one player with territory remaining, (unclaimed territories are ignored)
    /// 
    /// </summary>
    /// <returns>The winning player object or null if no winner has been found</returns>
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

    /// <summary>
    /// 
    /// Called when the game is over
    /// Displays a Dialog saying which player has won and allows the player to quit the game or restart the game
    /// 
    /// </summary>
    public void EndGame() {
       
        gameFinished = true;

        #region Show the winner dialog (Added by Jack 01/02/2018)

        dialog.SetDialogType(Dialog.DialogType.EndGame);
        dialog.SetDialogData(GetWinner().name);
        dialog.Show();

        #endregion

        currentPlayer.Active = false;
        currentPlayer = null;
        turnState = TurnState.NULL;
        Debug.Log("GAME FINISHED");
    }

    /// <summary>
    /// 
    /// Updates the Player Information GUI components and the Actions remaining label
    /// 
    /// </summary>
	public void UpdateGUI() {

		// update all players' GUIs
		for (int i = 0; i < 4; i++) {
			players [i].Gui.UpdateDisplay();
		}
        UpdateActionsRemainingLabel();
	}
        
    //Added by Dom
    /// <summary>
    /// 
    /// sets up a game with the state stored by the passed GameData
    /// 
    /// </summary>
    /// <param name="savedGame">The saved game state</param>
    public void Initialize(GameData savedGame)
    {
        gameMap = GameObject.Find("Map");

        // initialize the game
        actionsRemaining = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();

        UnityEngine.UI.Button endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        if (savedGame.player4Controller.Equals("human"))
        {
            CreatePlayers(false);
        } else
        {
            CreatePlayers(true);
        }

        // set global game settings
        this.turnState = savedGame.turnState;
        this.gameFinished = savedGame.gameFinished;
        this.testMode = savedGame.testMode;
        this.currentPlayer = players[savedGame.currentPlayerID];
        currentPlayer.Gui.Activate();
        players[savedGame.currentPlayerID].Active = true;

        // set player attack bonus
        players[0].AttackBonus = savedGame.player1Attack;
        players[1].AttackBonus = savedGame.player2Attack;
        players[2].AttackBonus = savedGame.player3Attack;
        players[3].AttackBonus = savedGame.player4Attack;

        // set player defence bonus
        players[0].DefenceBonus = savedGame.player1Defence;
        players[1].DefenceBonus = savedGame.player2Defence;
        players[2].DefenceBonus = savedGame.player3Defence;
        players[3].DefenceBonus = savedGame.player4Defence;

        // set player colour
        players[0].Color = savedGame.player1Color;
        players[1].Color = savedGame.player2Color;
        players[2].Color = savedGame.player3Color;
        players[3].Color = savedGame.player4Color;

        // set player colour
        players[0].SetController(savedGame.player1Controller);
        players[1].SetController(savedGame.player2Controller);
        players[2].SetController(savedGame.player3Controller);
        players[3].SetController(savedGame.player4Controller);

        // get an array of all sectors
        sectors = gameMap.GetComponentsInChildren<Sector>();

        // initialize each sector
        foreach (Sector sector in sectors)
        {
            sector.Initialize();
        }

        // set sector owners
        SetupSectorOwner(0, savedGame.sector01Owner);
        SetupSectorOwner(1, savedGame.sector02Owner);
        SetupSectorOwner(2, savedGame.sector03Owner);
        SetupSectorOwner(3, savedGame.sector04Owner);
        SetupSectorOwner(4, savedGame.sector05Owner);
        SetupSectorOwner(5, savedGame.sector06Owner);
        SetupSectorOwner(6, savedGame.sector07Owner);
        SetupSectorOwner(7, savedGame.sector08Owner);
        SetupSectorOwner(8, savedGame.sector09Owner);
        SetupSectorOwner(9, savedGame.sector10Owner);
        SetupSectorOwner(10, savedGame.sector11Owner);
        SetupSectorOwner(11, savedGame.sector12Owner);
        SetupSectorOwner(12, savedGame.sector13Owner);
        SetupSectorOwner(13, savedGame.sector14Owner);
        SetupSectorOwner(14, savedGame.sector15Owner);
        SetupSectorOwner(15, savedGame.sector16Owner);
        SetupSectorOwner(16, savedGame.sector17Owner);
        SetupSectorOwner(17, savedGame.sector18Owner);
        SetupSectorOwner(18, savedGame.sector19Owner);
        SetupSectorOwner(19, savedGame.sector20Owner);
        SetupSectorOwner(20, savedGame.sector21Owner);
        SetupSectorOwner(21, savedGame.sector22Owner);
        SetupSectorOwner(22, savedGame.sector23Owner);
        SetupSectorOwner(23, savedGame.sector24Owner);
        SetupSectorOwner(24, savedGame.sector25Owner);
        SetupSectorOwner(25, savedGame.sector26Owner);
        SetupSectorOwner(26, savedGame.sector27Owner);
        SetupSectorOwner(27, savedGame.sector28Owner);
        SetupSectorOwner(28, savedGame.sector29Owner);
        SetupSectorOwner(29, savedGame.sector30Owner);
        SetupSectorOwner(30, savedGame.sector31Owner);
        SetupSectorOwner(31, savedGame.sector32Owner);

        // set unit level in sectors
        SetupUnit(0, savedGame.sector01Level);
        SetupUnit(1, savedGame.sector02Level);
        SetupUnit(2, savedGame.sector03Level);
        SetupUnit(3, savedGame.sector04Level);
        SetupUnit(4, savedGame.sector05Level);
        SetupUnit(5, savedGame.sector06Level);
        SetupUnit(6, savedGame.sector07Level);
        SetupUnit(7, savedGame.sector08Level);
        SetupUnit(8, savedGame.sector09Level);
        SetupUnit(9, savedGame.sector10Level);
        SetupUnit(10, savedGame.sector11Level);
        SetupUnit(11, savedGame.sector12Level);
        SetupUnit(12, savedGame.sector13Level);
        SetupUnit(13, savedGame.sector14Level);
        SetupUnit(14, savedGame.sector15Level);
        SetupUnit(15, savedGame.sector16Level);
        SetupUnit(16, savedGame.sector17Level);
        SetupUnit(17, savedGame.sector18Level);
        SetupUnit(18, savedGame.sector19Level);
        SetupUnit(19, savedGame.sector20Level);
        SetupUnit(20, savedGame.sector21Level);
        SetupUnit(21, savedGame.sector22Level);
        SetupUnit(22, savedGame.sector23Level);
        SetupUnit(23, savedGame.sector24Level);
        SetupUnit(24, savedGame.sector25Level);
        SetupUnit(25, savedGame.sector26Level);
        SetupUnit(26, savedGame.sector27Level);
        SetupUnit(27, savedGame.sector28Level);
        SetupUnit(28, savedGame.sector29Level);
        SetupUnit(29, savedGame.sector30Level);
        SetupUnit(30, savedGame.sector31Level);
        SetupUnit(31, savedGame.sector32Level);

        //set VC sector
        if (savedGame.VCSector != -1) sectors[savedGame.VCSector].PVC = true;

        UpdateGUI();

    }

    //Added by Dom
    /// <summary>
    /// 
    /// sets the sector owner, if it has one
    /// 
    /// </summary>
    /// <param name="sectorId">id of sector being set</param>
    /// <param name="ownerId">id of player</param>
    private void SetupSectorOwner(int sectorId, int ownerId)
    {
        if (ownerId == -1)
        {
            return;
        }
        Player p = players[ownerId];
        sectors[sectorId].Owner = p;
        p.ownedSectors.Add(sectors[sectorId]);
    }
    //Added by Dom
    /// <summary>
    /// 
    /// sets up the units on the passed sector
    /// 
    /// </summary>
    /// <param name="sectorIndex">Sector id of sector being setup</param>
    /// <param name="level">unit level on sector; -1 if no unit on this sector</param>
    private void SetupUnit(int sectorIndex, int level)
    {
        if (level == -1)
        {
            return;
        }
        Unit unit = Instantiate(sectors[sectorIndex].Owner.UnitPrefab).GetComponent<Unit>();
        unit.Initialize(sectors[sectorIndex].Owner, sectors[sectorIndex]);
        unit.Level = level;
        unit.UpdateUnitMaterial();
        unit.MoveTo(sectors[sectorIndex]);
        sectors[sectorIndex].Owner.units.Add(unit);
    }

    /// <summary>
    /// 
    /// calls UpdateAccessible
    /// 
    /// </summary>
    void Update () {
        UpdateAccessible();
	}

    /// <summary>
    /// 
    /// at the end of each turn, check for a winner and end the game if necessary; otherwise, start the next player's turn 
    /// exposed version of update method so accessible for testing
    /// 
    /// </summary>
    public void UpdateAccessible ()
    {
        if (triggerDialog)
        {
            triggerDialog = false;
            dialog.SetDialogType(Dialog.DialogType.SaveQuit);
            dialog.SetDialogData("PLAYER 1");
            dialog.Show();
        }
        // if the current turn has ended and test mode is not enabled
        if (turnState == TurnState.EndOfTurn)
        {

            // if there is no winner yet
            if (GetWinner() == null)
            {
                // start the next player's turn
                // Swapped by Jack
                NextTurnState();
                NextPlayer();

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
    
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 
    /// Allocates a reward to the player when they complete the mini game
    /// Reward = (Number of coins collected + 1) / 2 added to attack and defence bonus
    /// 
    /// </summary>
    internal void GiveReward()
    {
        int rewardLevel = PlayerPrefs.GetInt("_mgScore");
        // REWARD TO BE ADDED TO PLAYER
        int bonus = (int)Math.Floor((double)(rewardLevel+1)/2); 
        currentPlayer.AttackBonus = currentPlayer.AttackBonus + bonus;
        currentPlayer.DefenceBonus = currentPlayer.DefenceBonus + bonus;

        dialog.SetDialogType(Dialog.DialogType.ShowText);

        dialog.SetDialogData("REWARD!", string.Format("Well done, you have gained:\n+{0} Attack\n+{0} Defence", bonus));

        dialog.Show();

        UpdateGUI(); // update GUI with new bonuses

        Debug.Log("Player " + (Array.IndexOf(players, currentPlayer) + 1) + " has won " + bonus + " points");
    }


}
