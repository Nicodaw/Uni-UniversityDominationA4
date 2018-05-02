using System.Collections.Generic;

/// <summary>
/// Used to store the available selection of the effect.
/// </summary>
public class EffectAvailableSelection
{
    public IEnumerable<Player> Players { get; set; }
    public IEnumerable<Sector> Sectors { get; set; }
    public IEnumerable<Unit> Units { get; set; }
}
