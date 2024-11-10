using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGameState : MonoBehaviour
{
    public static MapGameState Instance;

    public List<BirdDataObject> birdSpawnDataList; // List of all bird spawn data

    // TODO: Remove when we're calculating birds based on location
    public List<BirdDataObject> spawnableBirds = new List<BirdDataObject>();

    public List<BirdDataObject> spawnedBirds = new List<BirdDataObject>();
    
    [SerializeField]
    private LayerGameObjectPlacement _mynaSpawner;

    [SerializeField]
    private LayerGameObjectPlacement _owlSpawner;

    [SerializeField]
    private LayerGameObjectPlacement _pigeonSpawner;

    [SerializeField]
    private Camera _mapCamera;

    [SerializeField]
    private LightshipMapView _lightshipMapView;

    void Start()
    {
        StartCoroutine(StartLocationService());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reassign the _mapCamera when the scene is loaded
        _mapCamera = Camera.main; // Or find it by name or tag if it's not the main camera
        _lightshipMapView = FindObjectOfType<LightshipMapView>();
        

        if (_mapCamera == null)
        {
            Debug.LogError("Map Camera not found after scene load.");
        }

        if (Input.location != null && Input.location.status == LocationServiceStatus.Running)
        {
           
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            if (_lightshipMapView != null)
            {
                var latlng = new LatLng(playerLocation.x, playerLocation.y);
                _lightshipMapView.SetMapCenter(latlng);

                foreach (var birdData in spawnedBirds)
                {
                    Debug.Log($"Alexandra spawning bird data: {birdData.birdName}");
                    SpawnBird(birdData, playerLocation, true);
                }
            }
        }

    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
            Debug.LogError("Location service is not enabled by the user.");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait <= 0)
        {
            Debug.LogError("Timed out");
            yield break;
        }

        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.LogError("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude);
        }
    }

    private void Awake()
    {
        _mapCamera = Camera.main;
        if (_mapCamera != null)
        {
            Debug.Log("Map Camera assigned in Awake.");
        }
        else
        {
            Debug.LogError("Map Camera is null in Awake.");
        }
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            if (_lightshipMapView != null)
            {
                var latlng = new LatLng(playerLocation.x, playerLocation.y);
                _lightshipMapView.SetMapCenter(latlng);

                // TODO: Implemnt to try to spawn birds constantly
                //if (Vector3.Distance(playerLocation, lastPlayerPosition) >= movementThreshold)
                //{
                //    lastPlayerPosition = playerLocation;
                //    // TODO: Implement
                //    // CheckForNewBirds(currentPlayerPosition);
                //}
                MapGameState.Instance.TrySpawnBirdsAtLocation(playerLocation);
            }
        }
    }

    // Method to get birds that can spawn at a given location
    public List<BirdDataObject> GetSpawnableBirdsAtLocation(Vector2 playerLocation)
    {
        // TODO: Uncomment when we're getting birds based on the user's location 

        //List<BirdSpawnData> spawnableBirds = new List<BirdSpawnData>();

        //foreach (BirdSpawnData birdData in birdSpawnDataList)
        //{
        //    foreach (GeoLocation location in birdData.possibleLocations)
        //    {
        //        float distance = GetDistance(playerLocation, location.ToVector2());

        //        if (distance <= birdData.spawnRadius)
        //        {
        //            spawnableBirds.Add(birdData);
        //            break; // No need to check other locations for this bird
        //        }
        //    }
        //}

        //return spawnableBirds;
        return birdSpawnDataList;
    }

    // Helper method to calculate distance between two lat/lon points using the Haversine formula
    private float GetDistance(Vector2 point1, Vector2 point2)
    {
        float lat1Rad = point1.x * Mathf.Deg2Rad;
        float lon1Rad = point1.y * Mathf.Deg2Rad;
        float lat2Rad = point2.x * Mathf.Deg2Rad;
        float lon2Rad = point2.y * Mathf.Deg2Rad;

        float dLat = lat2Rad - lat1Rad;
        float dLon = lon2Rad - lon1Rad;

        float a = Mathf.Pow(Mathf.Sin(dLat / 2), 2) +
                  Mathf.Cos(lat1Rad) * Mathf.Cos(lat2Rad) *
                  Mathf.Pow(Mathf.Sin(dLon / 2), 2);

        float c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));

        float earthRadius = 6371000; // Earth's radius in meters

        float distance = earthRadius * c;

        return distance; // Distance in meters
    }

    // Method to attempt spawning birds at the player's location
    public void TrySpawnBirdsAtLocation(Vector2 playerLocation)
    {
        // TODO: Add when we incorporate player location
        // List<BirdSpawnData> spawnableBirds = GetSpawnableBirdsAtLocation(playerLocation);

        if (PersistentDataManager.Instance == null || PersistentDataManager.Instance.gameBirds == null)
        {
            Debug.LogWarning("GameBirds should not be null here");
        }

        // TODO: Instead of getting all gameBirds, get the birds that are within a distance of the player
        var birdsInPlayersArea = PersistentDataManager.Instance.gameBirds;
        foreach(var birdKeyValue in birdsInPlayersArea)
        {
            var birdDataValue = birdKeyValue.Value.birdData;
            
            if (birdDataValue.birdName != null && spawnableBirds.Find(b => b.birdName == birdDataValue.birdName) == null)
            {
                spawnableBirds.Add(new BirdDataObject()
                {
                    birdName = birdDataValue.birdName,
                    spawnProbability = 1,
                    spawnRadius = 5,
                    location = new Vector3(playerLocation.x, playerLocation.y)
                });
            }
        }
        foreach (BirdDataObject birdData in spawnableBirds)
        {
            // TODO: Uncomment when we add probability in

            //float randomValue = Random.Range(0f, 1f);
            //if (randomValue <= birdData.spawnProbability)
            //{
            //    // Spawn the bird
            //    SpawnBird(birdData, playerLocation);
            //}

            // TODO: This will need to be fixed to account for the spawned birds location

            if (!spawnedBirds.Contains(birdData))
            {

                spawnedBirds.Add(birdData);
                SpawnBird(birdData, playerLocation);
            }
        }
    }

    private void SpawnBird(BirdDataObject birdData, Vector2 playerLocation, bool isRespawn = false)
    {
        string pinName;
        switch (birdData.birdName)
        {
            case "Common Myna": pinName = "MynaPin"; break;
            case "Barn Owl": pinName = "BarnOwlPin"; break;
            case "Pigeon": pinName = "PigeonPin"; break;


            default: pinName = null; break;
        }
        if (pinName == null || GameObject.FindGameObjectWithTag(pinName) != null)
        {
            return; // Bird already exists
        }


        if (_mapCamera != null)
        {
            var cameraForward = _mapCamera.transform.forward;
            var forward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            var rotation = Quaternion.LookRotation(forward);

            Vector3 spawnPosition;

            if (isRespawn && birdData.location != Vector3.zero)
            {
                // Use the stored scene position
                spawnPosition = birdData.location;
            }
            else
            {
                // Calculate a new spawn position
                // spawnPosition = CalculateSpawnPosition(playerLocation, rotation);
                // Convert playerLocation (latitude, longitude) to a LatLng object
                var latLng = new LatLng(playerLocation.x, playerLocation.y);

                // Convert the LatLng to scene coordinates
                var scenePosition = _lightshipMapView.LatLngToScene(latLng);

                float offsetDistance;
                if (birdData.birdName == "Barn Owl")
                {
                    offsetDistance = 10.0f;
                }
                else if (birdData.birdName == "Pigeon")
                {
                    offsetDistance = 5.0f;
                }
                else
                {
                    offsetDistance = 25.0f;
                }

                // Define the offset distance in Unity units (1 unit = 1 meter)

                // Calculate the spawn position by offsetting the scene position
                spawnPosition = scenePosition + forward * offsetDistance;

                // Store the spawn position in the bird data
                birdData.location = spawnPosition;
            }

            // TODO: Verify this is right
            switch (birdData.birdName)
            {
                case "Common Myna": _mynaSpawner.PlaceInstance(spawnPosition, rotation); return;
                case "Barn Owl": _owlSpawner.PlaceInstance(spawnPosition, rotation); return;
                case "Pigeon": _pigeonSpawner.PlaceInstance(spawnPosition, rotation); return;
                default: Debug.LogWarning($"Bird spawner does not exist for ${birdData.birdName}"); return;
            }
        }
    }

    private LatLng ScreenPointToLatLong(Vector3 screenPosition)
    {
        var clickRay = _mapCamera.ScreenPointToRay(screenPosition);
        var pointOnMap = clickRay.origin + clickRay.direction * (-clickRay.origin.y / clickRay.direction.y);
        return _lightshipMapView.SceneToLatLng(pointOnMap);
    }

}
