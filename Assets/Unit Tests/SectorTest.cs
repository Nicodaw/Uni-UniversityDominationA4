#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class SectorTest : BaseGameTest
{
    Color SectorColor(Sector sector) => sector.gameObject.GetComponent<Renderer>().material.color;

    [SetUp]
    public void SectorTest_SetUp() => DefMapInit();

    [Test]
    public void HasPVC_Correct()
    {
        foreach (Sector sector in map.Sectors)
            Assert.That(sector.HasPVC, Is.EqualTo(sector.Stats.HasEffect<EffectImpl.PVCEffect>()));
    }

    [Test]
    public void AllowPVC_Correct()
    {
        foreach (Sector sector in map.Sectors)
            Assert.That(sector.AllowPVC, Is.EqualTo(sector.Landmark == null && sector.Unit == null));
    }

    [Test]
    public void Unit_SetNew()
    {
        Sector original = map.Sectors.First(s => s.Owner == null && s.Unit == null);
        Unit unit = InitUnit(original, 0);
        Sector target = map.Sectors.First(s => s.Owner == null && s.Unit == null);
        Assume.That(target.Unit, Is.Null);
        Assume.That(target.Owner, Is.Null);

        // explicitly use the property
        target.Unit = unit;
        Assert.That(target.Unit, Is.EqualTo(unit));
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
    }

    [Test]
    public void Unit_OnDeathRaised()
    {
        Sector target = map.Sectors.First(s => s.Unit == null);
        InitUnit(target, 0);
        Assume.That(target.Unit, Is.Not.Null);
        bool onDeathRaised = false;

        target.OnUnitDeath += (sender, e) => onDeathRaised = true;
        target.Unit.Kill(Players[1]);
        Assert.That(onDeathRaised, Is.True);
    }

    [Test]
    public void Unit_SetNullClears()
    {
        Sector target = map.Sectors.First(s => s.Unit == null);
        Unit unit = InitUnit(target, 0);
        Assume.That(target.Unit, Is.Not.Null);
        bool onDeathRaised = false;

        target.OnUnitDeath += (sender, e) => onDeathRaised = true;
        target.Unit = null;
        Assert.That(target.Unit, Is.Null);
        unit.Kill(Players[1]);
        Assert.That(onDeathRaised, Is.False);
    }

    [Test]
    public void Owner_SetNew()
    {
        Sector target = map.Sectors.First(s => s.Owner == null);
        Assume.That(target.Owner, Is.Null);
        Color originalColor = SectorColor(target);
        Assume.That(originalColor, Is.Not.EqualTo(Players[0].Color));
        bool onCapturedRaised = false;

        target.OnCaptured += (sender, e) => onCapturedRaised = true;
        target.Owner = Players[0];
        Color newColor = SectorColor(target);
        Assert.That(newColor, Is.Not.EqualTo(originalColor));
        Assert.That(newColor, Is.EqualTo(Players[0].Color));
        Assert.That(onCapturedRaised, Is.True);
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
    }

    [Test]
    public void Owner_SetNull()
    {
        Sector target = map.Sectors.First(s => s.Owner == null);
        target.Owner = Players[0];
        Assume.That(target.Owner, Is.EqualTo(Players[0]));
        bool onCapturedRaised = false;

        target.OnCaptured += (sender, e) => onCapturedRaised = true;
        target.Owner = null;
        Assert.That(target.Owner, Is.Null);
        Assert.That(onCapturedRaised, Is.True);
    }

    [Test]
    public void AdjacentSectors_Traversable()
    {
        map.Sectors[0].Stats.ApplyEffect(new CustomEffect { traversable = false });
        Assume.That(map.Sectors[0].Stats.Traversable, Is.False);
        map.Sectors[4].Stats.ApplyEffect(new CustomEffect { traversable = false });
        Assume.That(map.Sectors[4].Stats.Traversable, Is.False);
        foreach (Sector sector in map.Sectors)
            foreach (Sector adjacent in sector.AdjacentSectors)
                Assert.That(adjacent.Stats.Traversable, Is.True);
    }

    [Test]
    public void Highlighted_Toggle()
    {
        Sector target = map.Sectors[0];
        Assume.That(target.Highlighted, Is.False);
        Color originalColor = SectorColor(target);

        target.Highlighted = true;
        Assert.That(target.Highlighted, Is.True);
        Assert.That(SectorColor(target), Is.Not.EqualTo(originalColor));
        target.Highlighted = false;
        Assert.That(target.Highlighted, Is.False);
        Assert.That(SectorColor(target) == originalColor);
    }

    [Test]
    public void Highlighted_DoubleSet()
    {
        Sector target = map.Sectors[0];
        Assume.That(target.Highlighted, Is.False);
        Color originalColor = SectorColor(target);

        target.Highlighted = false;
        Assert.That(target.Highlighted, Is.False);
        Assert.That(SectorColor(target), Is.EqualTo(originalColor));
        target.Highlighted = true;
        Assume.That(target.Highlighted, Is.True);
        Color newColor = SectorColor(target);
        target.Highlighted = true;
        Assert.That(target.Highlighted, Is.True);
        Assert.That(SectorColor(target), Is.EqualTo(newColor));
    }

    // adjacent highlight

    [Test]
    public void TransferUnits_WithNull()
    {
        var search = map.Sectors.Where(s => s.Owner == null && s.Unit == null);
        Sector origin = search.ElementAt(0);
        Assume.That(origin.Owner, Is.Null);
        Assume.That(origin.Unit, Is.Null);
        Sector target = search.ElementAt(1);
        Assume.That(target.Owner, Is.Null);
        Assume.That(target.Unit, Is.Null);

        Unit unit = InitUnit(origin, 0);
        Assume.That(origin.Owner, Is.EqualTo(Players[0]));
        Assume.That(origin.Unit, Is.EqualTo(unit));

        // transfer unit
        origin.TransferUnits(target);
        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.Null);
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(unit));
    }

    [Test]
    public void TransferUnits_WithUnit()
    {
        var search = map.Sectors.Where(s => s.Owner == null && s.Unit == null);
        Sector origin = search.ElementAt(0);
        Assume.That(origin.Owner, Is.Null);
        Assume.That(origin.Unit, Is.Null);
        Sector target = search.ElementAt(1);
        Assume.That(target.Owner, Is.Null);
        Assume.That(target.Unit, Is.Null);

        Unit unitA = InitUnit(origin, 0);
        Assume.That(origin.Owner, Is.EqualTo(Players[0]));
        Assume.That(origin.Unit, Is.EqualTo(unitA));
        Unit unitB = InitUnit(target, 0);
        Assume.That(target.Owner, Is.EqualTo(Players[0]));
        Assume.That(target.Unit, Is.EqualTo(unitB));

        // transfer unit
        origin.TransferUnits(target);
        Assert.That(origin.Owner, Is.EqualTo(Players[0]));
        Assert.That(origin.Unit, Is.EqualTo(unitB));
        Assert.That(target.Owner, Is.EqualTo(Players[0]));
        Assert.That(target.Unit, Is.EqualTo(unitA));
    }
}
#endif
