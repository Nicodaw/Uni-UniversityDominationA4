using System;

[Serializable]
public class SerializablePlayer
{
    public PlayerKind kind;
    public int id;
    public SerializableColor color;
    public SerializableEffectManager effectManager;
    public SerializableCardManager cards;
    public int actionsPerformed;
    public bool hasHadTurn;
}
