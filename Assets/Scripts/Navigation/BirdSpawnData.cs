using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



public class BirdSpawnData : MonoBehaviour
{
    public BirdData birdData; // Reference to the BirdData asset
    public float spawnProbability;
    public float spawnRadius;
    public List<GeoLocation> possibleLocations;

   
}
