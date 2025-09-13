using System;

[Serializable]
public class Geometry
{
    public string type;
    public float[] coordinates; // Assuming a Point with [longitude, latitude]
}