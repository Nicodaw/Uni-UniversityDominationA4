using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PlayerTest 
{
    private Game game;
    private Map map;
	private Player[] players;
	private PlayerUI[] gui;

    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.Game;
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
    }

    [UnityTest]
    public IEnumerator CaptureSector_ChangesOwner() {
        
        Setup();

        Player previousOwner = map.sectors[0].Owner;
     //   bool run = false; // used to decide whether to check previous players sector list (if no previous owner, do not look in list)

       // if (map.sectors[0].Owner != null)
       // {            
       //     run = true;
       // }

        game.players[0].Capture(map.sectors[0]);
        Assert.AreSame(map.sectors[0].Owner, game.players[0]); // owner stored in sector
        Assert.IsTrue(game.players[0].ownedSectors.Contains(map.sectors[0])); // sector is stored as owned by the player

		if (/*run == true*/previousOwner != null) // if sector had previous owner
        {
            Assert.IsFalse(previousOwner.ownedSectors.Contains(map.sectors[0])); // sector has been removed from previous owner list
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersBeerAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.SetResourceType(Landmark.ResourceType.Attack);

        // get beer amounts for each player before capture
        int attackerBeerBeforeCapture = playerA.AttackBonus;
        int defenderBeerBeforeCapture = playerB.AttackBonus;
        Player previousOwner = landmarkedSector.Owner;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.IsTrue(attackerBeerBeforeCapture + landmark.GetAmount() == playerA.AttackBonus);
        Assert.IsTrue(defenderBeerBeforeCapture - landmark.GetAmount() == previousOwner.AttackBonus);

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_BothPlayersKnowledgeAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        playerB.Capture(landmarkedSector);

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.SetResourceType(Landmark.ResourceType.Defence);

        // get knowledge amounts for each player before capture
        int attackerKnowledgeBeforeCapture = playerA.DefenceBonus;
        int defenderKnowledgeBeforeCapture = playerB.DefenceBonus;
        Player previousOwner = landmarkedSector.Owner;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are transferred correctly
        Assert.IsTrue(attackerKnowledgeBeforeCapture + landmark.GetAmount() == playerA.DefenceBonus);
        Assert.IsTrue(defenderKnowledgeBeforeCapture - landmark.GetAmount() == previousOwner.DefenceBonus);

        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerBeerAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Beer
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.SetResourceType(Landmark.ResourceType.Attack);

        // get player beer amount before capture
        int oldBeer = playerA.AttackBonus;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are gained correctly
        Assert.IsTrue(playerA.AttackBonus - oldBeer == landmark.GetAmount());
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator CaptureLandmark_NeutralLandmarkPlayerKnowledgeAmountCorrect() {
        
        Setup();

        // capturing landmark
        Sector landmarkedSector = map.sectors[1]; 
        landmarkedSector.Initialize();
        Landmark landmark = landmarkedSector.Landmark;
        Player playerA = game.players[0];

        // ensure 'landmarkedSector' is a landmark of type Knowledge
        Assert.IsNotNull(landmarkedSector.Landmark);
        landmark.SetResourceType(Landmark.ResourceType.Defence);

        // get player knowledge amount before capture
        int oldKnowledge = playerA.DefenceBonus;

        playerA.Capture(landmarkedSector);

        // ensure sector is captured correctly
        Assert.AreSame(landmarkedSector.Owner, playerA);
        Assert.IsTrue(playerA.ownedSectors.Contains(landmarkedSector));

        // ensure resources are gained correctly
        Assert.IsTrue(playerA.DefenceBonus - oldKnowledge == landmark.GetAmount());

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_SpawnedWhenLandmarkOwnedAndUnoccupied() {
        
        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = null;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a unit has been spawned for playerA in landmarkedSector
        Assert.IsTrue(playerA.units.Contains(landmarkedSector.Unit));

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkOwnedAndOccupied() {

        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Player playerA = game.players[0];

        // ensure that 'landmarkedSector' is a landmark and contains a Level 5 unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = MonoBehaviour.Instantiate(playerA.UnitPrefab).GetComponent<Unit>();
        landmarkedSector.Unit.Level = 5;
        landmarkedSector.Unit.Owner = playerA;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerA.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure a Level 1 unit has not spawned over the Level 5 unit already in landmarkedSector
        Assert.IsTrue(landmarkedSector.Unit.Level == 5);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SpawnUnits_NotSpawnedWhenLandmarkNotOwned() {

        Setup();

        Sector landmarkedSector = map.sectors[1]; 
        Player playerA = game.players[0];
        Player playerB = game.players[1];
        landmarkedSector.Unit = null;

        // ensure that 'landmarkedSector' is a landmark and does not contain a unit
        landmarkedSector.Initialize();
        landmarkedSector.Unit = null;
        Assert.IsNotNull(landmarkedSector.Landmark);

        playerB.Capture(landmarkedSector);
        playerA.SpawnUnits();

        // ensure no unit is spawned at landmarkedSector
        Assert.IsNull(landmarkedSector.Unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator IsEliminated_PlayerWithNoUnitsAndNoLandmarksEliminated() {
        
        Setup();

        Player playerA = game.players[0];

        Assert.IsFalse(playerA.IsEliminated()); // not eliminated because they have units

        for (int i = 0; i < playerA.units.Count; i++)
        {
            playerA.units[i].DestroySelf(); // removes units
        }
        Assert.IsFalse(playerA.IsEliminated()); // not eliminated because they still have a landmark

        // player[0] needs to lose their landmark
        for (int i = 0; i < playerA.ownedSectors.Count; i++)
        {
            if (playerA.ownedSectors[i].Landmark != null)
            {
                playerA.ownedSectors[i].Landmark = null; // player[0] no longer has landmarks
            }
        }
        Assert.IsTrue(playerA.IsEliminated());

        yield return null;
    }
}