using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SectorTest 
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
    public IEnumerator SetOwner_SectorOwnerAndColorCorrect() {
        
        Setup();

        Sector sector = map.sectors[0];
        sector.Owner = null;
        Player player = players[0];

        sector.Owner = player;
        Assert.AreSame(sector.Owner, player);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(player.Color));

        sector.Owner = null;
        Assert.IsNull(sector.Owner);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(Color.gray));

        yield return null;
    }

    [UnityTest]
    public IEnumerator Initialize_OwnedAndNotOwnedSectorsOwnerAndColor() {
        
        Setup();

        Sector sectorWithoutLandmark = map.sectors[0];
        Sector sectorWithLandmark = map.sectors[1];

        sectorWithoutLandmark.Initialize();
        Assert.IsNull(sectorWithoutLandmark.Owner);
        Assert.IsTrue(sectorWithoutLandmark.gameObject.GetComponent<Renderer>().material.color.Equals(Color.gray));
        Assert.IsNull(sectorWithoutLandmark.Unit);
        Assert.IsNull(sectorWithoutLandmark.Landmark);

        sectorWithLandmark.Initialize();
        Assert.IsNull(sectorWithLandmark.Owner);
        Assert.IsTrue(sectorWithLandmark.gameObject.GetComponent<Renderer>().material.color.Equals(Color.gray));
        Assert.IsNull(sectorWithLandmark.Unit);
        Assert.IsNotNull(sectorWithLandmark.Landmark);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Highlight_SectorColourCorrect() {
        
        Setup();

        Sector sector = map.sectors[0];
        sector.gameObject.GetComponent<Renderer>().material.color = Color.gray;
        float amount = 0.2f;
        Color highlightedGray = Color.gray + (Color) (new Vector4(amount, amount, amount, 1));

        sector.ApplyHighlight(amount);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(highlightedGray));

        sector.RevertHighlight(amount);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(Color.gray));

        yield return null;
    }


    [UnityTest]
    public IEnumerator ClearUnit_UnitRemovedFromSector() {
        
        Setup();

        Sector sector = map.sectors[0]; // sector 0 contains a unit by default
        Assert.NotNull(sector.Unit);

        sector.ClearUnit();
        Assert.IsNull(sector.Unit);
        
        yield return null;
    }

    [UnityTest]
    public IEnumerator OnMouseAsButton_CorrectUnitIsSelected() {
        
        Setup();

        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Sector sectorC = map.sectors[2];
        Player playerA = players[0];
        Player playerB = players[1];
        Unit unitA = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        Unit unitB = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>(); // *should this be players[1]?* ###########################################################################################

        // ensure sectors A & B are adjacent to each other
        Assert.Contains(sectorA, sectorB.AdjacentSectors);
        Assert.Contains(sectorB, sectorA.AdjacentSectors);

        // ensure sectors A & C are not adjacent to each other
        foreach (Sector sector in sectorA.AdjacentSectors)
        {
            Assert.IsFalse(sector == sectorC);
        }
        foreach (Sector sector in sectorC.AdjacentSectors)
        {
            Assert.IsFalse(sector == sectorA);
        }

        sectorA.Unit = unitA;
        unitA.Sector = sectorA;

        sectorA.Owner = playerA;
        unitA.Owner = playerA;
        unitB.Owner = playerB;

        playerA.units.Add(unitA);
        playerB.units.Add(unitB);


        // test clicking a sector with a unit while the unit's owner
        // is active AND there are no units selected
        playerA.Active = true;
        unitA.IsSelected = false;
        unitB.IsSelected =false;

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsTrue(unitA.IsSelected);

        // test clicking on the sector containing the selected unit
        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected);


        // test clicking a sector with a unit while there are no
        // units selected, but the unit's owner is NOT active
        playerA.Active = false;
        unitA.IsSelected = false;
        unitB.IsSelected =false;

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected);


        // test clicking a sector with a unit while the unit's owner
        // is active, but there IS another unit selected
        playerA.Active = true;
        unitA.IsSelected = false;
        unitB.IsSelected = true;

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected);


        // test clicking on a sector adjacent to a selected unit
        unitA.IsSelected = true;
        unitB.IsSelected = false;

        sectorB.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected);

        // only need to test deselection;
        // other interactions covered in smaller tests below

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoUnoccupiedSector_NewSectorHasUnitAndOldDoesNotAndTurnStateProgressed() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move1);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        sectorA.Unit = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        sectorA.Unit.Sector = sectorA;
        sectorB.Unit = null;

        sectorB.MoveIntoUnoccupiedSector(sectorA.Unit);
        Assert.IsNotNull(sectorB.Unit);
        Assert.IsNull(sectorA.Unit);
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoFriendlyUnit_UnitsSwapSectorsAndTurnStateProgressed() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move1);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        sectorA.Unit = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        sectorA.Unit.Sector = sectorA;
        sectorA.Unit.Level = 5;
        sectorA.Unit.Owner = players[0];
        sectorA.Owner = players[0];

        sectorB.Unit = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        sectorB.Unit.Sector = sectorB;
        sectorB.Unit.Level = 1;
        sectorB.Unit.Owner = players[0];
        sectorB.Owner = players[0];

        sectorB.MoveIntoFriendlyUnit(sectorA.Unit);
        Assert.IsTrue(sectorA.Unit.Level == 1); // level 1 unit now in sectorA
        Assert.IsTrue(sectorB.Unit.Level == 5); // level 2 unit now in sectorB => units have swapped locations
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoHostileUnit_AttackingUnitTakesSectorAndLevelUpAndTurnEnd() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move1);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that the attacking unit wins
        ResetSectors(sectorA, sectorB);
        sectorA.Owner.AttackBonus = 99; // to ensure the sectorA unit will win any conflict (attacking)
        sectorB.Owner.DefenceBonus = 0;

        sectorB.MoveIntoHostileUnit(sectorA.Unit, sectorB.Unit);
        Assert.IsNull(sectorA.Unit); // attackingg unit moved out of sectorA
        Assert.IsTrue(sectorB.GetLevel() == 2); // attacking unit that moved to sectorB gained a level (the unit won the conflict)
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoHostileUnit_DefendingUnitDefendsSectorAndTurnEnd() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move1);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that the defending unit wins
        game.SetTurnState(Game.TurnState.Move1);
        ResetSectors(sectorA, sectorB);
        sectorA.Owner.AttackBonus = 0;
        sectorB.Owner.DefenceBonus = 99; //to ensure the sectorB unit will win any conflict (defending)

        sectorB.MoveIntoHostileUnit(sectorA.Unit, sectorB.Unit);
        Assert.IsNull(sectorA.Unit); // attacking unit destroyed
        Assert.IsTrue(sectorB.Unit.Level == 1); // defending unit did not gain a level following defence
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoHostileUnit_TieConflict_DefendingUnitDefendsSectorAndTurnEnd() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move1);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that there is a tie (defending unit wins)

        // *** UNITCONTROLLER DESTROYSELF METHOD NEEDS TO CLEAR UNIT ***

        game.SetTurnState(Game.TurnState.Move1);
        ResetSectors(sectorA, sectorB);
        sectorA.Unit.Level = -4;
        sectorA.Owner.AttackBonus = 0;
        sectorB.Unit.Level = -4;
        sectorB.Owner.DefenceBonus = 0; // making both units equal

        sectorB.MoveIntoHostileUnit(sectorA.Unit, sectorB.Unit);
        Assert.IsNull(sectorA.Unit); // attacking unit destroyed
        Assert.IsTrue(sectorB.Unit.Level == -4); // defending unit did not gain a level following defence
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        yield return null;
    }

    [UnityTest]
    public IEnumerator AdjacentSelectedUnit_SectorsAreAdjacent() {
        
        Setup();

        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // ensure sectors A and B are adjacent to each other
        Assert.Contains(sectorA, sectorB.AdjacentSectors);
        Assert.Contains(sectorB, sectorA.AdjacentSectors);

        // test with no unit in adjacent sector
        Assert.IsNull(sectorB.AdjacentSelectedUnit());

        // test with unselected unit in adjacent sector
        sectorA.Unit = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        sectorA.Unit.IsSelected = false;
        Assert.IsNull(sectorB.AdjacentSelectedUnit());

        // test with selected unit in adjacent sectors
        sectorA.Unit.IsSelected = true;
        Assert.IsNotNull(sectorB.AdjacentSelectedUnit());

        yield return null;
    }
    
    private void ResetSectors(Sector sectorA, Sector sectorB) {
        
        // re-initialize sectors for in between test cases in MoveIntoHostileUnitTest

        sectorA.Unit = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        sectorA.Unit.Sector = sectorA;
        sectorA.Unit.Owner = players[0];
        sectorA.Owner = players[0];
        sectorA.Unit.Level = 1;

        sectorB.Unit = MonoBehaviour.Instantiate(players[0].UnitPrefab).GetComponent<Unit>();
        sectorB.Unit.Sector = sectorB;
        sectorB.Unit.Owner = players[1];
        sectorB.Owner = players[1];
        sectorB.Unit.Level = 1;
    }
}