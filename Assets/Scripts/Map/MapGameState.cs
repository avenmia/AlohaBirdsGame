using DataBank;
using Esri.GameEngine.Geometry;              
using Esri.ArcGISMapsSDK.Components;         
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Esri.HPFramework;
using Esri.GameEngine;




#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class MapGameState : MonoBehaviour
{
    readonly string[] wanted = {
#if UNITY_ANDROID
        Permission.Camera,
        Permission.FineLocation   
#endif
    };
    public static MapGameState Instance;
    public List<BirdDataObject> birdSpawnDataList; // List of all bird spawn data
    public List<BirdDataObject> spawnedBirds = new List<BirdDataObject>();

    // Birds on the player's map
    public Dictionary<Guid, BirdLayerGameObjectPlacement.PooledGO> birdsOnMap
        = new Dictionary<Guid, BirdLayerGameObjectPlacement.PooledGO>();


    [SerializeField]
    private BirdLayerGameObjectPlacement _birdSpawner;
    
    [SerializeField]
    private Camera _mapCamera;
    
    [SerializeField] 
    private ArcGISMapComponent _arcGISMap;

    [SerializeField]
    private string _layerName = "MapLayer";
    [SerializeField]
    private LayerMask birdCollisionMask;

    private bool _mapReady;            
    private Coroutine _waiter;

    private const float DefaultLat  = 21.3096f;
    private const float DefaultLong = 157.8600f;         

    internal string LayerName
    {
        set => _layerName = value;
    }

    void Start()
    {
#if UNITY_ANDROID
        StartCoroutine(CheckAndRequest());
        // #elif UNITY_EDITOR
        // return;
        #else
        StartCoroutine(StartLocationService());
#endif

    }

    #if UNITY_ANDROID
    IEnumerator CheckAndRequest()
    {
        // 1 ─ Gather the ones we still need
        List<string> toAsk = new List<string>();
        foreach (var p in wanted)
            if (!Permission.HasUserAuthorizedPermission(p))
            {
                toAsk.Add(p);
            }

        // 2 ─ If something’s missing, fire one combined dialog
        if (toAsk.Count > 0)
        {
            bool finished = false;

            var cb = new PermissionCallbacks();
            cb.PermissionGranted += _ => { if (AllGranted()) finished = true; };
            cb.PermissionDenied  += _ => finished = true;
            cb.PermissionDeniedAndDontAskAgain += _ => finished = true;

            Permission.RequestUserPermissions(toAsk.ToArray(), cb);

            while (!finished)           // 3 ─ wait until the user responds
                yield return null;
        }

        // 4 ─ Proceed only if every permission is granted
        if (AllGranted())
        {
            yield return StartLocationService();
        }
        else
        {
            // Show your own “permission required” UI or open Settings panel
            Debug.LogWarning("Required permission missing; cannot start AR.");
            // optional: SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    bool AllGranted()
    {
        foreach (var p in wanted)
            if (!Permission.HasUserAuthorizedPermission(p))
                return false;
        return true;
    }
#endif  // UNITY_ANDROID

    void OnEnable()
    {
        _waiter = StartCoroutine(WaitForMapReady());
    }

   

    private IEnumerator StartLocationService()
    {
        Input.location.Start();
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait-- > 0)
            yield return new WaitForSeconds(1);

        // if (!Input.location.isEnabledByUser)
        // {
        //     Debug.LogError("Location service is not enabled by the user.");
        //     yield break;
        // }

        // Input.location.Start();

        // int maxWait = 20;
        // while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        // {
        //     yield return new WaitForSeconds(1);
        //     maxWait--;
        // }

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

    private IEnumerator WaitForMapReady()
    {
            while (_arcGISMap == null || _arcGISMap.View == null || _arcGISMap.View.Map == null)
                yield return null;

            while (_arcGISMap.View.Map.LoadStatus != ArcGISLoadStatus.Loaded) 
                yield return null;

            _mapReady = true;
            Debug.Log("[ArcGIS] Map is ready – starting gameplay systems");

            if (Input.location == null || Input.location.status != LocationServiceStatus.Running)
            {
                StartCoroutine(StartLocationService());
            }
            Debug.Log("[DEBUG] Removing selected bird if captured");
            RemoveSelectedBirdIfCaptured();
            Debug.Log("[DEBUG] Adding Non-Captured birds back in");
            RepopulateMapWithUncapturedBirds();
    }

    private void Awake()
    {
        _waiter = StartCoroutine(WaitForMapReady());
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!_mapReady) return;   
        if (Input.location.status == LocationServiceStatus.Running)
        {
            Vector2 playerLocation = new Vector2(Input.location.lastData.latitude, Input.location.lastData.longitude);
            if (_arcGISMap != null)
            {
                ArcGISPoint wgs84 = new ArcGISPoint(
                    playerLocation.y,            //  X = longitude
                    playerLocation.x,            //  Y = latitude
                    0,
                    ArcGISSpatialReference.WGS84()
                );

                _arcGISMap.OriginPosition = wgs84;
                _mapCamera.GetComponent<ArcGISLocationComponent>().Position = wgs84;

                // TODO: Add check for distance moved

                if (birdsOnMap.Count < 1)
                {
                    int maxBirdCount = 5;
                    int birdCount = UnityEngine.Random.Range(1, maxBirdCount);
                    for (int birdNum = 0; birdNum < birdCount; birdNum++)
                    {
                        Debug.Log($"[DEBUG] There are {birdsOnMap.Count} birds on map. Spawning bird at user's location");
                        MapGameState.Instance.TrySpawnBirdsAtLocation(playerLocation);
                    }
                }
            }
        }
    }

    public void RepopulateMapWithUncapturedBirds()
    {
        Debug.Log($"[DEBUG]: Repopulating birds");
        Debug.Log($"[DEBUG]: Birds on map: {birdsOnMap.Count}");
        foreach (var id in birdsOnMap.Keys.ToList())
        {
            Debug.Log($"[DEBUG]: Restoring Bird:{id}");
            var pooled = birdsOnMap[id];
            var birdData = spawnedBirds.FirstOrDefault(b => b.id == id);
            if (birdData == null)
            {
                Debug.Log("[DEBUG]: Bird is being removed, but should not be happening");
                birdsOnMap.Remove(id);
                continue;
            }

            if (pooled.Value == null)
            {
                Debug.Log("[DEBUG]: Bird was destroyed, recreating");
                var scenePos = birdData.location;
                var camForward = new Vector3(_mapCamera.transform.forward.x, 0, _mapCamera.transform.forward.z);
                var rotation = Quaternion.LookRotation(camForward);

                var newPooled = _birdSpawner.PlaceBirdInstance(
                                        scenePos,
                                        rotation,
                                        birdData.birdType,
                                        birdData.id);
                 Debug.Log("[DEBUG]: Bird was recreated adding back to birds on map");
                birdsOnMap[id] = newPooled;           // replace the stale entry
                continue;

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
            var userCapturedSelectedBird = filteredUserBirds.Where(b => b.caughtBirds.Contains(selectedBird.id)).FirstOrDefault();

            if (userCapturedSelectedBird != null)
            {
                Debug.Log($"[DEBUG]: User captured Selected bird: {userCapturedSelectedBird}");
                Debug.Log("[DEBUG] Removing selected bird from map and PDM");
                RemoveBird(selectedBird.id);
                if (spawnedBirds.Contains(selectedBird))
                {
                    Debug.Log("[DEBUG] Removing selected bird from spawned birds");
                    spawnedBirds.Remove(selectedBird);
                }
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
            location = CalculateSpawnPosition(playerLocation, Vector3.zero)
        };
        if (!spawnedBirds.Contains(spawnBird))
        {
            spawnedBirds.Add(spawnBird);
            SpawnBird(spawnBird);
        }

    }

    private void SpawnBird(BirdDataObject birdData, bool isRespawn = false)
    {
        string pinName = BirdTypeUtil.GetBirdPinName(birdData.birdName);

        if (_mapCamera != null)
        {
            var cameraForward = _mapCamera.transform.forward;
            var forward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            var rotation = Quaternion.LookRotation(forward);

            if(rotation == null || birdData == null || birdData.birdName == null)
            {
                Debug.LogWarning("player location, rotation, or bird name should not be null");
            }

            // birdData.location = playerLocation;

            var spawnedBird = _birdSpawner.PlaceBirdInstance(birdData.location, rotation, birdData.birdType, birdData.id);
            Debug.Log($"[DEBUG]: Adding {birdData.birdType}: {birdData.id} to birds on map");
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

    private Vector3 CalculateSpawnPosition(Vector2 playerLocation, Vector3 forward)
    {
        // TODO: Move to constant
        int maxAttempts = 5;
        float minRadiusFromPlayer = 20f;
        float maxRadiusFromPlayer = 60f;
        float minSeparationBetweenBirds = 30f;

        ArcGISPoint wgs84 = new ArcGISPoint(
            playerLocation.y, //Longitude            
            playerLocation.x, //Latitude            
            0,
            ArcGISSpatialReference.WGS84()
        );

        Debug.Log($"[DEBUG]: WGS84 {wgs84}");

        double3 hpCartesian = _arcGISMap.View.GeographicToWorld(wgs84);
        Debug.Log($"[DEBUG]: hpCartesian {hpCartesian}");
        Vector3 scenePosition    = hpCartesian.ToVector3(); 
   
        Vector3 result;

        for ( int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var random2D   = UnityEngine.Random.insideUnitCircle.normalized;
            var distance   = UnityEngine.Random.Range(minRadiusFromPlayer, maxRadiusFromPlayer);
                
            var candidate  = scenePosition + new Vector3(random2D.x, 0, random2D.y) * distance;
            var colliders = Physics.OverlapSphere(candidate, minSeparationBetweenBirds, birdCollisionMask, QueryTriggerInteraction.Ignore);

            if (colliders.Length > 0)
            {
                continue;
            }

            result = candidate;
            Debug.Log($"[DEBUG]:Returning candidate result {result}");
            return result; 

        }

        // TODO: Remove this?
        Debug.Log("[DEBUG]:Didn't find a candidate position");
        float offsetDistance = 45.0f;

        // Define the offset distance in Unity units (1 unit = 1 meter)

        // Calculate the spawn position by offsetting the scene position
        if(forward == Vector3.zero)
        {
            forward = new Vector3(0, 2, 1);
        }
        // forward += new Vector3(0, 7, 0);
        result = scenePosition + forward * offsetDistance;
        return result;
    }

}
