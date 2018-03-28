#if UNITY_EDITOR
using System.Linq;
using NUnit.Framework;

public class PlayerManagerTest : BaseGameTest
{
    [Test]
    public void Winner_NoWinnerUnits()
    {
        DefMapInit();

        // only make players 0 and 1 non-eliminated
        foreach (Unit unit in Players.Skip(2).SelectMany(p => p.Units))
        {
            unit.Sector.Owner = null;
            unit.Kill(Players[0]);
        }
        Assume.That(map.Sectors.Any(s =>
                                    s.Unit?.Owner == Players[2] ||
                                    s.Unit?.Owner == Players[3]),
                    Is.False);

        Assert.That(map.Sectors.Count(s => s.Owner != null), Is.EqualTo(2));
        Assert.That(map.Sectors.Count(s => s.Unit != null), Is.EqualTo(2));
        Assert.That(Players.Winner, Is.Null);
    }

    [Test]
    public void Winner_NoWinnerSectors()
    {
        DefMapInit();

        // only make players 0 and 1 non-eliminated
        foreach (Sector sector in Players.Skip(2).SelectMany(p => p.Units).Select(u => u.Sector))
            sector.Owner = null;
        foreach (Unit unit in Players.SelectMany(p => p.Units))
            unit.Kill(Players[0]);
        Assume.That(map.Sectors.Any(s => s.Unit != null), Is.False);
        Assume.That(map.Sectors.Any(s =>
                                    s.Owner == Players[2] ||
                                    s.Owner == Players[3]),
                    Is.False);

        Assert.That(map.Sectors.Count(s => s.Owner != null), Is.EqualTo(2));
        Assert.That(map.Sectors.Count(s => s.Unit != null), Is.Zero);
        Assert.That(Players.Winner, Is.Null);
    }

    [Test]
    public void Winner_WinnerUnits()
    {
        DefMapInit();

        // only make players 0 and 1 non-eliminated
        foreach (Unit unit in Players.Skip(1).SelectMany(p => p.Units))
        {
            unit.Sector.Owner = null;
            unit.Kill(Players[0]);
        }
        Assume.That(map.Sectors.Any(s =>
                                    s.Unit?.Owner == Players[1] ||
                                    s.Unit?.Owner == Players[2] ||
                                    s.Unit?.Owner == Players[3]),
                    Is.False);

        Assert.That(map.Sectors.Count(s => s.Owner != null), Is.EqualTo(1));
        Assert.That(map.Sectors.Count(s => s.Unit != null), Is.EqualTo(1));
        Assert.That(Players.Winner, Is.EqualTo(Players[0]));
    }

    [Test]
    public void Winner_WinnerSectors()
    {
        DefMapInit();

        // only make players 0 and 1 non-eliminated
        foreach (Sector sector in Players.Skip(1).SelectMany(p => p.Units).Select(u => u.Sector))
            sector.Owner = null;
        foreach (Unit unit in Players.SelectMany(p => p.Units))
            unit.Kill(Players[0]);
        Assume.That(map.Sectors.Any(s => s.Unit != null), Is.False);
        Assume.That(map.Sectors.Any(s =>
                                    s.Owner == Players[1] ||
                                    s.Owner == Players[2] ||
                                    s.Owner == Players[3]),
                    Is.False);

        Assert.That(map.Sectors.Count(s => s.Owner != null), Is.EqualTo(1));
        Assert.That(map.Sectors.Count(s => s.Unit != null), Is.Zero);
        Assert.That(Players.Winner, Is.EqualTo(Players[0]));
    }

    [Test]
    public void InitPlayers_FourHuman()
    {
        DefaultPlayerInit();

        // ensure creation of 4 players is accurate
        Assert.That(Players[0].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players[1].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players[2].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players[3].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players.Count, Is.EqualTo(4));
    }

    [Test]
    public void InitPlayers_ThreeHumanOneAi()
    {
        AiPlayerInit();

        // ensure game with three players and one neutral is accurate
        Assert.That(Players[0].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players[1].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players[2].Kind, Is.EqualTo(PlayerKind.Human));
        Assert.That(Players[3].Kind, Is.EqualTo(PlayerKind.AI));
        Assert.That(Players.Count, Is.EqualTo(4));
    }

    [Test]
    public void ToNextPlayer_NextPlayerActive()
    {
        DefMapInit();

        int cid = 0;
        Players.ToNextPlayer(ref cid);
        Assert.That(cid, Is.EqualTo(1));
        Assert.That(Players[0].Gui.IsActive, Is.False);
        Assert.That(Players[1].Gui.IsActive, Is.True);

        Players.ToNextPlayer(ref cid);
        Assert.That(cid, Is.EqualTo(2));
        Assert.That(Players[1].Gui.IsActive, Is.False);
        Assert.That(Players[2].Gui.IsActive, Is.True);

        Players.ToNextPlayer(ref cid);
        Assert.That(cid, Is.EqualTo(3));
        Assert.That(Players[2].Gui.IsActive, Is.False);
        Assert.That(Players[3].Gui.IsActive, Is.True);

        Players.ToNextPlayer(ref cid);
        Assert.That(cid, Is.Zero);
        Assert.That(Players[3].Gui.IsActive, Is.False);
        Assert.That(Players[0].Gui.IsActive, Is.True);
    }
}
#endif
