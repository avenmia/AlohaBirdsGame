using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.ObjectPools;
using System.Collections.Generic;
using UnityEngine;


public class BirdLayerGameObjectPlacement : LayerGameObjectPlacement
{

    [SerializeField] private GameObject _pigeonPrefab;
    [SerializeField] private GameObject _barnowlPrefab;
    [SerializeField] private GameObject _africansilverbillPrefab;

    private readonly Dictionary<GameObject, (LatLng Position, Quaternion Rotation)> _instances = new();

    private Dictionary<BirdType, ObjectPool<GameObject>> _birdPools;

    private Dictionary<BirdType, GameObject> _birdPrefabs;

    public override void Initialize(LightshipMapView lightshipMapView, GameObject parent)
    {
        _birdPrefabs = new Dictionary<BirdType, GameObject>
        {
            { BirdType.Pigeon, _pigeonPrefab },
            { BirdType.BarnOwl, _barnowlPrefab },
            { BirdType.AfricanSilverbill, _africansilverbillPrefab}
        };

        SetupBirdPools();
        base.Initialize(lightshipMapView, parent);
    }

    public void SetupBirdPools()
    {
        _birdPools = new Dictionary<BirdType, ObjectPool<GameObject>>();

        // Create one pool per prefab
        _birdPools[BirdType.Pigeon] = new ObjectPool<GameObject>(
            _pigeonPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.BarnOwl] = new ObjectPool<GameObject>(
            _barnowlPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.AfricanSilverbill] = new ObjectPool<GameObject>(
            _africansilverbillPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );
    }

    /// <summary>
    /// Places an instance of this component's prefab
    /// at a given <see cref="LatLng"/> coordinate.
    /// </summary>
    /// <param name="position">The location to place the instance</param>
    /// <param name="rotation">A local rotation applied to the placed instance</param>
    /// <param name="instanceName">An optional name to assign to the instance</param>
    /// <returns>An instance placed at the desired <see cref="LatLng"/></returns>
    public PooledObject<GameObject> PlaceBirdInstance(
        in Vector3 vecPosition,
        in Quaternion rotation,
        BirdType birdType,
        string instanceName = null)
    {
        var position = LightshipMapView.SceneToLatLng(vecPosition);
        // Get or create a prefab instance from the object pool
        var pool = _birdPools[birdType];
        var pooledObject = pool.GetOrCreate();

        var instance = pooledObject.Value;
        instance.name = instanceName ?? birdType.ToString();

        _instances.Add(instance, (position, rotation));

        PositionInstance(instance, position, rotation);

        return pooledObject;
    }

    /// <summary>
    /// Positions and orients a placed object instance
    /// </summary>
    /// <param name="instance">The object being positioned</param>
    /// <param name="location">The object's location</param>
    /// <param name="rotation">The object's local rotation</param>
    private void PositionInstance(GameObject instance, in LatLng location, in Quaternion rotation)
    {
        // Hook this up to the parent and set its transform
        var instanceTransform = GetTransform(instance);
        instanceTransform.SetParent(ParentMapLayer.transform, false);
        instanceTransform.localScale = GetObjectScale(LightshipMapView.MapRadius);
        instanceTransform.localRotation = GetObjectRotation(rotation);
        instanceTransform.position = GetObjectPosition(location);
    }
}