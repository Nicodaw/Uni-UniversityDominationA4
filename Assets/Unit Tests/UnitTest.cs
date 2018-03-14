#if UNITY_EDITOR
using NUnit.Framework;

public class UnitTest : BaseGameTest
{
    [Test]
    public void MoveToFriendlyFromNull_UnitInCorrectSector()
    {
        Unit unit = InitUnit();
        Sector sectorA = map.Sectors[0];
        Player playerA = Players[0];

        // test moving from null
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerA;

        unit.MoveTo(sectorA);
        Assert.That(unit.Sector, Is.EqualTo(sectorA));
        Assert.That(sectorA.Unit, Is.EqualTo(unit));
    }

    [Test]
    public void MoveToNeutral_UnitInCorrectSector()
    {
        Unit unit = InitUnit();
        Sector sectorA = map.Sectors[0];
        Sector sectorB = map.Sectors[1];
        Player playerA = Players[0];

        // test moving from one sector to another
        unit.Sector = sectorA;
        unit.Owner = playerA;
        sectorA.Unit = unit;
        sectorB.Unit = null;
        sectorA.Owner = playerA;
        sectorB.Owner = playerA;

        unit.MoveTo(sectorB);
        Assert.That(unit.Sector, Is.EqualTo(sectorB));
        Assert.That(sectorB.Unit, Is.EqualTo(unit));
        Assert.That(sectorA.Unit, Is.Null);
    }

    [Test]
    public void MoveToFriendly_UnitInCorrectSector()
    {
        Unit unit = InitUnit();
        Sector sectorA = map.Sectors[0];
        Player playerA = Players[0];

        // test moving into a friendly sector (no level up)
        unit.Level = 1;
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerA;

        unit.MoveTo(sectorA);
        Assert.That(unit.Level, Is.EqualTo(1));
    }

    public void MoveToHostile_UnitInCorrectSectorAndLevelUp()
    {
        Unit unit = InitUnit();
        Sector sectorA = map.Sectors[0];
        Player playerA = Players[0];
        Player playerB = Players[1];

        // test moving into a non-friendly sector (level up)
        unit.Level = 1;
        unit.Sector = null;
        sectorA.Unit = null;
        unit.Owner = playerA;
        sectorA.Owner = playerB;

        unit.MoveTo(sectorA);
        Assert.That(unit.Level, Is.EqualTo(2));
        Assert.That(sectorA.Owner, Is.EqualTo(unit.Owner));
    }

    [Test]
    public void SwapPlaces_UnitsInCorrectNewSectors()
    {
        Unit[] units = InitUnits(2);

        Sector sectorA = map.Sectors[0];
        Sector sectorB = map.Sectors[1];
        Player player = Players[0];

        // places players unitA in sectorA
        units[0].Owner = player;
        units[0].Sector = sectorA;
        sectorA.Unit = units[0];

        // places players unitB in sectorB
        units[1].Owner = player;
        units[1].Sector = sectorB;
        sectorB.Unit = units[1];

        units[0].SwapPlacesWith(units[1]);
        Assert.That(units[0].Sector, Is.EqualTo(sectorB)); // unitA in sectorB
        Assert.That(sectorB.Unit, Is.EqualTo(units[0])); // sectorB has unitA
        Assert.That(units[1].Sector, Is.EqualTo(sectorA)); // unitB in sectorA
        Assert.That(sectorA.Unit, Is.EqualTo(units[1])); // sectorA has unitB
    }

    [Test]
    public void LevelUp_UnitLevelIncreasesByOne()
    {
        Unit unit = InitUnit();

        // ensure LevelUp increments level as expected
        unit.Level = 1;
        unit.LevelUp();
        Assert.That(unit.Level, Is.EqualTo(2));
    }

    [Test]
    public void LevelUp_UnitLevelDoesNotPastFive()
    {
        Unit unit = InitUnit();

        // ensure LevelUp does not increment past 5
        unit.Level = 5;
        unit.LevelUp();
        Assert.That(unit.Level, Is.EqualTo(5));
    }

    [Test]
    public void SelectAndDeselect_SelectedTrueWhenSelectedFalseWhenDeselected()
    {
        Unit unit = InitUnit();
        Sector sector = map.Sectors[0];

        unit.Sector = sector;
        unit.IsSelected = false;

        unit.Select();
        Assert.That(unit.IsSelected);

        unit.Deselect();
        Assert.That(unit.IsSelected, Is.False);
    }

    [Test]
    public void DestroySelf_UnitNotInSectorAndNotInPlayersUnitsList()
    {
        Unit unit = InitUnit();
        Sector sector = map.Sectors[0];
        Player player = Players[0];

        unit.Sector = sector;
        sector.Unit = unit;

        unit.Owner = player;
        player.Units.Add(unit);

        unit.DestroySelf();

        Assert.That(sector.Unit, Is.Null); // unit not on sector 
        Assert.That(player.Units, Does.Not.Contains(unit)); // unit not in list of players units
    }
}
#endif
