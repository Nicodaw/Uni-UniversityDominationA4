using System;

[Serializable]
public class SerializablePlayer
{
    public PlayerKind kind;
    public int id;
    public SerializableColor color;
    public SerializableEffectManager effectManager;
    public int actionsRemaining;
    public SerializableCardManager cards;
    public bool hasHadTurn;
}
