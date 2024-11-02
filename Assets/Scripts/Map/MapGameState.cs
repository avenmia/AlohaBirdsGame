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

    public List<BirdSpawnData> birdSpawnDataList; // List of all bird spawn data

    // TODO: Remove when we're calculating birds based on location
    public List<BirdSpawnData> spawnableBirds = new List<BirdSpawnData>();

    public List<BirdSpawnData> spawnedBirds = new List<BirdSpawnData>();
    
    [SerializeField]
    private LayerGameObjectPlacement _objectSpawner;

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

        if (_mapCamera == null)
        {
            Debug.LogError("Map Camera not found after scene load.");
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

    // Method to get birds that can spawn at a given location
    public List<BirdSpawnData> GetSpawnableBirdsAtLocation(Vector2 playerLocation)
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
        foreach (BirdSpawnData birdData in spawnableBirds)
        {
            // TODO: Uncomment when we add probability in

            //float randomValue = Random.Range(0f, 1f);
            //if (randomValue <= birdData.spawnProbability)
            //{
            //    // Spawn the bird
            //    SpawnBird(birdData, playerLocation);
            //}
            if(!spawnedBirds.Contains(birdData))
            {
                // TODO: This will need to be fixed to account for the spawned birds location
                spawnedBirds.Add(birdData);
                SpawnBird(birdData, playerLocation);
            }
        }


    }

    private void SpawnBird(BirdSpawnData birdData, Vector2 playerLocation)
    {
        // Implement the logic to spawn the bird in your game world
        // For example, instantiate a prefab or show an icon on the map

        Debug.Log($"Spawning {birdData.birdData.name} at location {playerLocation}");
        // var location = ScreenPointToLatLong(touchPosition);

        if (_mapCamera != null)
        {
            var cameraForward = _mapCamera.transform.forward;
            var forward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            var rotation = Quaternion.LookRotation(forward);

            //_objectSpawner.PlaceInstance(location, rotation);

            // TODO: Verify this is right
            _objectSpawner.PlaceInstance(playerLocation, rotation);
            return;
        }
    }

    private LatLng ScreenPointToLatLong(Vector3 screenPosition)
    {
        var clickRay = _mapCamera.ScreenPointToRay(screenPosition);
        var pointOnMap = clickRay.origin + clickRay.direction * (-clickRay.origin.y / clickRay.direction.y);
        return _lightshipMapView.SceneToLatLng(pointOnMap);
    }

    private void Update()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            MapGameState.Instance.TrySpawnBirdsAtLocation(playerLocation);
            // TODO: Remove after player location
            TrySpawnBirdsAtLocation(playerLocation);
        }

        var touchPosition = Vector3.zero;
        bool touchDetected = false;

        if (Input.touchCount == 1)
        {
            if (Input.touches[0].phase == TouchPhase.Ended)
            {
                touchPosition = Input.touches[0].position;
                touchDetected = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            touchPosition = Input.mousePosition;
            touchDetected = true;
        }

        if (touchDetected)
        {
            CheckForInteractableTouch(touchPosition);
        }

    }

    private void CheckForInteractableTouch(Vector3 touchPosition)
    {
        var touchRay = _mapCamera.ScreenPointToRay(touchPosition);

        // raycast into scene and see if we hit a map feature
        if (!Physics.Raycast(touchRay, out var hitInfo))
        {
            return;
        }

        // TODO: Implement to add touching the AR Object
        // check if the collider we hit is a feature
        // var hitResourceItem = hitInfo.collider.GetComponent<MapGameResourceFeature>();
        //if (hitResourceItem == null)
        //{
        //    return;
        //}

        // check if this resource has any units available to consume
        //if (!hitResourceItem.ResourcesAvailable)
        //{
        //    return;
        //}

        // award the player resources for finding this map resource
        //int amount = hitResourceItem.GainResources();
        //MapGameState.Instance.AddResource(hitResourceItem.ResourceType, amount);

        // spawn an animated floating text to show resources being gained
        //var floatingTextPosition = hitInfo.point + Vector3.up * 20.0f;
        //var forward = floatingTextPosition - _mapCamera.transform.position;
        //var rotation = Quaternion.LookRotation(forward, Vector3.up);
        //var floatText = Instantiate(_floatingTextPrefab, floatingTextPosition, rotation);
        //floatText.SetText($"+{amount} {hitResourceItem.ResourceType.ToString()}");
    }
}
