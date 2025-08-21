// COPYRIGHT 1995-2021 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using Esri.ArcGISMapsSDK.Authentication;
using Esri.ArcGISMapsSDK.Memory;
using Esri.ArcGISMapsSDK.Security;
using Esri.ArcGISMapsSDK.Utils;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.ArcGISMapsSDK.Utils.Math;
using Esri.GameEngine;
using Esri.GameEngine.Authentication;
using Esri.GameEngine.Elevation.Base;
using Esri.GameEngine.Extent;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Layers;
using Esri.GameEngine.Layers.Base;
using Esri.GameEngine.Layers.BuildingScene;
using Esri.GameEngine.Map;
using Esri.GameEngine.View;
using Esri.HPFramework;
using Esri.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Esri.ArcGISMapsSDK.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[RequireComponent(typeof(HPRoot))]
	[AddComponentMenu("ArcGIS Maps SDK/ArcGIS Map")]
	public class ArcGISMapComponent : MonoBehaviour, IMemorySystem, IArcGISMapComponentInterface, ISerializationCallbackReceiver
	{
		[SerializeField]
		private string apiKey = "";
		public string APIKey
		{
			get => apiKey;
			set
			{
				if (apiKey != value)
				{
					apiKey = value != null ? value : "";
					InitializeArcGISMap();
				}
			}
		}

		[SerializeField]
		private string basemap = "";
		private string lastBasemap = null;
		public string Basemap
		{
			get => basemap;
			set
			{
				SetBasemapSourceAndType(value, basemapType);
			}
		}

		[SerializeField]
		private BasemapTypes basemapType = BasemapTypes.Basemap;
		private BasemapTypes? lastBasemapType = null;
		public BasemapTypes BasemapType
		{
			get => basemapType;
			set
			{
				SetBasemapSourceAndType(basemap, value);
			}
		}

		[SerializeField]
#pragma warning disable 618
		private OAuthAuthenticationConfigurationMapping basemapAuthentication;
#pragma warning restore 618
		[SerializeField]
		private ArcGISAuthenticationType basemapAuthenticationType = ArcGISAuthenticationType.APIKey;
		private ArcGISAuthenticationType lastBasemapAuthenticationType = ArcGISAuthenticationType.APIKey;
		public ArcGISAuthenticationType BasemapAuthenticationType
		{
			get => basemapAuthenticationType;
			set
			{
				if (basemapAuthenticationType != value)
				{
					basemapAuthenticationType = value;

					// If the map either failed to load or was unloaded,
					// updating the layers could be enough for the map to succeed at loading so we will retry
					LoadMap();
				}
			}
		}

		[SerializeField]
		private ArcGISMapElevationInstanceData mapElevation = new ArcGISMapElevationInstanceData();

		public ArcGISMapElevationInstanceData MapElevation
		{
			get
			{
				return mapElevation;
			}
			set
			{
				if (mapElevation == value)
				{
					return;
				}

				mapElevation = value ?? new ArcGISMapElevationInstanceData();

				UpdateElevation();

				// If the map either failed to load or was unloaded,
				// updating the layers could be enough for the map to succeed at loading so we will retry
				LoadMap();
			}
		}

		[SerializeField]
		private bool enableExtent = false;
		public bool EnableExtent
		{
			get => enableExtent;
			set
			{
				if (enableExtent != value)
				{
					enableExtent = value;
					UpdateExtent();
				}
			}
		}

		[SerializeField]
		private ArcGISExtentInstanceData extent = new ArcGISExtentInstanceData() { GeographicCenter = new ArcGISPoint(0, 0, 0, ArcGISSpatialReference.WGS84()) };
		public ArcGISExtentInstanceData Extent
		{
			get => extent;
			set
			{
				if (extent != value)
				{
					extent = value != null ? value : new ArcGISExtentInstanceData();
					UpdateExtent();
				}
			}
		}

		// As a user, if you want to programmatically work with the layers collection use the ArcGISMapComponent.View.Map.Layers collection
		[SerializeField]
		private List<ArcGISLayerInstanceData> layers = new List<ArcGISLayerInstanceData>();
		public List<ArcGISLayerInstanceData> Layers
		{
			get
			{
				return layers;
			}
			set
			{
				if (layers != value)
				{
					layers = value ?? new List<ArcGISLayerInstanceData>();
					UpdateLayers();

					// If the map either failed to load or was unloaded,
					// updating the layers could be enough for the map to succeed at loading so we will retry
					LoadMap();
				}
			}
		}

		public ArcGISMap Map
		{
			get
			{
				return View?.Map;
			}
			set
			{
				if (!value)
				{
					return;
				}

				MapType = value.MapType;
				View.Map = value;
#if UNITY_EDITOR
				lastMap = value;
#endif
			}
		}

		private IMemorySystemHandler memorySystemHandler;
		public IMemorySystemHandler MemorySystemHandler
		{
			get
			{
				if (memorySystemHandler == null)
				{
#if UNITY_ANDROID
					memorySystemHandler = new AndroidDefaultMemorySystemHandler();
#else
					memorySystemHandler = new DefaultMemorySystemHandler();
#endif
				}

				return memorySystemHandler;
			}
			set
			{
				if (memorySystemHandler != value)
				{
					memorySystemHandler = value;

					if (memorySystemHandler != null && view != null)
					{
						InitializeMemorySystem();
					}
				}
			}
		}

		[SerializeField]
		private ArcGISPoint originPosition = new ArcGISPoint(0, 0, 0, ArcGISSpatialReference.WGS84());
		public ArcGISPoint OriginPosition
		{
			get => originPosition;
			set
			{
				if (originPosition != value)
				{
					originPosition = value;
					OnOriginPositionChanged();
				}
			}
		}

		[SerializeField]
		private ArcGISMapType mapType = ArcGISMapType.Local;
		public ArcGISMapType MapType
		{
			get => mapType;
			set
			{
				if (mapType != value)
				{
					mapType = value;
					OnMapTypeChanged();
				}
			}
		}

		[SerializeField]
		private bool useCustomMapSpatialReference = false;
		public bool UseCustomMapSpatialReference
		{
			get => useCustomMapSpatialReference;
			set
			{
				if (useCustomMapSpatialReference == value)
				{
					return;
				}

				useCustomMapSpatialReference = value;

				UpdateCustomMapSpatialReference();
			}
		}

		[SerializeField]
		private ArcGISSpatialReference customMapSpatialReference = ArcGISSpatialReference.WebMercator();
		public ArcGISSpatialReference CustomMapSpatialReference
		{
			get => customMapSpatialReference;
			set
			{
				if (value != null && customMapSpatialReference != null && customMapSpatialReference.Equals(value))
				{
					return;
				}

				customMapSpatialReference = value;

				UpdateCustomMapSpatialReference();
			}
		}

#if UNITY_EDITOR
		private ArcGISMap lastMap = null;

		[SerializeField]
		private bool editorModeEnabled = true;
		private bool? lastEditorModeEnabled = null;
		public bool EditorModeEnabled
		{
			get => editorModeEnabled;
			set
			{
				if (editorModeEnabled != value && !Application.isPlaying)
				{
					editorModeEnabled = value;

					UpdateEditorModeEnabled();
				}
			}
		}

		[SerializeField]
		private bool dataFetchWithSceneView = true;
		private bool? lastDataFetchWithSceneView = null;
		public bool DataFetchWithSceneView
		{
			get => dataFetchWithSceneView;

			set
			{
				if (dataFetchWithSceneView != value && !Application.isPlaying)
				{
					dataFetchWithSceneView = value;

					UpdateDataFetchWithSceneView();
				}
			}
		}

		[SerializeField]
		private bool rebaseWithSceneView = false;
		private bool? lastRebaseWithSceneView = null;
		public bool RebaseWithSceneView
		{
			get => rebaseWithSceneView;

			set
			{
				if (rebaseWithSceneView != value && !Application.isPlaying)
				{
					rebaseWithSceneView = value;

					UpdateRebaseWithSceneView();
				}
			}
		}
#endif

		[SerializeField]
		private bool meshCollidersEnabled = false;
		private bool? lastMeshCollidersEnabled = null;
		public bool MeshCollidersEnabled
		{
			get => meshCollidersEnabled;
			set
			{
				if (meshCollidersEnabled != value)
				{
					meshCollidersEnabled = value;

					UpdateMeshCollidersEnabled();
				}
			}
		}

		private ArcGISView view = null;
		public ArcGISView View
		{
			get
			{
				if (!view)
				{
					view = new ArcGISView(ArcGISGameEngineType.Unity, Esri.GameEngine.MapView.ArcGISGlobeModel.Ellipsoid);

					view.SpatialReferenceChanged += () =>
					{
						hasSpatialReference = view?.SpatialReference != null;

						internalHasChanged = true;
					};

					InitializeMemorySystem();
				}

				return view;
			}
		}

		private double3 universePosition;
		public double3 UniversePosition
		{
			get => hpRoot.RootUniversePosition;
			set
			{
				if (!universePosition.Equals(value) || !hpRoot.RootUniversePosition.Equals(value))
				{
					universePosition = value;
					OnUniversePositionChanged();
				}
			}
		}

		public Quaternion UniverseRotation
		{
			get => hpRoot.RootUniverseRotation;
		}

		public double4x4 WorldMatrix
		{
			get
			{
				return hpRoot.WorldMatrix;
			}
		}

		public static ArcGISElevationSourceInstanceData DefaultElevationSourceData
		{
			get
			{
				return new ArcGISElevationSourceInstanceData()
				{
					IsEnabled = false,
					Name = "Terrain 3D",
					Source = "https://elevation3d.arcgis.com/arcgis/rest/services/WorldElevation3D/Terrain3D/ImageServer",
					Type = ArcGISElevationSourceType.ArcGISImageElevationSource
				};
			}
		}

		[SerializeField]
#pragma warning disable 618
		private List<ArcGISAuthenticationConfigurationInstanceData> configurations = new List<ArcGISAuthenticationConfigurationInstanceData>();
#pragma warning restore 618
		[SerializeField]
		private List<ArcGISOAuthUserConfiguration> oauthUserConfigurations = new List<ArcGISOAuthUserConfiguration>();
		internal List<ArcGISOAuthUserConfiguration> OAuthUserConfigurations => oauthUserConfigurations;

		private GameObject rendererComponentGameObject = null;

		public delegate void MapTypeChangedEventHandler();

		public delegate void EditorModeEnabledChangedEventHandler();

		public delegate void MeshCollidersEnabledChangedEventHandler();

		public event MapTypeChangedEventHandler MapTypeChanged;
#if UNITY_EDITOR
		public event EditorModeEnabledChangedEventHandler EditorModeEnabledChanged;
#endif
		public event ArcGISExtentUpdatedEventHandler ExtentUpdated;

		public event MeshCollidersEnabledChangedEventHandler MeshCollidersEnabledChanged;

		public UnityEvent RootChanged = new UnityEvent();

		private bool hasSpatialReference = false;
		private HPRoot hpRoot;
		private bool internalHasChanged = false;
		private Quaternion universeRotation;

#if UNITY_EDITOR
		private ArcGISEditorCameraComponent editorCameraComponent = null;
#endif

		private void Awake()
		{
			if (originPosition != null && originPosition.IsValid)
			{
				// Ensure HPRoot is sync'd from geoPosition, rather than geoPosition being sync'd from HPRoot
				internalHasChanged = true;
			}

			rendererComponentGameObject = gameObject.GetComponentInChildren<ArcGISRendererComponent>()?.gameObject;

			if (rendererComponentGameObject == null)
			{
				rendererComponentGameObject = new GameObject("ArcGISRenderer");

				rendererComponentGameObject.hideFlags = HideFlags.HideAndDontSave;
				rendererComponentGameObject.transform.SetParent(transform, false);

				var rendererComponent = rendererComponentGameObject.AddComponent<ArcGISRendererComponent>();

				rendererComponent.ExtentUpdated += delegate (ArcGISExtentUpdatedEventArgs e)
				{
					ExtentUpdated?.Invoke(e);
				};
			}
		}

		private void InitializeMemorySystem()
		{
			MemorySystemHandler.Initialize(this);

			SetMemoryQuotas(MemorySystemHandler.GetMemoryQuotas());
		}

		public bool HasSpatialReference()
		{
			return hasSpatialReference;
		}

		private void OnEnable()
		{
			hpRoot = GetComponent<HPRoot>();

			if (rendererComponentGameObject)
			{
				rendererComponentGameObject.SetActive(true);
			}

			SyncPositionWithHPRoot();

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				EnableMainCameraView(!dataFetchWithSceneView);

				// Avoid repeated element when ArcGISMapComponent is copied.
				var lastEditorCameraComponent = GetComponentInChildren<ArcGISEditorCameraComponent>();

				if (lastEditorCameraComponent)
				{
					DestroyImmediate(lastEditorCameraComponent.gameObject);
				}

				var editorCameraComponentGameObject = new GameObject("ArcGISEditorCamera");

				editorCameraComponentGameObject.hideFlags = HideFlags.HideAndDontSave;
				editorCameraComponentGameObject.transform.SetParent(transform);
				editorCameraComponentGameObject.SetActive(false);

				editorCameraComponent = editorCameraComponentGameObject.AddComponent<ArcGISEditorCameraComponent>();
				editorCameraComponent.WorldRepositionEnabled = rebaseWithSceneView;
				editorCameraComponent.EditorViewEnabled = dataFetchWithSceneView;

				editorCameraComponentGameObject.SetActive(true);

				if (ArcGISProjectSettingsAsset.Instance != null)
				{
					ArcGISProjectSettingsAsset.Instance.OnCollisionEnabledInEditorWorldChanged += HandleOnForecCollisionInEditoWorldChanged;
				}
			}
			else
			{
#endif
				EnableMainCameraView(true);
#if UNITY_EDITOR
			}
#endif

#if UNITY_EDITOR
			if (BuildPipeline.isBuildingPlayer)
			{
				return;
			}
#endif

			InitializeArcGISMap();
		}

		private void OnDisable()
		{
			if (rendererComponentGameObject)
			{
				rendererComponentGameObject.SetActive(false);
			}

#if UNITY_EDITOR
			if (!Application.isPlaying && editorCameraComponent)
			{
				EnableMainCameraView(true);
				DestroyImmediate(editorCameraComponent.gameObject);

				if (ArcGISProjectSettingsAsset.Instance != null)
				{
					ArcGISProjectSettingsAsset.Instance.OnCollisionEnabledInEditorWorldChanged -= HandleOnForecCollisionInEditoWorldChanged;
				}
			}
#endif
		}

		internal void InitializeArcGISMap()
		{
			ArcGISMap map;

			if (useCustomMapSpatialReference)
			{
				map = new ArcGISMap(customMapSpatialReference, MapType);
			}
			else
			{
				map = new ArcGISMap(MapType);
			}

			View.Map = map;
#if UNITY_EDITOR
			lastMap = map;
#endif

			lastBasemap = null;

			// setup first time properties
			UpdateAuthenticationConfigurations();

			OnOriginPositionChanged();

			UpdateBasemap();

			UpdateElevation();

			UpdateExtent();

			UpdateLayers();
		}

		private void Update()
		{
			SyncPositionWithHPRoot();
		}

		private void LoadMap()
		{
			if (View?.Map?.LoadStatus == ArcGISLoadStatus.FailedToLoad)
			{
				View.Map.RetryLoad();
			}
			else if (View?.Map?.LoadStatus == ArcGISLoadStatus.NotLoaded)
			{
				View.Map.Load();
			}
		}

		internal void UpdateBasemap()
		{
			if (!View?.Map)
			{
				return;
			}

			if (lastBasemap == basemap && lastBasemapType == basemapType && lastBasemapAuthenticationType == basemapAuthenticationType)
			{
				return;
			}

			if (!string.IsNullOrEmpty(Basemap))
			{
				if (BasemapType == BasemapTypes.ImageLayer)
				{
					View.Map.Basemap = new ArcGISBasemap(Basemap, ArcGISLayerType.ArcGISImageLayer, GetEffectiveAPIKey());
				}
				else if (BasemapType == BasemapTypes.VectorTileLayer)
				{
					View.Map.Basemap = new ArcGISBasemap(Basemap, ArcGISLayerType.ArcGISVectorTileLayer, GetEffectiveAPIKey());
				}
				else
				{
					View.Map.Basemap = new ArcGISBasemap(Basemap, GetEffectiveAPIKey());
				}
			}
			else
			{
				View.Map.Basemap = new ArcGISBasemap();
			}

			lastBasemap = basemap;
			lastBasemapType = basemapType;
			lastBasemapAuthenticationType = basemapAuthenticationType;
		}

		private void UpdateBuildingAttributeFiltersFromDefinition(ArcGISBuildingSceneLayerFilterInstanceData buildingSceneLayerFilter, ArcGISBuildingSceneLayer buildingSceneLayer)
		{
			// Existing collection must be modified, not replaced.
			buildingSceneLayer.ActiveBuildingAttributeFilter = null;
			buildingSceneLayer.BuildingAttributeFilters.RemoveAll();

			ArcGISBuildingAttributeFilter activeFilter = null;

			// Make new API filters for each filter in the instance data.
			for (int i = 0; i < buildingSceneLayerFilter.BuildingAttributeFilters.Count; i++)
			{
				var buildingAttributeFilter = CreateArcGISBuildingAttributeFilterFromDefinition(buildingSceneLayerFilter.BuildingAttributeFilters[i]);

				buildingSceneLayer.BuildingAttributeFilters.Add(buildingAttributeFilter);

				if (i == buildingSceneLayerFilter.ActiveBuildingAttributeFilterIndex)
				{
					activeFilter = buildingAttributeFilter;
				}
			}

			// Set the active filter.
			if (activeFilter != null && buildingSceneLayerFilter.IsBuildingAttributeFilterEnabled)
			{
				// The active filter must be a member of the layers BuildingAttributeFilters collection.
				buildingSceneLayer.ActiveBuildingAttributeFilter = activeFilter;
			}
		}

		private void UpdateBuildingDisciplinesFromDefinition(ArcGISBuildingSceneLayerFilterInstanceData buildingSceneLayerFilter, ArcGISBuildingSceneLayer buildingSceneLayer)
		{
			if (!buildingSceneLayerFilter.IsBuildingDisciplinesCategoriesFilterEnabled)
			{
				return;
			}

			for (ulong i = 0; i < buildingSceneLayer.Sublayers.GetSize(); i++)
			{
				ApplyVisibilityToSubLayersRecursive(buildingSceneLayer.Sublayers.At(i), buildingSceneLayerFilter);
			}
		}

		private void ApplyVisibilityToSubLayersRecursive(ArcGISBuildingSceneSublayer sublayer, ArcGISBuildingSceneLayerFilterInstanceData filterInstanceData)
		{
			if (sublayer.Name.Equals("Overview"))
			{
				sublayer.IsVisible = false;
				return;
			}

			if (sublayer.Sublayers == null || sublayer.Sublayers.GetSize() == 0)
			{
				sublayer.IsVisible = filterInstanceData.EnabledCategories.Contains(sublayer.SublayerId) && filterInstanceData.EnabledDisciplines.Contains(sublayer.Discipline);
				return;
			}

			for (ulong i = 0; i < sublayer.Sublayers.GetSize(); i++)
			{
				sublayer.IsVisible = true;
				ApplyVisibilityToSubLayersRecursive(sublayer.Sublayers.At(i), filterInstanceData);
			}
		}

		internal void UpdateMeshModification()
		{
			var meshModifications = CreateArcGISMeshModificationCollectionFromDefinition(MapElevation.MeshModifications);

			View.Map.Elevation.MeshModifications = meshModifications;
		}

		private bool DefaultElevationExists()
		{
			foreach (var elevation in MapElevation.ElevationSources)
			{
				if (elevation.Source == DefaultElevationSourceData.Source)
				{
					return true;
				}
			}

			return false;
		}

		internal void UpdateElevation()
		{
			int invalidElevationSourceCounter = 0;

			if (!DefaultElevationExists())
			{
				MapElevation.ElevationSources.Insert(MapElevation.ElevationSources.Count, DefaultElevationSourceData);

				var newItem = MapElevation.ElevationSources[MapElevation.ElevationSources.Count - 1];
				newItem.APIObject = CreateArcGISElevationSourceFromDefinition(newItem, GetEffectiveAPIKey());
			}

			for (int i = 0; i < MapElevation.ElevationSources.Count; i++)
			{
				var elevationSource = MapElevation.ElevationSources[i];

				if (!CanCreateArcGISElevationSourceFromDefinition(elevationSource))
				{
					invalidElevationSourceCounter++;
					continue;
				}

				int apiIndex = -1;

				for (ulong j = 0; j < View.Map.Elevation.ElevationSources.GetSize(); j++)
				{
					var internalElevationSource = View.Map.Elevation.ElevationSources.At(j);

					// View.Map.Elevation.ElevationSources.Contains does a pointer comparison so we need to manually check the values
					// TODO: matt9678 update internalElevationSource.APIKey == APIKey to get the APIKey used for this elevation source instead of the one on the map component
					if (internalElevationSource.Source == elevationSource.Source && (int)internalElevationSource.ObjectType == (int)elevationSource.Type && internalElevationSource.APIKey == GetEffectiveAPIKey())
					{
						// Found the elevation source
						apiIndex = (int)j;

						// Name, IsVisible, Opacity can all just be updated if they differ
						if (internalElevationSource.Name != elevationSource.Name)
						{
							internalElevationSource.Name = elevationSource.Name;
						}

						if (internalElevationSource.IsEnabled != elevationSource.IsEnabled)
						{
							internalElevationSource.IsEnabled = elevationSource.IsEnabled;
						}

						break;
					}
				}

				// Didn't find the elevation source
				if (apiIndex == -1)
				{
					// Elevation source didn't exist yet, add it
					apiIndex = i - invalidElevationSourceCounter;
					elevationSource.APIObject = CreateArcGISElevationSourceFromDefinition(elevationSource, GetEffectiveAPIKey());
					View.Map.Elevation.ElevationSources.Insert((ulong)apiIndex, elevationSource.APIObject);
					continue;
				}

				// Calculate the expected new index
				var elevationSourcesIndex = i - invalidElevationSourceCounter;

				if ((int)apiIndex != elevationSourcesIndex && elevationSourcesIndex < (int)View.Map.Elevation.ElevationSources.GetSize())
				{
					// Elevation source isn't where we thought it would be, move it
					View.Map.Elevation.ElevationSources.Move((ulong)apiIndex, (ulong)elevationSourcesIndex);
				}
			}

			// More elevation sources in RTC than we expected, remove the extras
			for (ulong i = (ulong)(MapElevation.ElevationSources.Count - invalidElevationSourceCounter); i < View.Map.Elevation.ElevationSources.GetSize(); i++)
			{
				View.Map.Elevation.ElevationSources.Remove(i);
			}

			view.Map.Elevation.ExaggerationFactor = mapElevation.ExaggerationFactor;

			UpdateMeshModification();
		}

		internal void UpdateLayers()
		{
			if (!View?.Map)
			{
				return;
			}

			int invalidLayerCounter = 0;

			for (int i = 0; i < Layers.Count; i++)
			{
				var layer = Layers[i];

				if (!CanCreateArcGISLayerFromDefinition(layer))
				{
					invalidLayerCounter++;
					continue;
				}

				int apiIndex = -1;

				for (ulong j = 0; j < View.Map.Layers.GetSize(); j++)
				{
					var internalLayer = View.Map.Layers.At(j);

					// View.Map.Layers.Contains does a pointer comparison so we need to manually check the values
					// TODO: matt9678 update internalLayer.APIKey == APIKey to get the APIKey used for this layer instead of the one on the map component
					if (internalLayer.Source == layer.Source && (int)internalLayer.ObjectType == (int)layer.Type && internalLayer.APIKey == GetEffectiveAPIKey())
					{
						// Found the layer
						apiIndex = (int)j;

						// Name, IsVisible, Opacity can all just be updated if they differ
						if (internalLayer.Name != layer.Name)
						{
							internalLayer.Name = layer.Name;
						}

						if (internalLayer.IsVisible != layer.IsVisible)
						{
							internalLayer.IsVisible = layer.IsVisible;
						}

						if (internalLayer.Opacity != layer.Opacity)
						{
							internalLayer.Opacity = layer.Opacity;
						}

						if (internalLayer is ArcGIS3DObjectSceneLayer threeDObjectSceneLayer)
						{
							var spatialFeatureFilter = CreateArcGISSpatialFeatureFilterFromDefinition(layer.SpatialFeatureFilter);
							if (threeDObjectSceneLayer.FeatureFilter != spatialFeatureFilter)
							{
								threeDObjectSceneLayer.FeatureFilter = spatialFeatureFilter;
							}
						}
						else if (internalLayer is ArcGISBuildingSceneLayer buildingSceneLayer)
						{
							var spatialFeatureFilter = CreateArcGISSpatialFeatureFilterFromDefinition(layer.SpatialFeatureFilter);
							if (buildingSceneLayer.FeatureFilter != spatialFeatureFilter)
							{
								buildingSceneLayer.FeatureFilter = spatialFeatureFilter;
							}

							UpdateBuildingAttributeFiltersFromDefinition(layer.BuildingSceneLayerFilter, buildingSceneLayer);
							UpdateBuildingDisciplinesFromDefinition(layer.BuildingSceneLayerFilter, buildingSceneLayer);
						}
						else if (internalLayer is ArcGISIntegratedMeshLayer integratedMeshSceneLayer)
						{
							var meshModificationCollection = CreateArcGISMeshModificationCollectionFromDefinition(layer.MeshModifications);
							if (integratedMeshSceneLayer.MeshModifications != meshModificationCollection)
							{
								integratedMeshSceneLayer.MeshModifications = meshModificationCollection;
							}
						}
						else if (internalLayer is ArcGISOGC3DTilesLayer ogc3DTilesLayer)
						{
							var meshModificationCollection = CreateArcGISMeshModificationCollectionFromDefinition(layer.MeshModifications);
							if (ogc3DTilesLayer.MeshModifications != meshModificationCollection)
							{
								ogc3DTilesLayer.MeshModifications = meshModificationCollection;
							}
						}

						break;
					}
				}

				// Didn't find the layer
				if (apiIndex == -1)
				{
					// Layer didn't exist yet, add it
					apiIndex = i - invalidLayerCounter;
					layer.APIObject = CreateArcGISLayerFromDefinition(layer, GetEffectiveAPIKey());
					View.Map.Layers.Insert((ulong)apiIndex, layer.APIObject);
					continue;
				}

				// Calculate the expected new index
				var layersIndex = i - invalidLayerCounter;

				if ((int)apiIndex != layersIndex && layersIndex < (int)View.Map.Layers.GetSize())
				{
					// Layer isn't where we thought it would be, move it
					View.Map.Layers.Move((ulong)apiIndex, (ulong)layersIndex);
				}
			}

			// More layers in RTC than we expected, remove the extras
			for (ulong i = (ulong)(Layers.Count - invalidLayerCounter); i < View.Map.Layers.GetSize(); i++)
			{
				View.Map.Layers.Remove(i);
			}
		}

