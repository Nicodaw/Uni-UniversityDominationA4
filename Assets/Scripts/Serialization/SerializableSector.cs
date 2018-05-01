using System;

[Serializable]
public class SerializableSector
{
    public SerializableEffectManager effectManager;
    public SerializableUnit unit;
    public int? owner;
    public bool blockPrefabActive;
    public bool leafletGuyPrefabActive;
}
