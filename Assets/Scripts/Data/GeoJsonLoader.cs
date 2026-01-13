using Newtonsoft.Json.Linq;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class GeoJsonLoader : MonoBehaviour
{
    public string geoJsonFilePath = "Assets/Resources/GameBirds/Data/bird_sightings.geojson"; // Adjust the path
    public GameObject objectPrefab; // Assign a prefab to spawn

    public void Start()
    {
        LoadGeoJson();
    }

    public void LoadGeoJson()
    {
        var data = Resources.Load<TextAsset>("BirdLocationData/small_bird_sightings");

        if (data == null)
        {
            Debug.LogError("bird location geojson file not found!");
            return;
        }
        FeatureCollection featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(data.text);
        if (featureCollection?.features == null)
        {
            Debug.LogError("No features found in GeoJSON data!");
            return;
        }

        foreach (Feature feature in featureCollection.features)
        {
            if (feature.geometry?.coordinates != null && feature.geometry.coordinates.Length >= 2)
            {
                float longitude = feature.geometry.coordinates[0];
                float latitude = feature.geometry.coordinates[1];

                Debug.Log($"Bird sighting at longitude: {longitude}, latitude: {latitude}");

                // Convert geographic coordinates to a Unity world position.
                // This conversion depends on your AR mapping system (e.g., Niantic Lightship Maps).
                Vector3 worldPosition = ConvertGeoToWorldPosition(latitude, longitude);

                // Example: Instantiate an AR marker at this position.
                // Instantiate(markerPrefab, worldPosition, Quaternion.identity);
            }
        }
    }

    // Replace with your actual conversion logic using Niantic Lightship Maps or your custom method.
    Vector3 ConvertGeoToWorldPosition(float latitude, float longitude)
    {
        // This is a simplified placeholder conversion.
        return new Vector3(longitude, 0, latitude);
    }

    void SpawnObjectAtLocation(double latitude, double longitude)
    {
        Vector3 worldPosition = GeoToUnityPosition(latitude, longitude);
        Instantiate(objectPrefab, worldPosition, Quaternion.identity);
    }

    Vector3 GeoToUnityPosition(double latitude, double longitude)
    {
        // Convert geographic coordinates to Unity world space
        // This conversion depends on your Lightship map settings
        // Assuming your AR session anchors to a reference point

        // Example conversion (adjust based on your world origin setup)
        float x = (float)(longitude * 100.0);
        float z = (float)(latitude * 100.0);

        return new Vector3(x, 0, z); // Keep y as 0 for ground-level spawning
    }
}