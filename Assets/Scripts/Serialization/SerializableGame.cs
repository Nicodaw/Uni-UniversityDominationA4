using System;

[Serializable]
public class SerializableGame
{
    public bool processEvents;
    public SerializableMap map;
    public SerializablePlayerManager playerManager;
    public int currentPlayerId;
}
