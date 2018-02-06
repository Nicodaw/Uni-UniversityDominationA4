using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class GameTest 
{
    private Game game;
    private Map map;
    private Player[] players;
	private PlayerUI[] gui;

	[UnityTest]
	public IEnumerator CreatePlayers_TwoPlayersAreHumanAndTwoNot() {
        
        Setup();

        // ensure creation of 2 players is accurate
        game.GetComponent<Game>().CreatePlayers(2, false);
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsFalse(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsFalse(game.GetComponent<Game>().players[3].IsHuman());

        yield return null;
    }

    [UnityTest]
    public IEnumerator CreatePlayers_ThreePlayersAreHumanAndOneNot() {
        
        Setup();

        // ensure creation of 3 players is accurate
        game.GetComponent<Game>().CreatePlayers(3, false);
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsFalse(game.GetComponent<Game>().players[3].IsHuman());

        yield return null;
    }

    [UnityTest]
    public IEnumerator CreatePlayers_FourPlayersAreHuman() {
        
        Setup();

        // ensure creation of 4 players is accurate
        game.GetComponent<Game>().CreatePlayers(4, false);
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[3].IsHuman());

        yield return null;
    }

    [UnityTest]
    public IEnumerator CreatePlayers_AtLeastTwoPlayersAreHuman() {
        
        Setup();

        // ensure that at least 2 players are created no matter what
        game.GetComponent<Game>().CreatePlayers(0, false);
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsFalse(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsFalse(game.GetComponent<Game>().players[3].IsHuman());

        yield return null;
    }

    [UnityTest]
    public IEnumerator CreatePlayers_AtMostFourPlayersAreHuman() {
        
        Setup();

        // ensure that at most 4 players are created no matter what
        game.GetComponent<Game>().CreatePlayers(5, false);
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[3].IsHuman());

		yield return null;
	}

    [UnityTest]
    public IEnumerator InitializeMap_OneLandmarkAllocatedWithUnitPerPlayer() {
        
        // MAY BE MADE OBSELETE BY TESTS OF THE INDIVIDUAL METHODS
        Setup();
        game.InitializeMap();

        // ensure that each player owns 1 sector and has 1 unit at that sector
        List<Sector> listOfAllocatedSectors = new List<Sector>();
        foreach (Player player in players)
        {
            Assert.IsTrue(player.ownedSectors.Count == 1);
            Assert.IsNotNull(player.ownedSectors[0].GetLandmark());
            Assert.IsTrue(player.units.Count == 1);

            Assert.AreSame(player.ownedSectors[0], player.units[0].GetSector());

            listOfAllocatedSectors.Add(player.ownedSectors[0]);
        }


        foreach (Sector sector in map.sectors)
        {
            if (sector.GetOwner() != null && !listOfAllocatedSectors.Contains(sector)) // any sector that has an owner but is not in the allocated sectors from above
            {
                Assert.Fail(); // must be an error as only sectors owned should be landmarks from above
            }
        }

        yield return null;    
    }

    [UnityTest]
    public IEnumerator NoUnitSelected_ReturnsFalseWhenUnitIsSelected() {
        
        Setup();
        game.Initialize();

        // clear any selected units
        foreach (Player player in game.players)
        {
            foreach (Unit unit in player.units)
            {
                unit.SetSelected(false);
            }
        }

        // assert that NoUnitSelected returns true
        Assert.IsTrue(game.NoUnitSelected());

        // select a unit
        players[0].units[0].SetSelected(true);

        // assert that NoUnitSelected returns false
        Assert.IsFalse(game.NoUnitSelected());

        yield return null;
    }


    [UnityTest]
    public IEnumerator NextPlayer_CurrentPlayerChangesToNextPlayerEachTime() {
        
        Setup();

        Player playerA = players[0];
        Player playerB = players[1];
        Player playerC = players[2];
        Player playerD = players[3];

        // set the current player to the first player
        game.currentPlayer = playerA;
        playerA.SetActive(true);

        // ensure that NextPlayer changes the current player
        // from player A to player B
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerB);
        Assert.IsFalse(playerA.IsActive());
        Assert.IsTrue(playerB.IsActive());

        // ensure that NextPlayer changes the current player
        // from player B to player C
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerC);
        Assert.IsFalse(playerB.IsActive());
        Assert.IsTrue(playerC.IsActive());

        // ensure that NextPlayer changes the current player
        // from player C to player D
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerD);
        Assert.IsFalse(playerC.IsActive());
        Assert.IsTrue(playerD.IsActive());

        // ensure that NextPlayer changes the current player
        // from player D to player A
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerA);
        Assert.IsFalse(playerD.IsActive());
        Assert.IsTrue(playerA.IsActive());

        yield return null;
    }

    [UnityTest]
    public IEnumerator NextPlayer_EliminatedPlayersAreSkipped() {
        
        Setup();

        Player playerA = players[0];
        Player playerB = players[1];
        Player playerC = players[2];
        Player playerD = players[3];

        game.currentPlayer = playerA;

        playerC.units.Add(MonoBehaviour.Instantiate(playerC.GetUnitPrefab()).GetComponent<Unit>()); // make player C not eliminated
        playerD.units.Add(MonoBehaviour.Instantiate(playerD.GetUnitPrefab()).GetComponent<Unit>()); // make player D not eliminated

        game.SetTurnState(Game.TurnState.EndOfTurn);
        game.UpdateAccessible(); // removes players that should be eliminated (A and B)

        // ensure eliminated players are skipped
        Assert.IsTrue(game.currentPlayer == playerC);
        Assert.IsFalse(playerA.IsActive());
        Assert.IsFalse(playerB.IsActive());
        Assert.IsTrue(playerC.IsActive());

        yield return null;
    }


    [UnityTest]
    public IEnumerator NextTurnState_TurnStateProgressesCorrectly() {
        
        Setup();

        // initialize turn state to Move1
        game.SetTurnState(Game.TurnState.Move1);

        // ensure NextTurnState changes the turn state
        // from Move1 to Move2
        game.NextTurnState();
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move2);

        // ensure NextTurnState changes the turn state
        // from Move2 to EndOfTurn
        game.NextTurnState();
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        // ensure NextTurnState changes the turn state
        // from EndOfTurn to Move1
        game.NextTurnState();
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move1);

        // ensure NextTurnState does not change turn state
        // if the current turn state is NULL
        game.SetTurnState(Game.TurnState.NULL);
        game.NextTurnState();
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.NULL);

        yield return null;
    }
        
    [UnityTest]
    public IEnumerator GetWinner_OnePlayerWithLandmarksAndUnitsWins() {
        
        Setup();

        Sector landmark1 = map.sectors[1];
        Player playerA = players[0];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.IsNotNull(landmark1.GetLandmark());

        // ensure winner is found if only 1 player owns a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerA.units.Add(MonoBehaviour.Instantiate(playerA.GetUnitPrefab()).GetComponent<Unit>());
        Assert.IsNotNull(game.GetWinner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetWinner_NoWinnerWhenMultiplePlayersOwningLandmarks() {
        
        Setup();

        Sector landmark1 = map.sectors[1];
        Sector landmark2 = map.sectors[7];
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure'landmark1' and 'landmark2' are landmarks
        landmark1.Initialize();
        landmark2.Initialize();
        Assert.IsNotNull(landmark1.GetLandmark());
        Assert.IsNotNull(landmark2.GetLandmark());

        // ensure no winner is found if >1 players own a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerB.ownedSectors.Add(landmark2);
        Assert.IsNull(game.GetWinner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetWinner_NoWinnerWhenMultiplePlayersWithUnits() {
        
        Setup();

        Player playerA = players[0];
        Player playerB = players[1];

        // ensure no winner is found if >1 players have a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.units.Add(MonoBehaviour.Instantiate(playerA.GetUnitPrefab()).GetComponent<Unit>());
        playerB.units.Add(MonoBehaviour.Instantiate(playerB.GetUnitPrefab()).GetComponent<Unit>());
        Assert.IsNull(game.GetWinner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetWinner_NoWinnerWhenAPlayerHasLandmarkAndAnotherHasUnits() {
        
        Setup();

        Sector landmark1 = map.sectors[1];
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.IsNotNull(landmark1.GetLandmark());

        // ensure no winner is found if 1 player has a landmark
        // and another player has a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerB.units.Add(MonoBehaviour.Instantiate(playerB.GetUnitPrefab()).GetComponent<Unit>());
        Assert.IsNull(game.GetWinner());

        yield return null;
    }
        
    [UnityTest]
    public IEnumerator EndGame_GameEndsCorrectlyWithNoCurrentPlayerAndNoActivePlayersAndNoTurnState() {
        
        Setup();
        game.currentPlayer = game.players[0];
        game.EndGame();

        // ensure the game is marked as finished
        Assert.IsTrue(game.IsFinished());

        // ensure the current player is null
        Assert.IsNull(game.currentPlayer);

        // ensure no players are active
        foreach (Player player in game.players)
            Assert.IsFalse(player.IsActive());

        // ensure turn state is NULL
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.NULL);

        yield return null;
    }


    private void Setup() {
        
        // initialize the game, map, and players with any references needed
        // the "GameManager" asset contains a copy of the GameManager object
        // in the 4x4 Test, but its script lacks references to players & the map
        game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager")).GetComponent<Game>();

        // the "Map" asset is a copy of the 4x4 Test map, complete with
        // adjacent sectors and landmarks at (0,1), (1,3), (2,0), and (3,2),
        // but its script lacks references to the game & sectors
        map = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Map")).GetComponent<Map>();

        // the "Players" asset contains 4 prefab Player game objects; only
        // references not in its script is each player's color
        players = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Players")).GetComponentsInChildren<Player>();
		   
		// the "GUI" asset contains the PlayerUI object for each Player
		gui = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GUI")).GetComponentsInChildren<PlayerUI>();

		// the "Scenery" asset contains the camera and light source of the 4x4 Test
		// can uncomment to view scene as tests run, but significantly reduces speed
		//MonoBehaviour.Instantiate(Resources.Load<GameObject>("Scenery"));

        // establish references from game to players & map
        game.players = players;
        game.gameMap = map.gameObject;
         
        // establish references from map to game & sectors (from children)
        map.game = game;
        map.sectors = map.gameObject.GetComponentsInChildren<Sector>();

        // establish references to SSB 64 colors for each player
        players[0].SetColor(Color.red);
        players[1].SetColor(Color.blue);
        players[2].SetColor(Color.yellow);
        players[3].SetColor(Color.green);

		// establish references to a PlayerUI and Game for each player & initialize GUI
		for (int i = 0; i < players.Length; i++) 
		{
			players[i].SetGui(gui[i]);
			players[i].SetGame(game);
			players[i].GetGui().Initialize(players[i], i + 1);
		}

        // enable game's test mode
        game.EnableTestMode();
    }

    private void ClearSectorsAndUnitsOfAllPlayers() {
        
        foreach (Player player in game.players)
        {
            ClearSectorsAndUnits(player);
        }
    }

    private void ClearSectorsAndUnits(Player player) {
        
        player.units = new List<Unit>();
        player.ownedSectors = new List<Sector>();
    }
}