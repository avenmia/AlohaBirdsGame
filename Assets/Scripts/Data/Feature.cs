using System.Collections.Generic;
using System;

[Serializable]
public class Feature
{
    public string type;
    public Geometry geometry;
    public Dictionary<string, object> properties; // For any additional data.
}