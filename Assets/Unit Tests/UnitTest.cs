#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

public class UnitTest : BaseGameTest
{
    [SetUp]
    public void UnitTest_SetUp()
    {
        DefMapInit();
        SpawnAllPlayerUnits();
    }

    [Test]
    public void Sector_Sets()
    {
        Sector target = map.Sectors.First(s => s.Unit == null);
        Unit unit = InitUnit(target);
        target = map.Sectors.First(s => s.Unit == null);
        unit.Sector = target;
        Assert.That(unit.Sector, Is.EqualTo(target));
    }

    [Test]
    public void LevelUp_IncreasesLevel()
    {
        Unit unit = map.Sectors.Select(s => s.Unit).First(u => u != null);
        Assert.That(unit.Level, Is.EqualTo(1));
        unit.LevelUp();
        Assert.That(unit.Level, Is.EqualTo(2));
        unit.LevelUp();
        Assert.That(unit.Level, Is.EqualTo(3));
    }

    [Test]
    public void LevelUp_ObeysLevelCap()
    {
        Unit unit = map.Sectors.Select(s => s.Unit).First(u => u != null);
        for (; unit.Level < unit.Stats.LevelCap; unit.LevelUp()) // level up to cap
        { }
        Assert.That(unit.Level, Is.EqualTo(unit.Stats.LevelCap));
        unit.LevelUp();
        Assert.That(unit.Level, Is.EqualTo(unit.Stats.LevelCap));
    }

    [UnityTest]
    public IEnumerator Kill_DestroysAndRaises()
    {
        Unit unit = map.Sectors.Select(s => s.Unit).First(u => u != null);
        bool onDeathRaised = false;
        unit.OnDeath += (sender, e) => onDeathRaised = true;

        Assert.That(unit, Is.Not.Null);
        Assert.That(unit != null); // operator check
        unit.Kill(Players[0]);
        Assert.That(unit == null); // operator check
        Assert.That(onDeathRaised, Is.True);
        yield return null; // allow unit to carry out GameObject destruction
        Assert.That((UnityEngine.MonoBehaviour)unit == null); // monobehaviour check
    }
}
#endif