#if UNITY_EDITOR
		internal void UpdateEditorModeEnabled()
		{
			if (lastEditorModeEnabled == editorModeEnabled || Application.isPlaying)
			{
				return;
			}

			if (lastEditorModeEnabled.HasValue)
			{
				if (!editorModeEnabled)
				{
					lastMap = view.Map;
					view.Map = null;
					view = null;
				}
				else
				{
					View.Map = lastMap;
					lastMap = null;
				}

				ArcGISMainThreadScheduler.Instance().Schedule(() =>
				{
					EditorModeEnabledChanged?.Invoke();
				});
			}

			lastEditorModeEnabled = editorModeEnabled;
		}

		internal void UpdateDataFetchWithSceneView()
		{
			if (lastDataFetchWithSceneView == dataFetchWithSceneView || !editorCameraComponent || Application.isPlaying)
			{
				return;
			}

			editorCameraComponent.EditorViewEnabled = dataFetchWithSceneView;

			EnableMainCameraView(!dataFetchWithSceneView);

			lastDataFetchWithSceneView = dataFetchWithSceneView;
		}

		internal void UpdateRebaseWithSceneView()
		{
			if (lastRebaseWithSceneView == rebaseWithSceneView || !editorCameraComponent || Application.isPlaying)
			{
				return;
			}

			editorCameraComponent.WorldRepositionEnabled = rebaseWithSceneView;

			lastRebaseWithSceneView = rebaseWithSceneView;
		}
