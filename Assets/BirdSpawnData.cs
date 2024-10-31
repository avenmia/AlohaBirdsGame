using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "BirdSpawnData", menuName = "Avidex/Bird Spawn Data", order = 2)]
//public class BirdSpawnData : ScriptableObject
//{
//    public BirdData birdData; // Reference to the BirdData asset
//    public float spawnProbability;
//    public float spawnRadius;
//    public List<GeoLocation> possibleLocations;
//}

[System.Serializable]
public class BirdSpawnData : MonoBehaviour
{
    public BirdData birdData; // Reference to the BirdData asset
    public float spawnProbability;
    public float spawnRadius;
    public List<GeoLocation> possibleLocations;
}

[System.Serializable]
public class GeoLocation
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