using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeoLocation : MonoBehaviour
{
    public float latitude;
    public float longitude;

    public GeoLocation(float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public Vector2 ToVector2()
    {
        return new Vector2(latitude, longitude);
    }
}
