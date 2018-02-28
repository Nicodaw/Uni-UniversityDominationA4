#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

public class SectorTest : BaseGameTest
{
    [Test]
    public void SetOwner_SectorOwnerAndColorCorrect()
    {
        Sector sector = map.sectors[0];
        sector.Owner = null;
        Player player = players[0];

        sector.Owner = player;
        Assert.That(sector.Owner, Is.EqualTo(player));
        Assert.That(sector.gameObject.GetComponent<Renderer>().material.color, Is.EqualTo(player.Color));

        sector.Owner = null;
        Assert.That(sector.Owner, Is.Null);
        Assert.That(sector.gameObject.GetComponent<Renderer>().material.color, Is.EqualTo(Color.gray));
    }

    [Test]
    public void Initialize_OwnedAndNotOwnedSectorsOwnerAndColor()
    {
        Sector sectorWithoutLandmark = map.sectors[0];
        Sector sectorWithLandmark = map.sectors[1];

        sectorWithoutLandmark.Initialize();
        Assert.That(sectorWithoutLandmark.Owner, Is.Null);
        Assert.That(sectorWithoutLandmark.gameObject.GetComponent<Renderer>().material.color, Is.EqualTo(Color.gray));
        Assert.That(sectorWithoutLandmark.Unit, Is.Null);
        Assert.That(sectorWithoutLandmark.Landmark, Is.Null);

        sectorWithLandmark.Initialize();
        Assert.That(sectorWithLandmark.Owner, Is.Null);
        Assert.That(sectorWithLandmark.gameObject.GetComponent<Renderer>().material.color, Is.EqualTo(Color.gray));
        Assert.That(sectorWithLandmark.Unit, Is.Null);
        Assert.That(sectorWithLandmark.Landmark, Is.Not.Null);
    }

    [Test]
    public void Highlight_SectorColourCorrect()
    {
        Sector sector = map.sectors[0];
        sector.gameObject.GetComponent<Renderer>().material.color = Color.gray;
        float amount = 0.2f;
        Color highlightedGray = Color.gray + (Color)(new Vector4(amount, amount, amount, 1));

        sector.ApplyHighlight(amount);
        Assert.That(sector.gameObject.GetComponent<Renderer>().material.color, Is.EqualTo(highlightedGray));

        sector.RevertHighlight(amount);
        Assert.That(sector.gameObject.GetComponent<Renderer>().material.color, Is.EqualTo(Color.gray));
    }

    [Test]
    public void ClearUnit_UnitRemovedFromSector()
    {
        Sector sector = map.sectors[0];
        sector.Unit = InitUnit();

        sector.ClearUnit();
        Assert.That(sector.Unit, Is.Null);
    }

    [Test]
    public void OnMouseAsButton_CorrectUnitIsSelected()
    {
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Sector sectorC = map.sectors[2];
        Player playerA = players[0];
        Player playerB = players[1];
        Unit unitA = InitUnit(0);
        Unit unitB = InitUnit(1);

        // ensure sectors A & B are adjacent to each other
        Assert.That(sectorB.AdjacentSectors, Contains.Item(sectorA));
        Assert.That(sectorA.AdjacentSectors, Contains.Item(sectorB));

        // ensure sectors A & C are not adjacent to each other
        foreach (Sector sector in sectorA.AdjacentSectors)
            Assert.That(sector, Is.Not.EqualTo(sectorC));
        foreach (Sector sector in sectorC.AdjacentSectors)
            Assert.That(sector, Is.Not.EqualTo(sectorA));

        sectorA.Unit = unitA;
        unitA.Sector = sectorA;

        sectorA.Owner = playerA;
        unitA.Owner = playerA;
        unitB.Owner = playerB;

        playerA.Units.Add(unitA);
        playerB.Units.Add(unitB);

        // test clicking a sector with a unit while the unit's owner
        // is active AND there are no units selected
        playerA.Active = true;
        unitA.IsSelected = false;
        unitB.IsSelected = false;

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.That(unitA.IsSelected);

        // test clicking on the sector containing the selected unit
        sectorA.OnMouseUpAsButtonAccessible();
        Assert.That(unitA.IsSelected, Is.False);

        // test clicking a sector with a unit while there are no
        // units selected, but the unit's owner is NOT active
        playerA.Active = false;
        unitA.IsSelected = false;
        unitB.IsSelected = false;

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.That(unitA.IsSelected, Is.False);

        // test clicking a sector with a unit while the unit's owner
        // is active, but there IS another unit selected
        playerA.Active = true;
        unitA.IsSelected = false;
        unitB.IsSelected = true;

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.That(unitA.IsSelected, Is.False);

        // test clicking on a sector adjacent to a selected unit
        unitA.IsSelected = true;
        unitB.IsSelected = false;

        sectorB.OnMouseUpAsButtonAccessible();
        Assert.That(unitA.IsSelected, Is.False);
    }

    [Test]
    public void MoveIntoUnoccupiedSector_NewSectorHasUnitAndOldDoesNotAndTurnStateProgressed()
    {
        game.TurnState = TurnState.Move1;
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        sectorA.Unit = InitUnit(0);
        sectorA.Unit.Sector = sectorA;
        sectorB.Unit = null;

        sectorB.MoveIntoUnoccupiedSector(sectorA.Unit);
        Assert.That(sectorB.Unit, Is.Not.Null);
        Assert.That(sectorA.Unit, Is.Not.Null);
        Assert.That(game.TurnState, Is.EqualTo(TurnState.Move2));
    }

