#if false
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GameTest : BaseGameTest
{
    [Test]
    public void CreatePlayers_FourPlayersAreHuman()
    {
        game.CreatePlayers(false);
        // ensure creation of 4 players is accurate
        Assert.That(Players[0].Human);
        Assert.That(Players[1].Human);
        Assert.That(Players[2].Human);
        Assert.That(Players[3].Human);
    }

    // Test added by Owain
    [Test]
    public void CreatePlayers_ThreePlayersHumanAndOneNeutral()
    {
        game.CreatePlayers(true);

        // ensure game with three players and one neutral is accurate
        Assert.That(Players[0].Human);
        Assert.That(Players[1].Human);
        Assert.That(Players[2].Human);
        Assert.That(Players[3].Neutral);
    }

    [Test]
    public void InitializeMap_OneLandmarkAllocatedWithUnitPerPlayer()
    {
        // MAY BE MADE OBSELETE BY TESTS OF THE INDIVIDUAL METHODS
        game.InitializeMap();

        // ensure that each player owns 1 sector and has 1 unit at that sector
        List<Sector> listOfAllocatedSectors = new List<Sector>();
        foreach (Player player in Players)
        {
            Assert.That(player.OwnedSectors.Count, Is.EqualTo(1));
            Assert.That(player.OwnedSectors[0].Landmark, Is.Not.Null);
            Assert.That(player.Units.Count, Is.EqualTo(1));

            Assert.That(player.OwnedSectors[0], Is.EqualTo(player.Units[0].Sector));

            listOfAllocatedSectors.Add(player.OwnedSectors[0]);
        }

        var search =
            from s in map.Sectors
            where s.Owner != null && !listOfAllocatedSectors.Contains(s)// any sector that has an owner but is not in the allocated sectors from above
            select s;

        Assert.That(search.Count(), Is.EqualTo(0));// must be an error as only sectors owned should be landmarks from above
    }

    [Test]
    public void NoUnitSelected_ReturnsFalseWhenUnitIsSelected()
    {
        game.Initialize(false);

        // clear any selected units
        foreach (Player player in game.players)
            foreach (Unit unit in player.Units)
                unit.IsSelected = false;

        // assert that NoUnitSelected returns true
        Assert.That(game.NoUnitSelected());

        // select a unit
        Players[0].Units[0].IsSelected = true;

        // assert that NoUnitSelected returns false
        Assert.That(game.NoUnitSelected(), Is.False);
    }

    [Test]
    public void NextPlayer_CurrentPlayerChangesToNextPlayerEachTime()
    {
        Player playerA = Players[0];
        Player playerB = Players[1];
        Player playerC = Players[2];
        Player playerD = Players[3];

        // set the current player to the first player
        game.currentPlayer = playerA;
        playerA.Active = true;

        // ensure that NextPlayer changes the current player
        // from player A to player B
        game.NextPlayer();
        Assert.That(game.currentPlayer, Is.EqualTo(playerB));
        Assert.That(playerA.Active, Is.False);
        Assert.That(playerB.Active);

        // ensure that NextPlayer changes the current player
        // from player B to player C
        game.NextPlayer();
        Assert.That(game.currentPlayer, Is.EqualTo(playerC));
        Assert.That(playerB.Active, Is.False);
        Assert.That(playerC.Active);

        // ensure that NextPlayer changes the current player
        // from player C to player D
        game.NextPlayer();
        Assert.That(game.currentPlayer, Is.EqualTo(playerD));
        Assert.That(playerC.Active, Is.False);
        Assert.That(playerD.Active);

        // ensure that NextPlayer changes the current player
        // from player D to player A
        game.NextPlayer();
        Assert.That(game.currentPlayer, Is.EqualTo(playerA));
        Assert.That(playerD.Active, Is.False);
        Assert.That(playerA.Active);
    }

    [UnityTest]
    public IEnumerator NextPlayer_EliminatedPlayersAreSkipped()
    {
        Player playerA = Players[0];
        Player playerB = Players[1];
        Player playerC = Players[2];
        Player playerD = Players[3];

        game.currentPlayer = playerA;

        playerC.Units.Add(InitUnit(2)); // make player C not eliminated
        playerD.Units.Add(InitUnit(3)); // make player D not eliminated

        game.TurnState = TurnState.EndOfTurn;
        yield return null; // wait for next Update()

        // ensure eliminated players are skipped
        Assert.That(game.currentPlayer, Is.EqualTo(playerC));
        Assert.That(playerA.Active, Is.False);
        Assert.That(playerB.Active, Is.False);
        Assert.That(playerC.Active);
    }

    // Test added by Owain
    [Test]
    public void NeutralPlayerTurn_EnsureNeutralPlayerMovesCorrectly()
    {
        game.Initialize(true);
        game.currentPlayer = Players[3];
        Assert.That(game.currentPlayer.Units.Count, Is.EqualTo(1));
        Sector[] adjacentSectors = game.currentPlayer.Units[0].Sector.AdjacentSectors;
        game.NeutralPlayerTurn();

        // Check that the neutral player is only moving to sectors that do not already contain units
        // Check that the neutral player is not moving to a sector containing the vice chancellor
        foreach (Sector sector in adjacentSectors)
            Assert.That(sector.Owner == null || sector.Owner.Neutral && !sector.HasPVC);
    }

    [Test]
    public void NextTurnState_TurnStateProgressesCorrectly()
    {
        // initialize turn state to Move1
        game.TurnState = TurnState.Move1;

        // ensure NextTurnState changes the turn state
        // from Move1 to Move2
        game.NextTurnState();
        Assert.That(game.TurnState, Is.EqualTo(TurnState.Move2));

        // ensure NextTurnState changes the turn state
        // from Move2 to EndOfTurn
        game.NextTurnState();
        Assert.That(game.TurnState, Is.EqualTo(TurnState.EndOfTurn));

        // ensure NextTurnState changes the turn state
        // from EndOfTurn to Move1
        game.NextTurnState();
        Assert.That(game.TurnState, Is.EqualTo(TurnState.Move1));

        // ensure NextTurnState does not change turn state
        // if the current turn state is NULL
        game.TurnState = TurnState.NULL;
        game.NextTurnState();
        Assert.That(game.TurnState, Is.EqualTo(TurnState.NULL));
    }

    [Test]
    public void GetWinner_OnePlayerWithLandmarksAndUnitsWins()
    {
        Sector landmark1 = map.Sectors[1];
        Player playerA = Players[0];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.That(landmark1.Landmark, Is.Not.Null);

        // ensure winner is found if only 1 player owns a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.OwnedSectors.Add(landmark1);
        playerA.Units.Add(InitUnit());
        Assert.That(game.GetWinner(), Is.Not.Null);
    }

    [Test]
    public void GetWinner_NoWinnerWhenMultiplePlayersOwningLandmarks()
    {
        Sector landmark1 = map.Sectors[1];
        Sector landmark2 = map.Sectors[7];
        Player playerA = Players[0];
        Player playerB = Players[1];

        // ensure'landmark1' and 'landmark2' are landmarks
        landmark1.Initialize();
        landmark2.Initialize();
        Assert.That(landmark1.Landmark, Is.Not.Null);
        Assert.That(landmark2.Landmark, Is.Not.Null);

        // ensure no winner is found if >1 players own a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.OwnedSectors.Add(landmark1);
        playerB.OwnedSectors.Add(landmark2);
        Assert.That(game.GetWinner(), Is.Null);
    }

    [Test]
    public void GetWinner_NoWinnerWhenMultiplePlayersWithUnits()
    {
        Player playerA = Players[0];
        Player playerB = Players[1];

        // ensure no winner is found if >1 players have a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.Units.Add(InitUnit(0));
        playerB.Units.Add(InitUnit(1));
        Assert.That(game.GetWinner(), Is.Null);
    }

    [Test]
    public void GetWinner_NoWinnerWhenAPlayerHasLandmarkAndAnotherHasUnits()
    {
        Sector landmark1 = map.Sectors[1];
        Player playerA = Players[0];
        Player playerB = Players[1];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.That(landmark1.Landmark, Is.Not.Null);

        // ensure no winner is found if 1 player has a landmark
        // and another player has a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.OwnedSectors.Add(landmark1);
        playerB.Units.Add(InitUnit(1));
        Assert.That(game.GetWinner(), Is.Null);
    }

    void ClearSectorsAndUnitsOfAllPlayers()
    {
        foreach (Player player in Players)
            ClearSectorsAndUnits(player);
    }

    void ClearSectorsAndUnits(Player player)
    {
        player.Units.Clear();
        player.OwnedSectors.Clear();
    }

    [Test]
    public void EndGame_GameEndsCorrectlyWithNoCurrentPlayerAndNoActivePlayersAndNoTurnState()
    {
        foreach (Player player in Players)
        {
            player.Units.Clear();
            player.OwnedSectors.Clear();
        }
        game.currentPlayer = Players[0];
        game.currentPlayer.Units.Add(null);
        game.EndGame();

        // ensure the game is marked as finished
        Assert.That(game.IsFinished);

        // ensure the current player is null
        Assert.That(game.currentPlayer, Is.Null);

        // ensure no players are active
        foreach (Player player in game.players)
            Assert.That(player.Active, Is.False);

        // ensure turn state is NULL
        Assert.That(game.TurnState, Is.EqualTo(TurnState.NULL));
    }
}
#endif
