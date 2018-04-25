#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;

public class PlayerTest : BaseGameTest
{
    [SetUp]
    public void PlayerTest_SetUp()
    {
        DefMapInit();
        SpawnAllPlayerUnits();
    }

    [Test]
    public void Units_Correct()
    {
        Sector target = map.Sectors.First(s => s.Unit == null);
        InitUnit(target, 0);
        Assume.That(target.Unit, Is.Not.Null);
        Assume.That(target.Unit.Owner, Is.EqualTo(Players[0]));
        target = map.Sectors.First(s => s.Unit == null);
        InitUnit(target, 0);
        Assert.That(Players[0].Units, Is.EquivalentTo(map.Sectors.Select(s => s.Unit).Where(u => u?.Owner == Players[0])));
        Assert.That(Players[0].Units.Count(), Is.EqualTo(3));
        Assert.That(Players[1].Units, Is.EquivalentTo(map.Sectors.Select(s => s.Unit).Where(u => u?.Owner == Players[1])));
        Assert.That(Players[1].Units.Count(), Is.EqualTo(1));
    }

    [Test]
    public void HasUnits_Correct()
    {
        Assert.That(Players[0].HasUnits, Is.True);
        foreach (Unit unit in Players[0].Units)
            unit.Kill(Players[1]);
        Assert.That(Players[0].HasUnits, Is.False);
        Assert.That(Players[1].HasUnits, Is.True);
    }

    [Test]
    public void OwnedSectors_Correct()
    {
        Sector target = map.Sectors.First(s => s.Owner == null);
        target.Owner = Players[0];
        target = map.Sectors.First(s => s.Owner == null);
        target.Owner = Players[0];
        Assert.That(Players[0].OwnedSectors, Is.EquivalentTo(map.Sectors.Where(s => s.Owner == Players[0])));
        Assert.That(Players[0].OwnedSectors.Count(), Is.EqualTo(3));
        Assert.That(Players[1].OwnedSectors, Is.EquivalentTo(map.Sectors.Where(s => s.Owner == Players[1])));
        Assert.That(Players[1].OwnedSectors.Count(), Is.EqualTo(1));
    }

    [Test]
    public void OwnedLandmarkedSectors_Correct()
    {
        Sector target = map.Sectors.First(s => s.Landmark != null && s.Owner != Players[0] && s.Owner != Players[1]);
        target.Owner = Players[0];
        Assume.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(Players[0].OwnedLandmarkSectors, Is.EquivalentTo(map.LandmarkedSectors.Where(s => s.Owner == Players[0])));
        Assert.That(Players[0].OwnedLandmarkSectors.Count(), Is.EqualTo(2));
        Assert.That(Players[1].OwnedLandmarkSectors, Is.EquivalentTo(map.LandmarkedSectors.Where(s => s.Owner == Players[1])));
        Assert.That(Players[1].OwnedLandmarkSectors.Count(), Is.EqualTo(1));
    }

    [Test]
    public void OwnsLandmark_Correct()
    {
        Assume.That(map.LandmarkedSectors.Where(s => s.Owner == Players[0]).Count(), Is.EqualTo(1));
        Assert.That(Players[0].OwnsLandmark, Is.True);
        foreach (Sector sector in map.LandmarkedSectors.Where(s => s.Owner == Players[0]))
            sector.Owner = null;
        Assume.That(map.LandmarkedSectors.Where(s => s.Owner == Players[0]).Any(), Is.False);
        Assert.That(Players[0].OwnsLandmark, Is.False);
        Assert.That(Players[1].OwnsLandmark, Is.True);
    }

    [Test]
    public void IsEliminated_Correct()
    {
        Players[0].SpawnUnits();
        Assert.That(Players[0].IsEliminated, Is.False);
        foreach (Sector sector in map.Sectors.Where(s => s.Owner == Players[0]))
        {
            sector.Unit?.Kill(Players[1]);
            sector.Owner = null;
        }
        Assume.That(map.Sectors.Where(s => s.Owner == Players[0]).Any(), Is.False);
        Assume.That(map.Sectors.Where(s => s.Unit?.Owner == Players[0]).Any(), Is.False);
        Assert.That(Players[0].IsEliminated, Is.True);
        Assert.That(Players[1].IsEliminated, Is.False);
    }

