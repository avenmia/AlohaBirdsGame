// COPYRIGHT 1995-2022 ESRI
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
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Elevation.Base;
using Esri.GameEngine.Geometry;
using Esri.GameEngine.Map;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorMapTool : ArcGISMapCreatorTool
	{
		private ArcGISMapComponent mapComponent;

		private Toggle extentToggle;
		private Toggle manualSelection;

		private Button globalButton;
		private Button localButton;
		private Button createMapButton;

		private int manualSRField;
		private int manualVCField;
		private TextField manualSRTextField;
		private TextField manualVCTextField;

		private DoubleField originXPositionField;
		private DoubleField originYPositionField;
		private DoubleField originZPositionField;
		private IntegerField originSRPositionField;
		private IntegerField originVCPositionField;

		private DoubleField extentXPositionField;
		private DoubleField extentYPositionField;
		private IntegerField extentSRPositionField;
		private IntegerField extentVCPositionField;
		private DoubleField extentShapeXField;
		private DoubleField extentShapeYField;
		private EnumField shapeField;
		private Foldout mapExtentFoldout;
		private VisualElement extentFields;
		private VisualElement rootElement;
		private VisualElement spatialReferenceFields;

		public override VisualElement GetContent()
		{
			return rootElement;
		}

		public override void OnEnable()
		{
			rootElement = new VisualElement();
			rootElement.name = "ArcGISMapCreatorMapTool";

			var template = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/MapToolTemplate.uxml");
			template.CloneTree(rootElement);

			rootElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/MapToolStyle.uss"));

			createMapButton = rootElement.Query<Button>(name: "button-create-map");
			createMapButton.RegisterCallback<MouseDownEvent>(evnt => CreateMap(evnt, rootElement));
		}

		public override void OnDeselected()
		{
			if (mapComponent)
			{
				mapComponent.MapTypeChanged -= ToggleMapTypeUI;
			}

			rootElement?.Unbind();
		}

		public override void OnDestroy()
		{
			OnDeselected();
		}

		public override void OnSelected()
		{
			OnSelected(MapCreatorUtilities.MapComponent);
		}

		internal void OnSelected(IArcGISMapComponentInterface mapInterface)
		{
			if (rootElement == null)
			{
				return;
			}

			mapComponent = mapInterface as ArcGISMapComponent;

			InitSpatialReferenceCategory(rootElement);
			InitMapExtentCategory(rootElement);
			PopulateExtentFields(rootElement);
			InitMapTypeToggle(rootElement);
			InitOriginCategory(rootElement);
			InitCreateMapButton(rootElement);

			if (!mapComponent)
			{
				rootElement.Unbind();
				return;
			}

			mapComponent.MapTypeChanged += ToggleMapTypeUI;
			var serializedObject = new SerializedObject(mapComponent);
			rootElement.Bind(serializedObject);

		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage($"MapCreator/Toolbar/{MapCreatorUtilities.Assets.GetThemeFolderName()}/MapToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Map";
		}

		private void InitMapTypeToggle(VisualElement rootElement)
		{
			globalButton = rootElement.Query<Button>(name: "select-global-button");
			globalButton.clickable.activators.Clear();
			globalButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				ToggleMapTypeButtons(true);
				mapExtentFoldout.style.display = DisplayStyle.None;

				if (mapComponent)
				{
					mapComponent.MapType = ArcGISMapType.Global;
				}
			});

			localButton = rootElement.Query<Button>(name: "select-local-button");
			localButton.clickable.activators.Clear();
			localButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				ToggleMapTypeButtons(false);
				mapExtentFoldout.style.display = DisplayStyle.Flex;
				if (mapComponent)
				{
					mapComponent.MapType = ArcGISMapType.Local;
					SwitchVisibleExtentShapeFields(mapComponent.Extent.ExtentShape);
				}
			});

			ToggleMapTypeUI();
		}

		private void ToggleMapTypeUI()
		{
			bool isGlobal = mapComponent?.MapType == ArcGISMapType.Global;
			ToggleMapTypeButtons(isGlobal);
			mapExtentFoldout.style.display = isGlobal ? DisplayStyle.None : DisplayStyle.Flex;
		}

		private void ToggleMapTypeButtons(bool isGlobal)
		{
			globalButton.SetEnabled(!isGlobal);
			localButton.SetEnabled(isGlobal);
		}

		private void InitOriginCategory(VisualElement rootElement)
		{
			Action<int> VCOriginChanged = (int value) =>
			{
				mapComponent?.OnOriginPositionChanged();
			};

			Action<int> HCOriginChanged = (int value) =>
			{
				mapComponent?.OnOriginPositionChanged();

				if (value == SpatialReferenceWkid.WGS84 || value == SpatialReferenceWkid.CGCS2000)
				{
					originXPositionField.label = "Longitude";
					originYPositionField.label = "Latitude";
					originZPositionField.label = "Altitude";
				}

				else
				{
					originXPositionField.label = "X";
					originYPositionField.label = "Y";
					originZPositionField.label = "Z";
				}
			};

			Action<double> XOriginChanged = (double value) =>
			{
				if (!mapComponent)
				{
					return;
				}

				mapComponent.OriginPosition = new ArcGISPoint(value, mapComponent.OriginPosition.Y, mapComponent.OriginPosition.Z, mapComponent.OriginPosition.SpatialReference);
				mapComponent.OnOriginPositionChanged();
			};

			Action<double> YOriginChanged = (double value) =>
			{
				if (!mapComponent)
				{
					return;
				}

				mapComponent.OriginPosition = new ArcGISPoint(mapComponent.OriginPosition.X, value, mapComponent.OriginPosition.Z, mapComponent.OriginPosition.SpatialReference);
				mapComponent.OnOriginPositionChanged();
			};

			Action<double> ZOriginChanged = (double value) =>
			{
				if (!mapComponent)
				{
					return;
				}

				mapComponent.OriginPosition = new ArcGISPoint(mapComponent.OriginPosition.X, mapComponent.OriginPosition.Y, value, mapComponent.OriginPosition.SpatialReference);
				mapComponent.OnOriginPositionChanged();
			};

			originXPositionField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-origin-x", null, XOriginChanged);
			originYPositionField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-origin-y", null, YOriginChanged);
			originZPositionField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-origin-z", null, ZOriginChanged);
			originSRPositionField = MapCreatorUtilities.InitializeIntegerField(rootElement, "map-origin-spatial-reference-h", null, HCOriginChanged);
			originVCPositionField = MapCreatorUtilities.InitializeIntegerField(rootElement, "map-origin-spatial-reference-v", null, VCOriginChanged);

			if (!mapComponent)
			{
				originXPositionField.value = 0;
				originYPositionField.value = 0;
				originZPositionField.value = 0;
				originSRPositionField.value = SpatialReferenceWkid.WGS84;
				originVCPositionField.value = 0;
			}

			MapCreatorUtilities.UpdateArcGISPointLabels(originXPositionField, originYPositionField, originZPositionField, originSRPositionField.value);
		}

		private void InitSpatialReferenceCategory(VisualElement rootElement)
		{
			spatialReferenceFields = rootElement.Query<VisualElement>(name: "map-coordinate-system-buttons");
			manualSelection = rootElement.Query<Toggle>(name: "toggle-enable-manual-selection");
			manualSRTextField = rootElement.Query<TextField>(name: "map-cs-horitzontal");
			manualVCTextField = rootElement.Query<TextField>(name: "map-cs-vertical");

			spatialReferenceFields.SetEnabled(false);
			manualSelection.value = false;
			manualSRField = SpatialReferenceWkid.WebMercator;
			manualVCField = 0;

			manualSelection.RegisterValueChangedCallback(evnt =>
			{
				if (evnt.previousValue == evnt.newValue)
				{
					return;
				}

				spatialReferenceFields.SetEnabled(evnt.newValue);

				mapComponent?.UpdateCustomMapSpatialReference();
			});

			manualSRTextField.RegisterValueChangedCallback(evnt =>
			{
				var previousManualSRField = manualSRField;

				if (int.TryParse(evnt.newValue, out manualSRField))
				{
					manualSRFieldToText();

					if (!mapComponent)
					{
						return;
					}

					try
					{
						mapComponent.CustomMapSpatialReference = new ArcGISSpatialReference(manualSRField, mapComponent.CustomMapSpatialReference.VerticalWKID);
					}
					catch
					{
					}
				}
				else
				{
					manualSRField = previousManualSRField;
				}
			});

			manualVCTextField.RegisterValueChangedCallback(evnt =>
			{
				var previousManualVCField = manualVCField;

				if (int.TryParse(evnt.newValue, out manualVCField))
				{
					manualVCToText();

					if (!mapComponent)
					{
						return;
					}

					try
					{
						mapComponent.CustomMapSpatialReference = new ArcGISSpatialReference(mapComponent.CustomMapSpatialReference.WKID, manualVCField);
					}
					catch
					{
					}
				}
				else
				{
					manualVCField = previousManualVCField;
				}
			});

			if (mapComponent)
			{
				spatialReferenceFields.SetEnabled(mapComponent.UseCustomMapSpatialReference);
				manualSelection.value = mapComponent.UseCustomMapSpatialReference;

				if (mapComponent.CustomMapSpatialReference)
				{
					manualSRTextField.value = mapComponent.CustomMapSpatialReference.WKID.ToString();
					manualVCTextField.value = mapComponent.CustomMapSpatialReference.VerticalWKID.ToString();
				}
			}

			manualSRFieldToText();
			manualVCToText();
		}

		private void manualSRFieldToText()
		{
			if (manualSRField == SpatialReferenceWkid.WGS84)
			{
				manualSRTextField.SetValueWithoutNotify("WGS 84 (4326)");
			}
			else
			{
				if (manualSRField == SpatialReferenceWkid.WebMercator)
				{
					manualSRTextField.SetValueWithoutNotify("Web Mercator (3857)");
				}
				else
				{
					manualSRTextField.SetValueWithoutNotify(manualSRField.ToString());
				}
			}
		}

		private void manualVCToText()
		{
			if (manualVCField == 0)
			{
				manualVCTextField.SetValueWithoutNotify("<None>");
			}
			else
			{
				if (manualVCField == SpatialReferenceVerticalWkid.EGM1996)
				{
					manualVCTextField.SetValueWithoutNotify("EGM 96 (5773)");
				}
				else
				{
					if (manualVCField == SpatialReferenceVerticalWkid.WGS84)
					{
						manualVCTextField.SetValueWithoutNotify("WGS 84 (115700)");
					}
					else
					{
						manualVCTextField.SetValueWithoutNotify(manualVCField.ToString());
					}
				}
			}
		}

		private void InitMapExtentCategory(VisualElement rootElement)
		{
			mapExtentFoldout = rootElement.Query<Foldout>(name: "category-map-extent");
			extentFields = rootElement.Query<VisualElement>(name: "map-extent-fields");

			var useOriginCenterToggle = (Toggle)rootElement.Query<Toggle>(name: "toggle-origin-center-extent");

			extentToggle = rootElement.Query<Toggle>(name: "toggle-enable-map-extent");
			extentToggle.RegisterValueChangedCallback(evnt =>
			{
				extentFields.SetEnabled(evnt.newValue);
				useOriginCenterToggle.SetEnabled(evnt.newValue);

				mapComponent?.UpdateExtent();
			});

			var gcFields = (VisualElement)mapExtentFoldout.Query<VisualElement>(name: "geographic-center-fields");
			useOriginCenterToggle.RegisterValueChangedCallback(evnt =>
			{
				if (mapComponent)
				{
					mapComponent.UpdateExtent();
				}
				else
				{
					extentXPositionField.value = 0;
					extentYPositionField.value = 0;
					extentSRPositionField.value = SpatialReferenceWkid.WGS84;
					extentVCPositionField.value = 0;
				}

				gcFields.style.display = evnt.newValue ? DisplayStyle.None : DisplayStyle.Flex;
			});

			shapeField = rootElement.Query<EnumField>(name: "map-shape-selector");
			shapeField.RegisterValueChangedCallback(evnt =>
			{
				if (evnt.newValue != null)
				{
					var shape = (MapExtentShapes)evnt.newValue;
					SwitchVisibleExtentShapeFields(shape);
					mapComponent?.UpdateExtent();
				}
			});

			if (!mapComponent || mapComponent.MapType == ArcGISMapType.Global)
			{
				mapExtentFoldout.style.display = DisplayStyle.None;

				extentToggle.value = false;
				extentFields.SetEnabled(extentToggle.value);
				shapeField.value = MapExtentShapes.Square;
			}
		}

		private void PopulateExtentFields(VisualElement rootElement)
		{
			Action<int> VCOriginChanged = (int value) =>
			{
				mapComponent?.UpdateExtent();
			};

			Action<int> HCOriginChanged = (int value) =>
			{
				mapComponent?.UpdateExtent();

				if (value == SpatialReferenceWkid.WGS84 || value == SpatialReferenceWkid.CGCS2000)
				{
					extentXPositionField.label = "Longitude";
					extentYPositionField.label = "Latitude";
				}
				else
				{
					extentXPositionField.label = "X";
					extentYPositionField.label = "Y";
				}
			};

			Action<double> XExtentChanged = (double value) =>
			{
				if (!mapComponent)
				{
					return;
				}

				var extent = mapComponent.Extent;
				extent.GeographicCenter = new ArcGISPoint(value, extent.GeographicCenter.Y, extent.GeographicCenter.Z, extent.GeographicCenter.SpatialReference);
				mapComponent.UpdateExtent();
			};

			Action<double> YExtentChanged = (double value) =>
			{
				if (!mapComponent)
				{
					return;
				}

				var extent = mapComponent.Extent;
				extent.GeographicCenter = new ArcGISPoint(extent.GeographicCenter.X, value, extent.GeographicCenter.Z, extent.GeographicCenter.SpatialReference);
				mapComponent.UpdateExtent();
			};

			Action<double> XShapeChanged = (double value) =>
			{
				if (mapComponent)
				{
					mapComponent.UpdateExtent();
				}
			};

			Action<double> YShapeChanged = (double value) =>
			{
				if (mapComponent)
				{
					mapComponent.UpdateExtent();
				}
			};

			extentXPositionField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-extent-x", null, XExtentChanged);
			extentYPositionField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-extent-y", null, YExtentChanged);
			extentSRPositionField = MapCreatorUtilities.InitializeIntegerField(rootElement, "map-extent-spatial-reference-h", null, HCOriginChanged);
			extentVCPositionField = MapCreatorUtilities.InitializeIntegerField(rootElement, "map-extent-spatial-reference-v", null, VCOriginChanged);

			extentShapeXField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-shape-dimensions-x", null, XShapeChanged);
			extentShapeYField = MapCreatorUtilities.InitializeDoubleField(rootElement, "map-shape-dimensions-y", null, YShapeChanged);

			if (!mapComponent)
			{
				extentXPositionField.value = 0;
				extentYPositionField.value = 0;
				extentSRPositionField.value = SpatialReferenceWkid.WGS84;
				extentVCPositionField.value = 0;

				extentShapeXField.value = 0;
				extentShapeYField.value = 0;
			}

			MapCreatorUtilities.UpdateArcGISPointLabels(extentXPositionField, extentYPositionField, null, extentSRPositionField.value);
		}

		private void SwitchVisibleExtentShapeFields(MapExtentShapes shape)
		{
			switch (shape)
			{
				case MapExtentShapes.Square:
					extentShapeXField.label = "Length";
					extentShapeYField.style.display = DisplayStyle.None;
					break;
				case MapExtentShapes.Rectangle:
					extentShapeXField.label = "X";
					extentShapeYField.style.display = DisplayStyle.Flex;
					break;
				case MapExtentShapes.Circle:
					extentShapeXField.label = "Radius";
					extentShapeYField.style.display = DisplayStyle.None;
					break;
				default:
					break;
			}

			shapeField.value = shape;
		}

		private void InitCreateMapButton(VisualElement rootElement)
		{
			createMapButton = rootElement.Query<Button>(name: "button-create-map");
			createMapButton.clickable.activators.Clear();
			createMapButton.SetEnabled(MapCreatorUtilities.MapComponent == null);
		}

		private void CreateMap(MouseDownEvent evnt, VisualElement rootElement)
		{
			var gameObject = new GameObject("ArcGISMap");
			mapComponent = gameObject.AddComponent<ArcGISMapComponent>();
			GameObjectUtility.EnsureUniqueNameForSibling(gameObject);
			Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			var button = (Button)evnt.currentTarget;
			button.SetEnabled(false);

			mapComponent.UseCustomMapSpatialReference = manualSelection.value;

			if (manualSelection.value)
			{
				try
				{
					mapComponent.CustomMapSpatialReference = new ArcGISSpatialReference(manualSRField, manualVCField);
				}
				catch
				{
				}
			}

			mapComponent.EnableExtent = extentToggle.value;
			mapComponent.MapType = globalButton.enabledSelf == false ? ArcGISMapType.Global : ArcGISMapType.Local;

			mapComponent.OriginPosition = new ArcGISPoint(originXPositionField.value, originYPositionField.value, originZPositionField.value,
				new ArcGISSpatialReference(originSRPositionField.value, originVCPositionField.value));

			if (mapComponent.MapType == ArcGISMapType.Local && extentSRPositionField.value != 0)
			{
				mapComponent.Extent = new ArcGISExtentInstanceData()
				{
					GeographicCenter = new ArcGISPoint(extentXPositionField.value, extentYPositionField.value, 0,
						new ArcGISSpatialReference(extentSRPositionField.value, extentVCPositionField.value)),
					ExtentShape = (MapExtentShapes)shapeField.value,
					ShapeDimensions = new double2(extentShapeXField.value, extentShapeYField.value)
				};
			}

			mapComponent.SetBasemapSourceAndType(ArcGISMapCreatorBasemapTool.GetDefaultBasemap(), BasemapTypes.Basemap);

			var elevationSourceInstanceData = new ArcGISElevationSourceInstanceData();
			{
				elevationSourceInstanceData.AuthenticationType = ArcGISMapsSDK.Authentication.ArcGISAuthenticationType.APIKey;
				elevationSourceInstanceData.IsEnabled = true;
				elevationSourceInstanceData.Name = "";
				elevationSourceInstanceData.Source = ArcGISMapCreatorElevationTool.GetDefaultElevation();
				elevationSourceInstanceData.Type = ArcGISElevationSourceType.ArcGISImageElevationSource;
			}

			var mapElevationInstanceData = new ArcGISMapElevationInstanceData();

			mapElevationInstanceData.ElevationSources = new List<ArcGISElevationSourceInstanceData>(new ArcGISElevationSourceInstanceData[] { elevationSourceInstanceData });

			mapComponent.MapElevation = mapElevationInstanceData;

			Selection.activeGameObject = gameObject;

			OnSelected();
		}
	}
}
