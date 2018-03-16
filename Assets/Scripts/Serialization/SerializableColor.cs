using System;
using UnityEngine;

[Serializable]
public class SerializableColor
{
    public float r;
    public float g;
    public float b;
    public float a;

    public Color Color
    {
        get { return new Color(r, g, b, a); }
        set
        {
            r = value.r;
            g = value.g;
            b = value.b;
            a = value.a;
        }
    }

    public static implicit operator SerializableColor(Color color)
    {
        return new SerializableColor
        {
            Color = color
        };
    }

    public static implicit operator Color(SerializableColor color)
    {
        return color.Color;
    }
}
