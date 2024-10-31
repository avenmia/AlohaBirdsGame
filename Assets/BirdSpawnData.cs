using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


// Note needs to not be a serializable object so that the maps object layer can spawn it
[System.Serializable]
public class BirdSpawnData : MonoBehaviour, IPointerClickHandler
{
    public BirdSpawnData birdData; // Reference to the BirdData asset
    public float spawnProbability;
    public float spawnRadius;
    public List<GeoLocation> possibleLocations;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Avendano Map Object Clicked!");

        // Store the bird data in the PersistentDataManager
        PersistentDataManager.Instance.selectedBirdData = birdData;

        Debug.Log($"Opening ARScene With Bird {birdData}");
        // Load the new scene
        // TODO: Change this when NavigationManager gets added
        SceneManager.LoadScene("ARScene");
    }
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