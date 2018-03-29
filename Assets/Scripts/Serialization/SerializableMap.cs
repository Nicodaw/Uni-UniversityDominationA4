using System;

[Serializable]
public class SerializableMap
{
    public int pvcAllocateWait;
    public int? lastPvcSector;
    public SerializableSector[] sectors;
}
