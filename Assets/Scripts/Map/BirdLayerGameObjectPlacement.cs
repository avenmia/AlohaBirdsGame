// using Niantic.Lightship.Maps;
// using Niantic.Lightship.Maps.Core.Coordinates;
// using Niantic.Lightship.Maps.MapLayers.Components;
// using Niantic.Lightship.Maps.ObjectPools;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Esri.GameEngine.Geometry;
using Esri.Unity;                        
using UnityEngine.Pool;
using Esri.ArcGISMapsSDK.Components;
using Unity.Mathematics;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;


public class BirdLayerGameObjectPlacement : MonoBehaviour
{

    [SerializeField] private ArcGISMapComponent _arcGISMap;   // drag your ArcGIS Map here
    [SerializeField] private Transform _layerRoot;    // empty GameObject under the map
    [SerializeField] private GameObject _pigeonPrefab;
    [SerializeField] private GameObject _barnowlPrefab;
    [SerializeField] private GameObject _africansilverbillPrefab;
    [SerializeField] private GameObject _hawaiianDuckPrefab;
    [SerializeField] private GameObject _kalijPheasantPrefab;
    [SerializeField] private GameObject _honeyCreeperPrefab;
    [SerializeField] private GameObject _houseSparrowPrefab;
    [SerializeField] private GameObject _redfowlPrefab;
    [SerializeField] private GameObject _whiteTernPrefab;

    static BirdLayerGameObjectPlacement() => Debug.Log("[DEBUG]: Bird Layer DLL loaded");

    // private readonly Dictionary<GameObject, (LatLng Position, Quaternion Rotation)> _instances = new();
    private readonly Dictionary<GameObject, (ArcGISPoint Position, Quaternion Rotation)> _instances = new();
    private Dictionary<BirdType, ObjectPool<GameObject>> _birdPools;


    [SerializeField] private BirdPrefabRegistry registry;

    private void Start()
    {
        Debug.Log("[DEBUG]: In awake for bird layer game object");
        InitializeArcGIS();
    }

    public void InitializeArcGIS()
    {
        Debug.Log("[DEBUG]: Initializing Bird layer game object placement");

        if (_arcGISMap == null) _arcGISMap = FindObjectOfType<ArcGISMapComponent>();

        if (_arcGISMap == null)
        {
            Debug.Log("[DEBUG]: ARC GIS MAP is still null in initialize");
        }

        // TODO: Not sure what this does?
        if (_layerRoot == null)
        {
            var go = new GameObject("BirdLayer");
            _layerRoot = go.transform;
            _layerRoot.SetParent(_arcGISMap.transform, false);
        }

        SetupBirdPools();
        DOTween.Init();
    }

    public void SetupBirdPools()
    {
        Debug.Log("[DEBUG]: Setting up Bird Pools");
        _birdPools = new();

        foreach (var entry in registry.entries)
        {
            var prefab = entry.prefab;
            var type   = entry.type;

            _birdPools[type] = new ObjectPool<GameObject>(
                () => Instantiate(prefab, _layerRoot),
                go => go.SetActive(true),
                go => go.SetActive(false),
                go => Destroy(go),
                collectionCheck: true,
                defaultCapacity: 8,
                maxSize: 32);
        }
    }

    public GameObject GetBirdPrefab(BirdType birdType)
    {
        Debug.Log($"[DEBUG]: Getting bird prefab {birdType}");
        if (registry == null)
        {
            Debug.Log("[DEBUG]: Bird Prefabs should not be null here");
        }
        Debug.Log($"[DEBUG]: Registry entries in GetBirdPrefab: {registry.entries.Length}");
        return registry.Get(birdType);
    }

    public PooledGO PlaceBirdInstance(
        Vector3 scenePos, Quaternion rotation,
        BirdType birdType, Guid birdId, string instanceName = null)
    {
        Debug.Log("[DEBUG]: 1");
        /* 1 ─ scene  →  geographic */
        var hpPos = new double3(scenePos.x, scenePos.y, scenePos.z);
        ArcGISPoint geo = _arcGISMap.View.WorldToGeographic(hpPos);

        Debug.Log("[DEBUG]: 2");
        /* 2 ─ pooled instance */
        var pool = _birdPools[birdType];
        var go   = pool.Get();
        go.name  = instanceName ?? birdType.ToString();
        _instances[go] = (geo, rotation);

        Debug.Log("[DEBUG]: 3");
        /* 3 ─ ArcGISLocationComponent: only Position & Rotation */
        var loc = go.GetComponent<ArcGISLocationComponent>()
                ?? go.AddComponent<ArcGISLocationComponent>();

        Debug.Log("[DEBUG]: 4");
        loc.Position = geo;

        Debug.Log("[DEBUG]: 5");
        /* 4 ─ visual tweaks & animation */
        Debug.Log($"[DEBUG]: Setting local scale for bird type: {birdType}");
        go.transform.localScale    = registry.Get(birdType).transform.localScale;
        go.transform.localPosition = new Vector3(0, 50, 0);
        go.transform.DOMoveY(10, 5f).SetEase(Ease.OutQuad);

        if (go.TryGetComponent(out ClickToNavigate nav)) nav.birdId = birdId;

        return new PooledGO(go, pool);
    }


