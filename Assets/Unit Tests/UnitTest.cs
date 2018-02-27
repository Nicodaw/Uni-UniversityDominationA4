using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class UnitTest 
{
    private Game game;
    private Map map;
	private Player[] players;
	private PlayerUI[] gui;
    private GameObject unitPrefab;

    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.GetGame();
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
        this.unitPrefab = t.GetUnitPrefab();
    }

    [UnityTest]
    public IEnumerator MoveToFriendlyFromNull_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving from null
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerA;

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.Sector == sectorA);
        Assert.IsTrue(sectorA.Unit == unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToNeutral_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Player playerA = players[0];

        // test moving from one sector to another
        unit.Sector = sectorA;
        unit.Owner = playerA;
        sectorA.Unit = unit;
        sectorB.Unit = null;
        sectorA.Owner = playerA;
        sectorB.Owner = playerA;

        unit.MoveTo(sectorB);
        Assert.IsTrue(unit.Sector == sectorB);
        Assert.IsTrue(sectorB.Unit == unit);
        Assert.IsNull(sectorA.Unit);

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveToFriendly_UnitInCorrectSector() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];

        // test moving into a friendly sector (no level up)
        unit.Level = 1;
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerA;

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.Level == 1);

        yield return null;
    }

    public IEnumerator MoveToHostile_UnitInCorrectSectorAndLevelUp() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sectorA = map.sectors[0];
        Player playerA = players[0];
        Player playerB = players[1];

        // test moving into a non-friendly sector (level up)
        unit.Level = 1;
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerB;

        unit.MoveTo(sectorA);
        Assert.IsTrue(unit.Level == 2);
        Assert.IsTrue(sectorA.Owner == unit.Owner);

        yield return null;
    }

    [UnityTest]
    public IEnumerator SwapPlaces_UnitsInCorrectNewSectors() {

        Setup();

        Unit unitA = map.sectors[0].Unit;
        Unit unitB = map.sectors[1].Unit;
        
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        yield return null;

        unitA.SwapPlacesWith(unitB);
        Assert.IsTrue(unitA.Sector == sectorB); // unitA in sectorB
        Assert.IsTrue(sectorB.Unit == unitA); // sectorB has unitA
        Assert.IsTrue(unitB.Sector == sectorA); // unitB in sectorA
        Assert.IsTrue(sectorA.Unit == unitB); // sectorA has unitB

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelIncreasesByOne() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp increments level as expected
        unit.Level = 1;
        unit.LevelUp();
        Assert.IsTrue(unit.Level == 2);

        yield return null;
    }

    [UnityTest]
    public IEnumerator LevelUp_UnitLevelDoesNotPastFive() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();

        // ensure LevelUp does not increment past 5
        unit.Level = 5;
        unit.LevelUp();
        Assert.IsTrue(unit.Level == 5);

        yield return null;
    }
        
    [UnityTest]
    public IEnumerator SelectAndDeselect_SelectedTrueWhenSelectedFalseWhenDeselected() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];

        unit.Sector = sector;
        unit.IsSelected = false;

        unit.Select();
        Assert.IsTrue(unit.IsSelected);

        unit.Deselect();
        Assert.IsFalse(unit.IsSelected);

        yield return null;
    }


    [UnityTest]
    public IEnumerator DestroySelf_UnitNotInSectorAndNotInPlayersUnitsList() {
        
        Setup();

        Unit unit = MonoBehaviour.Instantiate(unitPrefab).GetComponent<Unit>();
        Sector sector = map.sectors[0];
        Player player = players[0];

        unit.Sector = sector;
        sector.Unit = unit;

        unit.Owner = player;
        player.units.Add(unit);

        unit.DestroySelf();

        Assert.IsNull(sector.Unit); // unit not on sector 
        Assert.IsFalse(player.units.Contains(unit)); // unit not in list of players units

        yield return null;
    }
}