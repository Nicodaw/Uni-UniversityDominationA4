using System;

public class EliminatedEventArgs : EventArgs
{
    public Player Eliminator { get; }

    public EliminatedEventArgs(Player eliminator)
    {
        Eliminator = eliminator;
    }
}
