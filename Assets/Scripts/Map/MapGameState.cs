using DataBank;
using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.MapLayers.Components.BaseTypes;
using Niantic.Lightship.Maps.ObjectPools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGameState : MonoBehaviour
{
    public static MapGameState Instance;
    public List<BirdDataObject> birdSpawnDataList; // List of all bird spawn data
    public List<BirdDataObject> spawnedBirds = new List<BirdDataObject>();

    // Birds on the player's map
    public Dictionary<Guid, PooledObject<GameObject>> birdsOnMap = new Dictionary<Guid, PooledObject<GameObject>>();


    private readonly List<MapLayerComponent> _components = new();

    [SerializeField]
    private BirdLayerGameObjectPlacement _birdSpawner;
    
    [SerializeField]
    private Camera _mapCamera;
    [SerializeField]
    private LightshipMapView _lightshipMapView;
    [SerializeField]
    private string _layerName = "MapLayer";

    internal string LayerName
    {
        set => _layerName = value;
    }

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

    public void Initialize(LightshipMapView lightshipMapView, Transform parent)
    {
        var mapLayer = new GameObject(_layerName);
        mapLayer.transform.SetParent(parent);

        _components.AddRange(gameObject.GetComponentsInChildren<MapLayerComponent>());

        foreach (var component in _components)
        {
            component.Initialize(lightshipMapView, mapLayer);
        }
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

        if (_mapCamera.transform.forward == Vector3.zero)
        {
            _mapCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Debug.LogWarning("Camera forward vector was zero. Setting default rotation.");
        }

        if (Input.location != null && Input.location.status == LocationServiceStatus.Running)
        {
           
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            if (_lightshipMapView != null)
            {
                var latlng = new LatLng(playerLocation.x, playerLocation.y);
                _lightshipMapView.SetMapCenter(latlng);

                Debug.Log("[DEBUG] Removing selected bird if captured");
                RemoveSelectedBirdIfCaptured();

                //foreach (var birdData in spawnedBirds)
                //{
                   

                //    SpawnBird(birdData, playerLocation, true);
                //}
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
            if (_mapCamera.transform.forward == Vector3.zero)
            {
                _mapCamera.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                Debug.LogWarning("Camera forward vector was zero. Setting default rotation.");
            }
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

                // TODO: Update this if player's location changes
                // If no birds exist spawn a bird
                if (birdsOnMap.Count == 0 )
                {
                    Debug.Log($"[DEBUG] 0 birds on map. Spawning bird at user's location");
                    MapGameState.Instance.TrySpawnBirdsAtLocation(playerLocation);
                }
            }
        }
    }

    public void RemoveSelectedBirdIfCaptured()
    {
        Debug.Log($"[DEBUG]: Removing PDM selected bird if captured");

        var selectedBird = PersistentDataManager.Instance.selectedBirdData;
        // Debug.Log($"[DEBUG]: onscene loaded birdData: {birdData.id}");
        if (selectedBird != null)
        {
            Debug.Log($"[DEBUG]: PDM Selected bird: {selectedBird.id}");

            // Assuming the selected bird is the same type 
            var filteredUserBirds = PersistentDataManager.Instance.userCapturedBirds.Where(b => b.birdData.birdName == selectedBird.birdName);
            // TODO: Probably inefficient to go through all bird types and all bird IDs
            var userCapturedSelectedBird = filteredUserBirds.Where(b => b.caughtBirds.Contains(selectedBird.id)).FirstOrDefault();

            if (userCapturedSelectedBird != null)
            {
                Debug.Log($"[DEBUG]: User captured Selected bird: {userCapturedSelectedBird}");
                Debug.Log("[DEBUG] Removing selected bird from map and PDM");
                RemoveBird(selectedBird.id);
                if(spawnedBirds.Contains(selectedBird))
                {
                    Debug.Log("[DEBUG] Removing selected bird from spawned birds");
                    spawnedBirds.Remove(selectedBird);
                }
                // Debug.Log($"[DEBUG] Removing bird from spawned birds:{selectedBird.id}");
                // spawnedBirds.Remove(birdData);
            }
        }
    }

    public string GetBirdSpawn(Vector2 playerLocation)
    {
        BirdDb birdDb = new BirdDb();
        IDataReader reader = birdDb.Close_Birds(playerLocation.x, playerLocation.y);
        var potentialBirds = new Dictionary<string, int>();
        string birdNameResult = "";

        while (reader.Read())
        {
            
            // reader [0] = Species Name
            // reader [1] = Number of observations in range
            if (reader.FieldCount == 2)
            {
                var birdSpeciesName = reader[0].ToString();
                if (PersistentDataManager.GameBirdNames.Contains(birdSpeciesName))
                {
                    int numSightings = Int32.Parse(reader[1].ToString());
                    if(potentialBirds.ContainsKey(birdSpeciesName))
                    {
                        potentialBirds[birdSpeciesName] += numSightings;
                    }
                    else
                    {
                        potentialBirds.Add(birdSpeciesName, numSightings );
                    }
                }

            }
        }
        birdDb.close();

        int totalSightings = potentialBirds.Values.Sum();
        var birdProbabilities = new Dictionary<string, double>();

        foreach (var kvp in potentialBirds)
        {
            double probability = (double)kvp.Value / totalSightings;
            birdProbabilities[kvp.Key] = probability;
            Debug.Log($"Bird Species Name: {kvp.Key}, Number of sightings: {kvp.Value}, Probability: {probability}");

        }

        birdNameResult = GetRandomBird(birdProbabilities);
        return birdNameResult;
    }

    string GetRandomBird(Dictionary<string, double> birdProbabilities)
    {
        System.Random random = new System.Random();
        double randomValue = random.NextDouble(); // Value between 0.0 and 1.0
        double cumulative = 0.0;

        foreach (var kvp in birdProbabilities)
        {
            cumulative += kvp.Value;
            if (randomValue < cumulative)
            {
                return kvp.Key;
            }
        }

        return null; // fallback, in case of rounding errors
    }


    // Method to attempt spawning birds at the player's location
    public void TrySpawnBirdsAtLocation(Vector2 playerLocation)
    {

        if (PersistentDataManager.Instance == null || PersistentDataManager.Instance.gameBirds == null)
        {
            Debug.LogWarning("GameBirds should not be null here");
        }

        var bird = GetBirdSpawn(playerLocation);
        var spawnBird = new BirdDataObject()
        {
            id = Guid.NewGuid(),
            birdName = bird,
            birdType = BirdTypeUtil.GetBirdType(bird),
            spawnProbability = 1,
            spawnRadius = 5,
            location = new Vector3(playerLocation.x, playerLocation.y)
        };
        if (!spawnedBirds.Contains(spawnBird))
        {
            spawnedBirds.Add(spawnBird);
            SpawnBird(spawnBird, playerLocation);
        }

    }

    private void SpawnBird(BirdDataObject birdData, Vector2 playerLocation, bool isRespawn = false)
    {
        string pinName = BirdTypeUtil.GetBirdPinName(birdData.birdName);

        if (_mapCamera != null)
        {
            var cameraForward = _mapCamera.transform.forward;
            var forward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            var rotation = Quaternion.LookRotation(forward);

            if(playerLocation == null || rotation == null || birdData == null || birdData.birdName == null)
            {
                Debug.LogWarning("player location, rotation, or bird name should not be null");
            }

            Vector3 spawnPosition = CalculateSpawnPosition(playerLocation, birdData, forward);
                birdData.location = playerLocation;

            var spawnedBird = _birdSpawner.PlaceBirdInstance(spawnPosition, rotation, birdData.birdType, birdData.id);
            birdsOnMap.Add(birdData.id, spawnedBird);
        }
    }

    private void RemoveBird(Guid birdIdToRemove)
    {
        try
        {
            if (birdsOnMap.ContainsKey(birdIdToRemove))
            {
                Debug.Log($"[DEBUG] Removing {birdIdToRemove} from map");
                var birdToRemove = birdsOnMap[birdIdToRemove];
                birdsOnMap.Remove(birdIdToRemove);
                Debug.Log($"[DEBUG] Removing {birdIdToRemove} pooled object");
                _birdSpawner.RemoveBirdInstance(birdToRemove);
                Debug.Log($"[DEBUG] removing {birdIdToRemove} as PDM selected bird");
                PersistentDataManager.Instance.RemoveSelectedBird();
                Debug.Log($"[DEBUG] After selected bird removal: {PersistentDataManager.Instance.selectedBirdData} this should be null");
                Debug.Log($"[DEBUG] Bird Removal complete");
                return;
            }
            Debug.LogError($"[Error:MapGameState]: Removing a bird that is not on the map. {birdIdToRemove}");
        }
        catch(Exception e)
        {
            Debug.LogError($"[Error:MapGameState]: Removing bird with ID: {birdIdToRemove} failed. {e.Message}\n {e.StackTrace}");
        }

    }
    
    public void Spawn_Bird_Button()
    {
        string pinName = BirdTypeUtil.GetBirdPinName("WhiteTern");

        BirdDataObject birdData = new BirdDataObject();
        birdData.birdName = pinName;
        birdData.birdType = BirdType.WhiteTern;
        birdData.spawnProbability = 1;
        birdData.spawnRadius = 0;
        birdData.location = Vector3.zero;
        spawnedBirds.Add(birdData);


        if (_mapCamera != null)
        {
            var cameraForward = _mapCamera.transform.forward;
            var forward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            var rotation = Quaternion.LookRotation(forward);

            // Vector2 playerLocation = new Vector2(21.31624f, -157.858102f);
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            Vector3 spawnPosition = CalculateSpawnPosition(playerLocation, birdData, forward);
            birdData.location = playerLocation;

            _birdSpawner.PlaceBirdInstance(spawnPosition, rotation, birdData.birdType, birdData.id);
        }
    }

    private Vector3 CalculateSpawnPosition(Vector2 playerLocation, BirdDataObject birdData, Vector3 forward)
    {
        var latLng = new LatLng(playerLocation.x, playerLocation.y);

        // Convert the LatLng to scene coordinates
        var scenePosition = _lightshipMapView.LatLngToScene(latLng);

        float offsetDistance = 45.0f;

        // Define the offset distance in Unity units (1 unit = 1 meter)

        // Calculate the spawn position by offsetting the scene position
        if(forward == Vector3.zero)
        {
            forward = new Vector3(0, 2, 1);
        }
        // forward += new Vector3(0, 7, 0);
        var result = scenePosition + forward * offsetDistance;
        return result;
    }

    //private LatLng ScreenPointToLatLong(Vector3 screenPosition)
    //{
    //    var clickRay = _mapCamera.ScreenPointToRay(screenPosition);
    //    var pointOnMap = clickRay.origin + clickRay.direction * (-clickRay.origin.y / clickRay.direction.y);
    //    return _lightshipMapView.SceneToLatLng(pointOnMap);
    //}

}
