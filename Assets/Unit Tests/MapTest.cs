#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;

public class MapTest : BaseGameTest
{
    [SetUp]
    public void MapTest_SetUp() => DefMapInit();

    [Test]
    public void LandmarkedSectors_Correct()
    {
        Assert.That(map.LandmarkedSectors, Is.EquivalentTo(map.Sectors.Where(s => s.Landmark != null)));
    }

    [Test]
    public void AllocatePVC_Allocated()
    {
        foreach (Sector sector in map.Sectors.Where(s => s.HasPVC))
            sector.Stats.RemoveEffect<EffectImpl.PVCEffect>();
        Assume.That(map.Sectors.Any(s => s.HasPVC), Is.False);

        map.AllocatePVC();
        Assert.That(map.Sectors.Count(s => s.HasPVC), Is.EqualTo(1));
    }

    [Test]
    public void AllocatePVC_Reallocated()
    {
        foreach (Sector sector in map.Sectors.Where(s => s.HasPVC))
            sector.Stats.RemoveEffect<EffectImpl.PVCEffect>();
        Assume.That(map.Sectors.Any(s => s.HasPVC), Is.False);

        map.AllocatePVC();
        Sector original = map.Sectors.First(s => s.HasPVC);
        map.ResetPVCAllocateWait();
        map.AllocatePVC();
        Assert.That(map.Sectors.Count(s => s.HasPVC), Is.EqualTo(1));
        Assert.That(map.Sectors.First(s => s.HasPVC), Is.Not.EqualTo(original));
    }
}
#endif
