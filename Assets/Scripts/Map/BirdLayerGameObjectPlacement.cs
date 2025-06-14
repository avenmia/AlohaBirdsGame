using Niantic.Lightship.Maps;
using Niantic.Lightship.Maps.Core.Coordinates;
using Niantic.Lightship.Maps.MapLayers.Components;
using Niantic.Lightship.Maps.ObjectPools;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class BirdLayerGameObjectPlacement : LayerGameObjectPlacement
{

    [SerializeField] private GameObject _pigeonPrefab;
    [SerializeField] private GameObject _barnowlPrefab;
    [SerializeField] private GameObject _africansilverbillPrefab;
    [SerializeField] private GameObject _hawaiianDuckPrefab;
    [SerializeField] private GameObject _kalijPheasantPrefab;
    [SerializeField] private GameObject _honeyCreeperPrefab;
    [SerializeField] private GameObject _houseSparrowPrefab;
    [SerializeField] private GameObject _redfowlPrefab;
    [SerializeField] private GameObject _whiteTernPrefab;

    private readonly Dictionary<GameObject, (LatLng Position, Quaternion Rotation)> _instances = new();

    private Dictionary<BirdType, ObjectPool<GameObject>> _birdPools;

    private Dictionary<BirdType, GameObject> _birdPrefabs;

    public override void Initialize(LightshipMapView lightshipMapView, GameObject parent)
    {
        _birdPrefabs = new Dictionary<BirdType, GameObject>
        {
            { BirdType.Pigeon, _pigeonPrefab },
            { BirdType.BarnOwl, _barnowlPrefab },
            { BirdType.AfricanSilverbill, _africansilverbillPrefab},
            { BirdType.HawaiianDuck, _hawaiianDuckPrefab },
            { BirdType.HoneyCreeper, _honeyCreeperPrefab },
            { BirdType.KalijPheasant, _kalijPheasantPrefab },
            { BirdType.HouseSparrow, _houseSparrowPrefab },
            { BirdType.RedFowl, _redfowlPrefab },
            { BirdType.WhiteTern, _whiteTernPrefab }
        };

        SetupBirdPools();
        base.Initialize(lightshipMapView, parent);
        DOTween.Init();
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

        _birdPools[BirdType.HawaiianDuck] = new ObjectPool<GameObject>(
            _hawaiianDuckPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.KalijPheasant] = new ObjectPool<GameObject>(
            _kalijPheasantPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.HoneyCreeper] = new ObjectPool<GameObject>(
            _honeyCreeperPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.HouseSparrow] = new ObjectPool<GameObject>(
            _houseSparrowPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.RedFowl] = new ObjectPool<GameObject>(
            _redfowlPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );

        _birdPools[BirdType.WhiteTern] = new ObjectPool<GameObject>(
            _whiteTernPrefab,
            onAcquire: OnObjectPoolAcquire,
            onRelease: OnObjectPoolRelease
        );
    }

    public GameObject GetBirdPrefab(BirdType birdType)
    {
        return _birdPrefabs[birdType];
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
        Guid birdId,
        string instanceName = null)
    {
        try
        {

            var position = LightshipMapView.SceneToLatLng(vecPosition);
            // Get or create a prefab instance from the object pool
            var pool = _birdPools[birdType];
            var pooledObject = pool.GetOrCreate();

            var instance = pooledObject.Value;
            Debug.Log($"[DEBUG]: Is instance null: {instance == null}");
            instance.name = instanceName ?? birdType.ToString();

            Debug.Log($"[DEBUG]: Adding Pooled object value: {instance}");
            _instances.Add(instance, (position, rotation));

            Debug.Log("[DEBUG]: Getting click to Navigate and assigning ID");
            var birdClickHandler = instance.GetComponent<ClickToNavigate>();
            if(birdClickHandler != null )
            {
                Debug.Log("[DEBUG]: Setting ID");
                birdClickHandler.birdId = birdId;
            }


            PositionInstance(instance, position, rotation);
            instance.transform.localScale = _birdPrefabs[birdType].transform.localScale;
            instance.transform.localPosition += new Vector3(0, 100, 0);
            instance.transform.DOMoveY(10, 5f);

            return pooledObject;
        } catch(Exception e)
        {
            Debug.LogError($"[ERROR]: Unable to place instance for bird ID: {birdId} and {birdType.ToString()}. Exception: {e} \n Stack Trace: {e.StackTrace}");
            return default;
        }
    }

    public void RestoreBirdPosition(PooledObject<GameObject> birdToRestore)
    {
        Debug.Log($"[DEBUG]: Number of instances when restoring: {_instances.Count}");
        if (_instances.TryGetValue(birdToRestore.Value, out var result))
        {
            var (Position, Rotation) = result;
             Debug.Log($"[DEBUG]: Restoring at position: {Position} {Rotation}");
            PositionInstance(birdToRestore.Value, Position, Rotation);
            Debug.Log($"[DEBUG]: Bird restored successfully");
        }
        // var (Position, Rotation) = _instances[birdToRestore];
        // Debug.Log($"[DEBUG]: Restoring at position: {Position} {Rotation}");
        // PositionInstance(birdToRestore, Position, Rotation);
        Debug.Log($"[DEBUG]: Bird restored successfully");

    }

    public void RemoveBirdInstance(PooledObject<GameObject> birdToRemove)
    {
        if (_instances.TryGetValue(birdToRemove.Value, out var instanceData))
        {
            Debug.Log($"[DEBUG] Remove Bird Instance GameObject: {birdToRemove.Value}");

            _instances.Remove(birdToRemove.Value);
        }
        else
        {
            Debug.LogWarning("[DEBUG]: Tried to remove bird instance, but not found in dictionary.");
        }

        if (birdToRemove.Value != null)
        {
            Debug.Log("[DEBUG] calling dispose on pooled object");
            birdToRemove.Dispose();
        }
        else
        {
            Debug.Log("[DEBUG] Object pool value was null and did not dispose");
        }
    }

    /// <summary>
    /// Positions and orients a placed object instance
    /// </summary>
    /// <param name="instance">The object being positioned</param>
    /// <param name="location">The object's location</param>
    /// <param name="rotation">The object's local rotation</param>
    private void PositionInstance(GameObject instance, in LatLng location, in Quaternion rotation)
    {
        Debug.Log("[DEBUG]: Positioning instance");
        // Hook this up to the parent and set its transform
        var instanceTransform = GetTransform(instance);
        Debug.Log("[DEBUG]: After instance transform");

        instanceTransform.SetParent(ParentMapLayer.transform, false);
        Debug.Log("[DEBUG]: After setting parent map layer");
        // instanceTransform.localScale = GetObjectScale(LightshipMapView.MapRadius);
        instanceTransform.localRotation = GetObjectRotation(rotation);
        Debug.Log("[DEBUG]: After local rotation");
        instanceTransform.position = GetObjectPosition(location);
        Debug.Log("[DEBUG]: Instance positioned successfully");
    }
}