#endif

		internal void UpdateMeshCollidersEnabled()
		{
			if (lastMeshCollidersEnabled == meshCollidersEnabled)
			{
				return;
			}

			MeshCollidersEnabledChanged?.Invoke();

			lastMeshCollidersEnabled = meshCollidersEnabled;
		}

		public void UpdateHPRoot()
		{
			hpRoot.SetRootTR(universePosition, universeRotation);
		}

		internal ArcGISPolygon CreateArcGISPolygonFromDefinition(ArcGISPolygonInstanceData polygonInstanceData)
		{
			if (polygonInstanceData.Points.Count == 0)
			{
				return null;
			}

			var polygonBuilder = new ArcGISPolygonBuilder(polygonInstanceData.Points[0].SpatialReference);

			foreach (var point in polygonInstanceData.Points)
			{
				polygonBuilder.AddPoint(point);
			}

			return polygonBuilder.ToGeometry() as ArcGISPolygon;
		}

		internal ArcGISCollection<ArcGISMeshModification> CreateArcGISMeshModificationCollectionFromDefinition(ArcGISMeshModificationsInstanceData meshModificationsInstanceData)
		{
			var meshModificationCollection = new ArcGISCollection<ArcGISMeshModification>();

			if (!meshModificationsInstanceData || !meshModificationsInstanceData.IsEnabled)
			{
				return meshModificationCollection;
			}

			foreach (var meshModification in meshModificationsInstanceData.MeshModifications)
			{
				meshModificationCollection.Add(new ArcGISMeshModification(CreateArcGISPolygonFromDefinition(meshModification.Polygon), meshModification.Type));
			}

			return meshModificationCollection;
		}

		private ArcGISSolidBuildingFilterDefinition CreateArcGISSolidBuildingFilterDefinitionFromDefinition(ArcGISSolidBuildingFilterDefinitionInstanceData solidBuildingFilterDefinitionInstanceData)
		{
			if (!solidBuildingFilterDefinitionInstanceData)
			{
				return null;
			}

			return new ArcGISSolidBuildingFilterDefinition(solidBuildingFilterDefinitionInstanceData.Title, solidBuildingFilterDefinitionInstanceData.WhereClause);
		}

		internal ArcGISSpatialFeatureFilter CreateArcGISSpatialFeatureFilterFromDefinition(ArcGISSpatialFeatureFilterInstanceData spatialFeatureFilterInstanceData)
		{
			var polygonCollection = new ArcGISCollection<ArcGISPolygon>();

			if (!spatialFeatureFilterInstanceData.IsEnabled || spatialFeatureFilterInstanceData.Polygons.Count == 0)
			{
				return null;
			}

			foreach (var polygon in spatialFeatureFilterInstanceData.Polygons)
			{
				polygonCollection.Add(CreateArcGISPolygonFromDefinition(polygon));
			}

			return new ArcGISSpatialFeatureFilter(polygonCollection, spatialFeatureFilterInstanceData.Type);
		}

		internal bool CanCreateArcGISElevationSourceFromDefinition(ArcGISElevationSourceInstanceData elevationSourceDefinition)
		{
			return elevationSourceDefinition.Source != "" && elevationSourceDefinition.Type == ArcGISElevationSourceType.ArcGISImageElevationSource;
		}

		internal bool CanCreateArcGISLayerFromDefinition(ArcGISLayerInstanceData layerDefinition)
		{
			return layerDefinition.Source != "" &&
				(layerDefinition.Type == LayerTypes.ArcGIS3DObjectSceneLayer || layerDefinition.Type == LayerTypes.ArcGISImageLayer ||
				 layerDefinition.Type == LayerTypes.ArcGISIntegratedMeshLayer || layerDefinition.Type == LayerTypes.ArcGISVectorTileLayer ||
				 layerDefinition.Type == LayerTypes.ArcGISBuildingSceneLayer || layerDefinition.Type == LayerTypes.ArcGISGroupLayer ||
				 layerDefinition.Type == LayerTypes.ArcGISOGC3DTilesLayer);
		}

		private ArcGISBuildingAttributeFilter CreateArcGISBuildingAttributeFilterFromDefinition(ArcGISBuildingAttributeFilterInstanceData buildingAttributeFilterInstanceData)
		{
			if (!buildingAttributeFilterInstanceData)
			{
				return null;
			}

			var solidBuildingFilterDefinition = CreateArcGISSolidBuildingFilterDefinitionFromDefinition(buildingAttributeFilterInstanceData.SolidFilterDefinition);

			return new ArcGISBuildingAttributeFilter(buildingAttributeFilterInstanceData.Name, buildingAttributeFilterInstanceData.Description, solidBuildingFilterDefinition);
		}

		internal ArcGISElevationSource CreateArcGISElevationSourceFromDefinition(ArcGISElevationSourceInstanceData elevationSourceDefinition, string apiKey)
		{
			if (elevationSourceDefinition.Source == "")
			{
				return null;
			}

			string effectiveAPIKey = elevationSourceDefinition.AuthenticationType == ArcGISAuthenticationType.APIKey ? apiKey : "";

			ArcGISElevationSource elevationSource = null;

			if (elevationSourceDefinition.Type == ArcGISElevationSourceType.ArcGISImageElevationSource)
			{
				elevationSource = new GameEngine.Elevation.ArcGISImageElevationSource(elevationSourceDefinition.Source, elevationSourceDefinition.Name ?? "", effectiveAPIKey);
			}

			if (!elevationSource)
			{
				return null;
			}

			elevationSource.IsEnabled = elevationSourceDefinition.IsEnabled;

			return elevationSource;
		}

		internal ArcGISLayer CreateArcGISLayerFromDefinition(ArcGISLayerInstanceData layerDefinition, string apiKey)
		{
			if (layerDefinition.Source == "")
			{
				return null;
			}

			string effectiveAPIKey = layerDefinition.AuthenticationType == ArcGISAuthenticationType.APIKey ? apiKey : "";

			ArcGISLayer layer = null;

			var opacity = Mathf.Max(Mathf.Min(layerDefinition.Opacity, 1.0f), 0.0f);

			if (layerDefinition.Type == LayerTypes.ArcGIS3DObjectSceneLayer)
			{
				layer = new ArcGIS3DObjectSceneLayer(layerDefinition.Source, layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);
			}
			else if (layerDefinition.Type == LayerTypes.ArcGISImageLayer)
			{
				layer = new ArcGISImageLayer(layerDefinition.Source, layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);
			}
			else if (layerDefinition.Type == LayerTypes.ArcGISIntegratedMeshLayer)
			{
				layer = new ArcGISIntegratedMeshLayer(layerDefinition.Source, layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);
			}
			else if (layerDefinition.Type == LayerTypes.ArcGISVectorTileLayer)
			{
				layer = new ArcGISVectorTileLayer(layerDefinition.Source, layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);
			}
			else if (layerDefinition.Type == LayerTypes.ArcGISBuildingSceneLayer)
			{
				layer = new ArcGISBuildingSceneLayer(layerDefinition.Source, layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);

				layer.DoneLoading += delegate (Exception loadError)
				{
					if (loadError == null)
					{
						UpdateBuildingAttributeFiltersFromDefinition(layerDefinition.BuildingSceneLayerFilter, (ArcGISBuildingSceneLayer)layer);
						UpdateBuildingDisciplinesFromDefinition(layerDefinition.BuildingSceneLayerFilter, (ArcGISBuildingSceneLayer)layer);
					}
				};
			}
			else if (layerDefinition.Type == LayerTypes.ArcGISGroupLayer)
			{
				layer = new ArcGISGroupLayer(layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);
			}
			else if (layerDefinition.Type == LayerTypes.ArcGISOGC3DTilesLayer)
			{
				layer = new ArcGISOGC3DTilesLayer(layerDefinition.Source, layerDefinition.Name, opacity, layerDefinition.IsVisible, effectiveAPIKey);
			}

			if (!layer)
			{
				return null;
			}

			if (layer is ArcGIS3DObjectSceneLayer threeDObjectSceneLayer)
			{
				var featureFilter = CreateArcGISSpatialFeatureFilterFromDefinition(layerDefinition.SpatialFeatureFilter);

				threeDObjectSceneLayer.FeatureFilter = featureFilter;
			}
			else if (layer is ArcGISBuildingSceneLayer buildingSceneLayer)
			{
				var featureFilter = CreateArcGISSpatialFeatureFilterFromDefinition(layerDefinition.SpatialFeatureFilter);

				buildingSceneLayer.FeatureFilter = featureFilter;
			}
			else if (layer is ArcGISIntegratedMeshLayer integratedMeshLayer)
			{
				var meshModifications = CreateArcGISMeshModificationCollectionFromDefinition(layerDefinition.MeshModifications);

				integratedMeshLayer.MeshModifications = meshModifications;
			}

			return layer;
		}

		private void UpdateAuthenticationConfigurations()
		{
			ArcGISAuthenticationManager.OAuthUserConfigurations?.Clear();

			foreach (var config in oauthUserConfigurations)
			{
				config.clientId = config.ClientId.Trim();
				config.redirectURL = config.RedirectURL.Trim();
				config.portalURL = config.PortalURL.Trim();

				ArcGISAuthenticationManager.OAuthUserConfigurations?.Add(config);
			}
		}

		internal void UpdateExtent()
		{
			if (!View.Map)
			{
				return;
			}

			if (MapType == ArcGISMapType.Local)
			{
				var extent = this.Extent;

				if (enableExtent && Extent.UseOriginAsCenter)
				{
					extent.GeographicCenter = originPosition;
				}

				if (enableExtent && IsExtentDefinitionValid(extent))
				{
					View.Map.ClippingArea = CreateArcGISExtentFromDefinition(extent);
				}
				else
				{
					View.Map.ClippingArea = null;
				}
			}
		}

		internal void UpdateCustomMapSpatialReference()
		{
			InitializeArcGISMap();
		}

		public void EnableMainCameraView(bool enable)
		{
			var cameraComponentsInThisView = GetComponentsInChildren<ArcGISCameraComponent>(true);

			ArcGISCameraComponent mainMapCameraComponent = null;

			foreach (var cameraComponent in cameraComponentsInThisView)
			{
				if (!cameraComponent.GetComponent<ArcGISEditorCameraComponent>())
				{
					mainMapCameraComponent = cameraComponent;
					break;
				}
			}

			if (mainMapCameraComponent)
			{
				mainMapCameraComponent.enabled = enable;

				if (mainMapCameraComponent.gameObject.GetComponent<ArcGISRebaseComponent>())
				{
					mainMapCameraComponent.gameObject.GetComponent<ArcGISRebaseComponent>().enabled = enable;
				}
			}
		}

		private ArcGISExtent CreateArcGISExtentFromDefinition(ArcGISExtentInstanceData Extent)
		{
			var center = new ArcGISPoint(Extent.GeographicCenter.X, Extent.GeographicCenter.Y, Extent.GeographicCenter.Z, Extent.GeographicCenter.SpatialReference);

			ArcGISExtent extent;

			switch (Extent.ExtentShape)
			{
				case MapExtentShapes.Rectangle:
					extent = new ArcGISExtentRectangle(center, Extent.ShapeDimensions.x, Extent.ShapeDimensions.y);
					break;

				case MapExtentShapes.Square:
					extent = new ArcGISExtentRectangle(center, Extent.ShapeDimensions.x, Extent.ShapeDimensions.x);
					break;

				case MapExtentShapes.Circle:
					extent = new ArcGISExtentCircle(center, Extent.ShapeDimensions.x);
					break;

				default:
					extent = new ArcGISExtentRectangle(center, Extent.ShapeDimensions.x, Extent.ShapeDimensions.y);
					break;
			}

			return extent;
		}

		private bool IsExtentDefinitionValid(ArcGISExtentInstanceData Extent)
		{
			if (Extent.ShapeDimensions.x < 0)
			{
				Extent.ShapeDimensions.x = 0;
			}

			if (Extent.ShapeDimensions.y < 0)
			{
				Extent.ShapeDimensions.y = 0;
			}

			return Extent.ShapeDimensions.x > 0 && (Extent.ExtentShape != MapExtentShapes.Rectangle || Extent.ShapeDimensions.y > 0);
		}

		public void NotifyLowMemoryWarning()
		{
			if (view != null)
			{
				view.HandleLowMemoryWarning();
			}
		}

		public enum Version
		{
			// Before any version changes were made
			BeforeCustomVersionWasAdded = 0,

			// Move from a single elevation source to multiple
			ElevationSources_1_2 = 1,

			// Move all elevation properties to a single struct
			Elevation_1_4 = 2,

			// Move to new auth system
			Authentication_2_0 = 3,

			// -----<new versions can be added above this line>-------------------------------------------------
			VersionPlusOne,

			LatestVersion = VersionPlusOne - 1
		}

		[SerializeField]
		Version version = Version.BeforeCustomVersionWasAdded;

		public void OnAfterDeserialize()
		{
			if (version < Version.Elevation_1_4)
			{
				Debug.LogWarning("ArcGISMapComponent has been saved in a version older than 1.7. In order to avoid data loss, " +
					"please open and save your level with 1.7 before opening it with a version newer than 1.7.");
			}

			if (version < Version.Authentication_2_0 && configurations.Count > 0)
			{
				var configurationPortalUrls = new Dictionary<int, HashSet<string>>();

				for (int i = 0; i < configurations.Count; i++)
				{
					configurationPortalUrls[i] = new HashSet<string>();
				}

#pragma warning disable 612, 618
				if (basemapAuthentication != null && configurationPortalUrls.ContainsKey(basemapAuthentication.ConfigurationIndex))
				{
					configurationPortalUrls[basemapAuthentication.ConfigurationIndex].Add(GetPortalURLFromServiceURL(basemap));
					basemapAuthenticationType = ArcGISAuthenticationType.UserAuthentication;
				}

				foreach (var elevationSource in mapElevation.ElevationSources)
				{
					if (elevationSource.Authentication != null && configurationPortalUrls.ContainsKey(elevationSource.Authentication.ConfigurationIndex))
					{
						configurationPortalUrls[elevationSource.Authentication.ConfigurationIndex].Add(GetPortalURLFromServiceURL(elevationSource.Source));
						elevationSource.AuthenticationType = ArcGISAuthenticationType.UserAuthentication;
					}
				}

				foreach (var layer in layers)
				{
					if (layer.Authentication != null && configurationPortalUrls.ContainsKey(layer.Authentication.ConfigurationIndex))
					{
						configurationPortalUrls[layer.Authentication.ConfigurationIndex].Add(GetPortalURLFromServiceURL(layer.Source));
						layer.AuthenticationType = ArcGISAuthenticationType.UserAuthentication;
					}
				}
#pragma warning restore 612, 618

				for (int configIndex = 0; configIndex < configurations.Count; configIndex++)
				{
					if (configurationPortalUrls[configIndex].Count == 0)
					{
						configurationPortalUrls[configIndex].Add(string.Empty);
					}

					foreach (var portalUrl in configurationPortalUrls[configIndex])
					{
						// Create the new config.
						oauthUserConfigurations.Add(new ArcGISOAuthUserConfiguration(portalUrl,
																	configurations[configIndex].ClientID,
																	configurations[configIndex].RedirectURI,
																	string.Empty,
																	0,
																	0,
																	0,
																	true,
																	ArcGISUserInterfaceStyle.Unspecified,
																	false)
						{
							name = configurations[configIndex].Name,
						});
					}
				}

				UpdateAuthenticationConfigurations();

				Debug.LogWarning("ArcGIS Maps SDK: This scene was saved in a version older than 2.0. " +
					"Authentication configurations have been migrated to the new format. " +
					"Please verify that authentication configurations have correct values.");
			}

			version = Version.LatestVersion;
		}

		private string GetPortalURLFromServiceURL(string serviceUrl)
		{
			var newPortalUrl = string.Empty;
			var serverContext = ArcGISURLUtils.GetServerContext(serviceUrl);

			if (serverContext.Contains("arcgis.com"))
			{
				newPortalUrl = "https://www.arcgis.com";
			}
			else if (serverContext.Split("://").Length > 1)
			{
				var afterProtocol = serverContext.Split("://")[1];

				if (afterProtocol.Split('/').Length > 1)
				{
					var afterDomain = afterProtocol.Split('/')[1];
					var trimIndex = serverContext.IndexOf(afterDomain);
					newPortalUrl = $"{serverContext[0..trimIndex]}portal";
				}
			}

			return newPortalUrl;
		}

		public void OnBeforeSerialize()
		{
			version = Version.LatestVersion;
		}

		internal void OnMapTypeChanged()
		{
			InitializeArcGISMap();

			if (MapTypeChanged != null)
			{
				MapTypeChanged();
			}
		}

		internal void OnOriginPositionChanged()
		{
			internalHasChanged = true;
			SyncPositionWithHPRoot();

			if (MapType == ArcGISMapType.Local && EnableExtent && Extent.UseOriginAsCenter)
			{
				UpdateExtent();
			}
		}

		private void OnValidate()
		{
			UpdateBasemap();
			UpdateLayers();
#if UNITY_EDITOR
			UpdateEditorModeEnabled();
			UpdateDataFetchWithSceneView();
			UpdateRebaseWithSceneView();
#endif
			UpdateMeshCollidersEnabled();
			LoadMap();
		}

		internal void OnUniversePositionChanged()
		{
			var tangentToWorld = View.GetENUReference(universePosition).ToQuaterniond();

			universeRotation = tangentToWorld.ToQuaternion();

			UpdateHPRoot();
			SyncPositionWithHPRoot();

			RootChanged.Invoke();
		}

		private void PullChangesFromHPRoot()
		{
			universePosition = hpRoot.RootUniversePosition;
			universeRotation = hpRoot.RootUniverseRotation;

			originPosition = View.WorldToGeographic(universePosition);   // May result in NaN position
		}

		private void PushChangesToHPRoot()
		{
			var cartesianPosition = View.GeographicToWorld(originPosition);

			if (!cartesianPosition.IsValid())
			{
				// If the geographic position is not a valid cartesian position, ignore it
				PullChangesFromHPRoot(); // Reset position from current, assumed value, cartesian position

				return;
			}

			UniversePosition = cartesianPosition;
		}

		public void SetBasemapSourceAndType(string source, BasemapTypes type)
		{
			if (basemap == source && basemapType == type)
			{
				return;
			}

			basemap = source;
			basemapType = type;

			UpdateBasemap();

			// If the map either failed to load or was unloaded,
			// updating the layers could be enough for the map to succeed at loading so we will retry
			LoadMap();
		}

		public void SetMemoryQuotas(MemoryQuotas memoryQuotas)
		{
			if (view != null)
			{
				view.SetMemoryQuotas(memoryQuotas.SystemMemory.GetValueOrDefault(-1L), memoryQuotas.VideoMemory.GetValueOrDefault(-1L));
			}
		}

		public bool ShouldEditorComponentBeUpdated()
		{
#if UNITY_EDITOR
			return Application.isPlaying || (editorModeEnabled && Application.isEditor);
#else
			return true;
#endif
		}

		internal void SyncPositionWithHPRoot()
		{
			if (!View.SpatialReference)
			{
				// Defer until we have a spatial reference
				return;
			}

			if (internalHasChanged && originPosition.IsValid)
			{
				PushChangesToHPRoot();
			}
			else if (!originPosition.IsValid || !universePosition.Equals(hpRoot.RootUniversePosition) || universeRotation != hpRoot.RootUniverseRotation)
			{
				PullChangesFromHPRoot();
			}

			internalHasChanged = false;
		}

		public void CheckNumArcGISCameraComponentsEnabled()
		{
			var cameraComponents = GetComponentsInChildren<ArcGISCameraComponent>(false);

			int numEnabled = 0;

			foreach (var component in cameraComponents)
			{
				numEnabled += component.enabled ? 1 : 0;
			}

			if (numEnabled > 1)
			{
				Debug.LogWarning("Multiple ArcGISCameraComponents enabled at the same time!");
			}
		}

		public async Task<bool> ZoomToElevationSource(GameObject gameObject, ArcGISElevationSourceInstanceData elevationSourceInstanceData)
		{
			ArcGISElevationSource apiObject = null;

			foreach (var elevationSource in MapElevation.ElevationSources)
			{
				if (elevationSource == elevationSourceInstanceData)
				{
					apiObject = elevationSource.APIObject;

					break;
				}
			}

			if (!apiObject)
			{
				return false;
			}

			return await ZoomToElevationSource(gameObject, apiObject);
		}

		public async Task<bool> ZoomToElevationSource(GameObject gameObject, ArcGISElevationSource elevationSource)
		{
			if (!elevationSource)
			{
				Debug.LogWarning("Invalid elevation source passed to zoom to");
				return false;
			}

			if (elevationSource.LoadStatus != GameEngine.ArcGISLoadStatus.Loaded)
			{
				if (elevationSource.LoadStatus == GameEngine.ArcGISLoadStatus.NotLoaded)
				{
					elevationSource.Load();
				}
				else if (elevationSource.LoadStatus != GameEngine.ArcGISLoadStatus.FailedToLoad)
				{
					elevationSource.RetryLoad();
				}

				await Task.Run(() =>
				{
					while (elevationSource.LoadStatus == GameEngine.ArcGISLoadStatus.Loading)
					{
					}
				});

				if (elevationSource.LoadStatus == GameEngine.ArcGISLoadStatus.FailedToLoad)
				{
					Debug.LogWarning("Layer passed to zoom to layer must be loaded");
					return false;
				}
			}

			var layerExtent = elevationSource.Extent;

			if (!layerExtent)
			{
				Debug.LogWarning("The layer passed to zoom to layer does not have a valid extent");
				return false;
			}

			return ZoomToExtent(gameObject, layerExtent);
		}

		// Position a gameObject to look at an extent
		// if there is no Camera component to get the fov from just default it to 90 degrees
		public bool ZoomToExtent(GameObject gameObject, ArcGISExtent extent)
		{
			var spatialReference = View.SpatialReference;

			if (!spatialReference)
			{
				Debug.LogWarning("View must have a spatial reference to run zoom to layer");
				return false;
			}

			if (!extent)
			{
				Debug.LogWarning("Extent cannot be null");
				return false;
			}

			var cameraPosition = extent.Center;
			double largeSide;
			if (ArcGISExtentType.ArcGISExtentRectangle == extent.ObjectType)
			{
				var rectangleExtent = extent as ArcGISExtentRectangle;
				largeSide = Math.Max(rectangleExtent.Width, rectangleExtent.Height);
			}
			else if (ArcGISExtentType.ArcGISExtentCircle == extent.ObjectType)
			{
				var rectangleExtent = extent as ArcGISExtentCircle;
				largeSide = rectangleExtent.Radius * 2;
			}
			else
			{
				Debug.LogWarning(extent.ObjectType.ToString() + "extent type is not supported");
				return false;
			}

			// Accounts for an internal error where the dimmension was being divided instead of multiplied
			if (largeSide < 0.01)
			{
				double earthCircumference = 40e6;
				double meterPerEquaterDegree = earthCircumference / 360;
				largeSide *= meterPerEquaterDegree * meterPerEquaterDegree;
			}

			// In global mode we can't see the entire layer if it is on a global scale,
			// so we just need to see the diameter of the planet
			if (MapType == ArcGISMapType.Global)
			{
				var globeRadius = spatialReference.SpheroidData.MajorSemiAxis;
				largeSide = Math.Min(largeSide, 2 * globeRadius);
			}

			var cameraComponent = gameObject.GetComponent<Camera>();

			double radFOVAngle = Mathf.PI / 2; // 90 degrees
			if (cameraComponent)
			{
				radFOVAngle = cameraComponent.fieldOfView * Utils.Math.MathUtils.DegreesToRadians;
			}

			var radHFOV = Math.Atan(Math.Tan(radFOVAngle / 2));
			var zOffset = 0.5 * largeSide / Math.Tan(radHFOV);

			var newPosition = new ArcGISPoint(cameraPosition.X,
											  cameraPosition.Y,
											  cameraPosition.Z + zOffset,
											  cameraPosition.SpatialReference);
			var newRotation = new ArcGISRotation(0, 0, 0);

			ArcGISLocationComponent.SetPositionAndRotation(gameObject, newPosition, newRotation);

			return true;
		}

		// Position a gameObject to look at a layer
		// if there is no Camera component to get the fov from just default it to 90 degrees
		public async Task<bool> ZoomToLayer(GameObject gameObject, Esri.GameEngine.Layers.Base.ArcGISLayer layer)
		{
			if (!layer)
			{
				Debug.LogWarning("Invalid layer passed to zoom to layer");
				return false;
			}

			if (layer.LoadStatus != GameEngine.ArcGISLoadStatus.Loaded)
			{
				if (layer.LoadStatus == GameEngine.ArcGISLoadStatus.NotLoaded)
				{
					layer.Load();
				}
				else if (layer.LoadStatus != GameEngine.ArcGISLoadStatus.FailedToLoad)
				{
					layer.RetryLoad();
				}

				await Task.Run(() =>
				{
					while (layer.LoadStatus == GameEngine.ArcGISLoadStatus.Loading)
					{
					}
				});

				if (layer.LoadStatus == GameEngine.ArcGISLoadStatus.FailedToLoad)
				{
					Debug.LogWarning("Layer passed to zoom to layer must be loaded");
					return false;
				}
			}

			var layerExtent = layer.Extent;

			if (!layerExtent)
			{
				Debug.LogWarning("The layer passed to zoom to layer does not have a valid extent");
				return false;
			}

			return ZoomToExtent(gameObject, layerExtent);
		}

		public Physics.ArcGISRaycastHit GetArcGISRaycastHit(RaycastHit raycastHit)
		{
			Physics.ArcGISRaycastHit output;
			output.featureId = -1;
			output.featureIndex = -1;
			output.layer = null;

			var rendererComponent = rendererComponentGameObject.GetComponent<ArcGISRendererComponent>();

			if (raycastHit.collider != null && rendererComponent)
			{
				var renderable = rendererComponent.GetRenderableByGameObject(raycastHit.collider.gameObject);

				if (renderable != null)
				{
					output.featureIndex = Physics.RaycastHelpers.GetFeatureIndexByTriangleIndex(raycastHit.collider.gameObject, raycastHit.triangleIndex);
					output.layer = View.Map?.FindLayerById(renderable.LayerId);

					if (renderable.Material.NativeMaterial.HasTexture("_FeatureIds"))
					{
						// gets the feature ID
						var featureIds = (Texture2D)renderable.Material.NativeMaterial.GetTexture("_FeatureIds");

						var width = featureIds.width;
						int y = (int)Mathf.Floor(output.featureIndex / width);
						int x = output.featureIndex - y * width;

						var color = featureIds.GetPixel(x, y);
						var scaledColor = new Vector4(255f * color.r, 255f * color.g, 255f * color.b, 255f * color.a);
						var shift = new Vector4(1, 0x100, 0x10000, 0x1000000);
						scaledColor.Scale(shift);

						output.featureId = (int)(scaledColor.x + scaledColor.y + scaledColor.z + scaledColor.w);
					}
				}
			}

			return output;
		}

		public void HandleOnForecCollisionInEditoWorldChanged()
		{
			MeshCollidersEnabledChanged();
		}

		public ArcGISPoint EngineToGeographic(Vector3 position)
		{
			if (!HasSpatialReference())
			{
				Debug.LogWarning("Default Position. No Spatial Reference.");
				return new ArcGISPoint(0, 0, 0);
			}

			var worldPosition = math.inverse(WorldMatrix).HomogeneousTransformPoint(position.ToDouble3());
			return View.WorldToGeographic(worldPosition);
		}

		public Vector3 GeographicToEngine(ArcGISPoint position)
		{
			if (!HasSpatialReference())
			{
				Debug.LogWarning("Default Position. No Spatial Reference.");
				return new Vector3();
			}

			var worldPosition = View.GeographicToWorld(position);
			return WorldMatrix.HomogeneousTransformPoint(worldPosition).ToVector3();
		}

		internal string GetEffectiveAPIKey()
		{
			var result = APIKey ?? "";

			if (result == "")
			{
				result = ArcGISProjectSettingsAsset.Instance?.APIKey ?? "";
			}

			return result;
		}
	}
}