    /// <summary>
    /// Places an instance of this component's prefab
    /// at a given <see cref="LatLng"/> coordinate.
    /// </summary>
    /// <param name="position">The location to place the instance</param>
    /// <param name="rotation">A local rotation applied to the placed instance</param>
    /// <param name="instanceName">An optional name to assign to the instance</param>
    /// <returns>An instance placed at the desired <see cref="LatLng"/></returns>
    // public PooledObject<GameObject> PlaceBirdInstance(
    //     in Vector3 vecPosition,
    //     in Quaternion rotation,
    //     BirdType birdType,
    //     Guid birdId,
    //     string instanceName = null)
    // {
    //     try
    //     {

    //         var position = LightshipMapView.SceneToLatLng(vecPosition);
    //         // Get or create a prefab instance from the object pool
    //         var pool = _birdPools[birdType];
    //         var pooledObject = pool.GetOrCreate();

    //         var instance = pooledObject.Value;
    //         Debug.Log($"[DEBUG]: Is instance null: {instance == null}");
    //         instance.name = instanceName ?? birdType.ToString();

    //         Debug.Log($"[DEBUG]: Adding Pooled object value: {instance}");
    //         _instances.Add(instance, (position, rotation));

    //         Debug.Log("[DEBUG]: Getting click to Navigate and assigning ID");
    //         var birdClickHandler = instance.GetComponent<ClickToNavigate>();
    //         if (birdClickHandler != null)
    //         {
    //             Debug.Log("[DEBUG]: Setting ID");
    //             birdClickHandler.birdId = birdId;
    //         }


    //         PositionInstance(instance, position, rotation);
    //         instance.transform.localScale = _birdPrefabs[birdType].transform.localScale;
    //         instance.transform.localPosition += new Vector3(0, 100, 0);
    //         instance.transform.DOMoveY(10, 5f);

    //         return pooledObject;
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"[ERROR]: Unable to place instance for bird ID: {birdId} and {birdType.ToString()}. Exception: {e} \n Stack Trace: {e.StackTrace}");
    //         return default;
    //     }
    // }

    public void RestoreBirdPosition(PooledGO bird)
    {
        // Do we have a geo-position stored for this pooled object?
        if (!_instances.TryGetValue(bird.Value, out var data))
            return;

        var (geo, rot) = data;

        /* 1 ─ update the location component */
        ArcGISLocationComponent loc =
            bird.Value.GetComponent<ArcGISLocationComponent>();

        loc.Position = geo;                         // put it back on the street

        // ArcGISLocationComponent no longer has Heading/Pitch/Roll properties,
        // but it *does* still expose a single Rotation struct.
        //
        // Convert the quaternion we stored into heading-pitch-roll (Y-X-Z order):
        Vector3 eul = rot.eulerAngles;              // degrees
        loc.Rotation = new ArcGISRotation(
            eul.y,                                  // heading  (yaw)
            eul.x,                                  // pitch    (x)
            eul.z);                                 // roll     (z)

        /* 2 ─ ensure the visible transform matches the component */
        bird.Value.transform.rotation = rot;
    }

    public void RemoveBirdInstance(PooledGO bird)
    {
        if (bird.Value != null)
        {
            _instances.Remove(bird.Value);
            bird.Dispose();                       // back to pool
        }
    }

    // public void RestoreBirdPosition(PooledObject<GameObject> birdToRestore)
    // {
    //     Debug.Log($"[DEBUG]: Number of instances when restoring: {_instances.Count}");
    //     if (_instances.TryGetValue(birdToRestore.Value, out var result))
    //     {
    //         var (Position, Rotation) = result;
    //         Debug.Log($"[DEBUG]: Restoring at position: {Position} {Rotation}");
    //         PositionInstance(birdToRestore.Value, Position, Rotation);
    //         Debug.Log($"[DEBUG]: Bird restored successfully");
    //     }
    //     // var (Position, Rotation) = _instances[birdToRestore];
    //     // Debug.Log($"[DEBUG]: Restoring at position: {Position} {Rotation}");
    //     // PositionInstance(birdToRestore, Position, Rotation);
    //     Debug.Log($"[DEBUG]: Bird restored successfully");

    // }

    // public void RemoveBirdInstance(PooledObject<GameObject> birdToRemove)
    // {
    //     if (_instances.TryGetValue(birdToRemove.Value, out var instanceData))
    //     {
    //         Debug.Log($"[DEBUG] Remove Bird Instance GameObject: {birdToRemove.Value}");

    //         _instances.Remove(birdToRemove.Value);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("[DEBUG]: Tried to remove bird instance, but not found in dictionary.");
    //     }

    //     if (birdToRemove.Value != null)
    //     {
    //         Debug.Log("[DEBUG] calling dispose on pooled object");
    //         birdToRemove.Dispose();
    //     }
    //     else
    //     {
    //         Debug.Log("[DEBUG] Object pool value was null and did not dispose");
    //     }
    // }

    public readonly struct PooledGO : IDisposable
    {
        private readonly ObjectPool<GameObject> _pool;
        public readonly GameObject Value;

        internal PooledGO(GameObject value, ObjectPool<GameObject> pool)
        {
            Value = value;
            _pool = pool;
        }

        public void Dispose() => _pool.Release(Value);
    }
}