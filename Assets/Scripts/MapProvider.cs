using System.Collections;
using System.Collections.Generic;
using Esri.ArcGISMapsSDK.Components;
using Esri.GameEngine.Geometry;
using Unity.Mathematics;
using UnityEngine;

public class MapProvider : MonoBehaviour
{
    [SerializeField] private ArcGISMapComponent _arcGISMapComponent;

    public static MapProvider Instance;

    private void Awake()
    {
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

    public ArcGISMapComponent GetMap()
    {
        if (_arcGISMapComponent == null)
        {
            _arcGISMapComponent = GameObject.FindObjectOfType<ArcGISMapComponent>();
        }
        return _arcGISMapComponent;
    }

    public ArcGISPoint GetPoint(double3 pos)
    {
        return _arcGISMapComponent.View.WorldToGeographic(pos);
    }

    public void SetPositionFromLocation(GameObject go, Vector3 scenePos)
    {
        var loc = go.GetComponent<ArcGISLocationComponent>() ?? go.AddComponent<ArcGISLocationComponent>();
        var hpPos = new double3(scenePos.x, scenePos.y, scenePos.z);
        ArcGISPoint geo = GetPoint(hpPos);
        loc.Position = geo;
    }


}
