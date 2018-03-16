using System.Collections.Generic;

public class EffectAvailableSelection
{
    public IEnumerable<Player> Players { get; set; }
    public IEnumerable<Sector> Sectors { get; set; }
    public IEnumerable<Unit> Units { get; set; }
}
