using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Pool;
using Unity.Mathematics;


public class BirdLayerGameObjectPlacement : MonoBehaviour
{

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
        
        var map = MapProvider.Instance.GetMap();

        if (map == null)
        {
            Debug.Log("[DEBUG]: MAP is still null in initialize");
        }

        // TODO: Not sure what this does?
        if (_layerRoot == null)
        {
            var go = new GameObject("BirdLayer");
            _layerRoot = go.transform;
            _layerRoot.SetParent(map.transform, false);
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
        var pool = _birdPools[birdType];
        var go   = pool.Get();
        go.name  = instanceName ?? birdType.ToString();

        MapProvider.Instance.SetPositionFromLocation(go, scenePos);

        Debug.Log($"[DEBUG]: Setting local scale for bird type: {birdType}");
        go.transform.localScale    = registry.Get(birdType).transform.localScale;
        go.transform.localPosition = new Vector3(0, 50, 0);
        go.transform.DOMoveY(10, 5f).SetEase(Ease.OutQuad);

        if (go.TryGetComponent(out ClickToNavigate nav)) nav.birdId = birdId;

        return new PooledGO(go, pool);
    }


    public void RemoveBirdInstance(PooledGO bird)
    {
        if (bird.Value != null)
        {
            bird.Dispose();                       // back to pool
        }
    }

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