    [Test]
    public void AttemptMove_UnownedSector()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit unit = origin.Unit;
        int startActions = Players[0].ActionsRemaining;

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(unit));
        Assert.That(target.Owner, Is.Null);
        Assert.That(target.Unit, Is.Null);

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.Null);
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(unit));
        Assert.That(Players[0].ActionsRemaining, Is.EqualTo(startActions - 1));
    }

    [Test]
    public void AttemptMove_SelfOwnedSector()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit unit = origin.Unit;
        int startActions = Players[0].ActionsRemaining;

        // set owner to test player
        target.Owner = Players[0];

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(unit));
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.Null);

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.Null);
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(unit));
        Assert.That(Players[0].ActionsRemaining, Is.EqualTo(startActions - 1));
    }

    [Test]
    public void AttemptMove_OtherOwnedSector()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit unit = origin.Unit;
        int startActions = Players[0].ActionsRemaining;

        // set owner to secondary test player
        target.Owner = Players[1];

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(unit));
        Assert.That(target.Owner, Is.EqualTo(Players[1]));
        Assert.That(target.Unit, Is.Null);

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.Null);
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(unit));
        Assert.That(Players[0].ActionsRemaining, Is.EqualTo(startActions - 1));
    }

    [Test]
    public void AttemptMove_SelfOwnedSectorUnit()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit firstUnit = origin.Unit;
        Unit secondUnit = InitUnit(target, 0);
        int startActions = Players[0].ActionsRemaining;

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(firstUnit));
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(secondUnit));

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(secondUnit));
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(firstUnit));
        Assert.That(Players[0].ActionsRemaining, Is.EqualTo(startActions - 1));
    }

    [Test]
    public void AttemptMove_OtherOwnedSectorUnitWin()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit firstUnit = origin.Unit;
        Unit secondUnit = InitUnit(target, 1);
        int startActions = Players[0].ActionsRemaining;

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(firstUnit));
        Assert.That(target.Owner, Is.EqualTo(Players[1]));
        Assert.That(target.Unit, Is.EqualTo(secondUnit));

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.Null);
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(firstUnit));
        Assert.That(Players[0].ActionsRemaining, Is.EqualTo(startActions - 1));
        Assert.That(secondUnit == null); // force usage on overridden operator
    }

    [Test]
    public void AttemptMove_OtherOwnedSectorUnitLose()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit firstUnit = origin.Unit;
        Unit secondUnit = InitUnit(target, 1);
        int startActions = Players[0].ActionsRemaining;

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(firstUnit));
        Assert.That(target.Owner, Is.EqualTo(Players[1]));
        Assert.That(target.Unit, Is.EqualTo(secondUnit));

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.Null);
        Assert.That(target.Owner, Is.EqualTo(Players[1]));
        Assert.That(target.Unit, Is.EqualTo(secondUnit));
        Assert.That(Players[0].ActionsRemaining, Is.EqualTo(startActions - 1));
        Assert.That(firstUnit == null); // force usage on overridden operator
    }

    [Test]
    public void AttemptMove_ActionPerformedFired()
    {
        // set up move
        Sector origin = Players[0].Units.First().Sector;
        Sector target = origin.AdjacentSectors.First(s => s.Owner == null);
        Unit unit = origin.Unit;
        int startActions = Players[0].ActionsRemaining;
        bool onActionPerformedFired = false;

        Players[0].OnActionPerformed += (sender, e) => onActionPerformedFired = true;

        // do move
        Players[0].AttemptMove(origin, target);

        Assert.That(onActionPerformedFired, Is.True);
    }

    [Test]
    public void SpawnUnits_AreSpawned()
    {
        foreach (Unit unit in Players[0].Units)
            unit.Kill(Players[1]); // remove all units
        Assume.That(map.Sectors.Where(s => s.Unit?.Owner == Players[0]).Any(), Is.False);

        var availableSectors = Players[0].OwnedLandmarkSectors.Where(s => s.Unit == null).ToArray();
        Players[0].SpawnUnits();
        Assert.That(Players[0].Units.Select(u => u.Sector), Is.EquivalentTo(availableSectors));
    }

    [Test]
    public void SpawnUnits_NoRespawns()
    {
        Unit original = Players[0].Units.First();
        Players[0].SpawnUnits();
        Assert.That(Players[0].Units.Count(), Is.EqualTo(1));
        Assert.That(Players[0].Units.First(), Is.EqualTo(original));
    }

    [Test]
    public void SpawnUnitAt_Spawns()
    {
        // get first unowned sector
        Sector target = map.Sectors.First(s => s.Owner == null);
        int unitCount = Players[0].Units.Count();
        // spawn unit
        Players[0].SpawnUnitAt(target);

        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.Not.Null);
        Assert.That(target.Unit.Owner, Is.EqualTo(Players[0]));
        Assert.That(Players[0].Units.Count(), Is.EqualTo(unitCount + 1));
    }
}
#endif