    [Test]
    public void MoveIntoFriendlyUnit_UnitsSwapSectorsAndTurnStateProgressed()
    {
        game.TurnState = TurnState.Move1;
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        sectorA.Unit = InitUnit(0);
        sectorA.Unit.Sector = sectorA;
        sectorA.Unit.Level = 5;
        sectorA.Unit.Owner = players[0];
        sectorA.Owner = players[0];

        sectorB.Unit = InitUnit(0);
        sectorB.Unit.Sector = sectorB;
        sectorB.Unit.Level = 1;
        sectorB.Unit.Owner = players[0];
        sectorB.Owner = players[0];

        sectorB.MoveIntoFriendlyUnit(sectorA.Unit);
        Assert.That(sectorA.Unit.Level, Is.EqualTo(1)); // level 1 unit now in sectorA
        Assert.That(sectorB.Unit.Level, Is.EqualTo(5)); // level 2 unit now in sectorB => units have swapped locations
        Assert.That(game.TurnState, Is.EqualTo(TurnState.Move2));
    }

    [Test]
    public void MoveIntoHostileUnit_AttackingUnitTakesSectorAndLevelUpAndTurnEnd()
    {
        game.TurnState = TurnState.Move1;
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that the attacking unit wins
        ResetSectors(sectorA, sectorB);
        sectorA.Owner.AttackBonus = 99; // to ensure the sectorA unit will win any conflict (attacking)
        sectorB.Owner.DefenceBonus = 0;

        sectorB.MoveIntoHostileUnit(sectorA.Unit, sectorB.Unit);
        Assert.That(sectorA.Unit, Is.Null); // attackingg unit moved out of sectorA
        Assert.That(sectorB.GetLevel(), Is.EqualTo(2)); // attacking unit that moved to sectorB gained a level (the unit won the conflict)
        Assert.That(game.TurnState, Is.EqualTo(TurnState.EndOfTurn));
    }

    [Test]
    public void MoveIntoHostileUnit_DefendingUnitDefendsSectorAndTurnEnd()
    {
        game.TurnState = TurnState.Move1;
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that the defending unit wins
        game.TurnState = TurnState.Move1;
        ResetSectors(sectorA, sectorB);
        sectorA.Owner.AttackBonus = 0;
        sectorB.Owner.DefenceBonus = 99; //to ensure the sectorB unit will win any conflict (defending)

        sectorB.MoveIntoHostileUnit(sectorA.Unit, sectorB.Unit);
        Assert.That(sectorA.Unit, Is.Null); // attacking unit destroyed
        Assert.That(sectorB.Unit.Level, Is.EqualTo(1)); // defending unit did not gain a level following defence
        Assert.That(game.TurnState, Is.EqualTo(TurnState.EndOfTurn));
    }

    [Test]
    public void MoveIntoHostileUnit_TieConflict_DefendingUnitDefendsSectorAndTurnEnd()
    {
        game.TurnState = TurnState.Move1;
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that there is a tie (defending unit wins)

        // *** UNITCONTROLLER DESTROYSELF METHOD NEEDS TO CLEAR UNIT ***

        game.TurnState = TurnState.Move1;
        ResetSectors(sectorA, sectorB);
        sectorA.Unit.Level = -4;
        sectorA.Owner.AttackBonus = 0;
        sectorB.Unit.Level = -4;
        sectorB.Owner.DefenceBonus = 0; // making both units equal

        sectorB.MoveIntoHostileUnit(sectorA.Unit, sectorB.Unit);
        Assert.That(sectorA.Unit, Is.Null); // attacking unit destroyed
        Assert.That(sectorB.Unit.Level, Is.EqualTo(-4)); // defending unit did not gain a level following defence
        Assert.That(game.TurnState, Is.EqualTo(TurnState.EndOfTurn));
    }

    void ResetSectors(Sector sectorA, Sector sectorB)
    {
        // re-initialize sectors for in between test cases in MoveIntoHostileUnitTest

        sectorA.Unit = InitUnit(0);
        sectorA.Unit.Sector = sectorA;
        sectorA.Unit.Owner = players[0];
        sectorA.Owner = players[0];
        sectorA.Unit.Level = 1;

        sectorB.Unit = InitUnit(1);
        sectorB.Unit.Sector = sectorB;
        sectorB.Unit.Owner = players[1];
        sectorB.Owner = players[1];
        sectorB.Unit.Level = 1;
    }

    [Test]
    public void AdjacentSelectedUnit_SectorsAreAdjacent()
    {
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // ensure sectors A and B are adjacent to each other
        Assert.That(sectorB.AdjacentSectors, Contains.Item(sectorA));
        Assert.That(sectorA.AdjacentSectors, Contains.Item(sectorB));

        // test with no unit in adjacent sector
        Assert.That(sectorB.AdjacentSelectedUnit(), Is.Null);

        // test with unselected unit in adjacent sector
        sectorA.Unit = InitUnit();
        sectorA.Unit.IsSelected = false;
        Assert.That(sectorB.AdjacentSelectedUnit(), Is.Null);

        // test with selected unit in adjacent sectors
        sectorA.Unit.IsSelected = true;
        Assert.That(sectorB.AdjacentSelectedUnit(), Is.Not.Null);
    }
}
#endif
