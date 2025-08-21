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
using Esri.ArcGISMapsSDK.Utils.Math;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using System;
using Unity.Mathematics;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorCameraTool : ArcGISMapCreatorTool
	{
		private ArcGISCameraComponent selectedCamera;
		private ArcGISLocationComponent selectedLocationComponent;

		private Button createCamButton;
		private VisualElement rootElement;

		private DoubleField LocationFieldX;
		private DoubleField LocationFieldY;
		private DoubleField LocationFieldZ;
		private IntegerField LocationFieldSR;
		private IntegerField LocationFieldVC;

		private DoubleField RotationFieldHeading;
		private DoubleField RotationFieldPitch;
		private DoubleField RotationFieldRoll;

		private SceneView lastSceneView;

		public override void OnDeselected()
		{
			rootElement?.Unbind();
		}

		public override void OnEnable()
		{
			rootElement = new VisualElement();
			rootElement.name = "ArcGISMapCreatorCameraTool";

			var template = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/CameraToolTemplate.uxml");
			template.CloneTree(rootElement);

			rootElement.styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/CameraToolStyle.uss"));

			FindCamera();

			InitializeCreateCameraButton();

			InitializeCameraFields();

			InitAlignCameraToViewButton();
		}

		public override void OnSelected()
		{
			FindCamera();

			if (rootElement != null)
			{
				createCamButton?.SetEnabled(!selectedCamera || !selectedLocationComponent);

				InitializeCameraFields();
			}
		}

		public override VisualElement GetContent()
		{
			return rootElement;
		}

		public override Texture2D GetImage()
		{
			return MapCreatorUtilities.Assets.LoadImage($"MapCreator/Toolbar/{MapCreatorUtilities.Assets.GetThemeFolderName()}/CameraToolIcon.png");
		}

		public override string GetLabel()
		{
			return "Camera";
		}

		private void FindCamera()
		{
			selectedCamera = MapCreatorUtilities.CameraFromActiveMapComponent;

			if (selectedCamera)
			{
				if (!selectedCamera.transform.parent || !selectedCamera.transform.parent.GetComponent<ArcGISMapComponent>())
				{
					Debug.LogWarning("Parent the ArcGIS Camera game object to a game object with an ArcGIS Map component to use the Camera UI tool");
				}

				selectedLocationComponent = selectedCamera.GetComponent<ArcGISLocationComponent>();

				if (!selectedLocationComponent)
				{
					Debug.LogWarning("Attach an ArcGIS Location component to the ArcGIS Camera game object to use the full capability of the Camera UI tool");
				}
			}
			else
			{
				selectedLocationComponent = null;
			}
		}

		private void InitializeCreateCameraButton()
		{
			createCamButton = rootElement.Query<Button>(name: "button-create-camera");

			createCamButton.clickable.activators.Clear();
			createCamButton.RegisterCallback<MouseDownEvent>(evnt => CreateCamera(evnt));

			if (selectedCamera && selectedLocationComponent)
			{
				createCamButton.SetEnabled(false);
			}
		}

		private void CreateCamera(MouseDownEvent evnt)
		{
			GameObject cameraComponentGameObject;

			if (Camera.main && !Camera.main.GetComponentInParent<ArcGISMapComponent>())
			{
				cameraComponentGameObject = Camera.main.gameObject;
				cameraComponentGameObject.name = "ArcGISCamera";
			}
			else
			{
				cameraComponentGameObject = new GameObject("ArcGISCamera");
				cameraComponentGameObject.AddComponent<Camera>();
				cameraComponentGameObject.tag = "MainCamera";
			}

			if (SceneView.lastActiveSceneView)
			{
				cameraComponentGameObject.transform.position = SceneView.lastActiveSceneView.camera.transform.position;
				cameraComponentGameObject.transform.rotation = SceneView.lastActiveSceneView.camera.transform.rotation;
			}

			var mapComponent = (ArcGISMapComponent)MapCreatorUtilities.MapComponent;

			if (mapComponent)
			{
				cameraComponentGameObject.transform.parent = mapComponent.transform;
			}

			selectedCamera = cameraComponentGameObject.AddComponent<ArcGISCameraComponent>();

			selectedLocationComponent = cameraComponentGameObject.AddComponent<ArcGISLocationComponent>();
			selectedLocationComponent.Position = new ArcGISPoint(LocationFieldX.value, LocationFieldY.value, LocationFieldZ.value,
				new ArcGISSpatialReference(LocationFieldSR.value, LocationFieldVC.value));
			selectedLocationComponent.Rotation = new ArcGISRotation(RotationFieldHeading.value, RotationFieldPitch.value, RotationFieldRoll.value);

			var cameraLocationSerializedObject = new SerializedObject(selectedLocationComponent);
			rootElement.Bind(cameraLocationSerializedObject);

			createCamButton.SetEnabled(false);

			Undo.RegisterCreatedObjectUndo(cameraComponentGameObject, "Create " + cameraComponentGameObject.name);

			Selection.activeGameObject = cameraComponentGameObject;
		}

		private void InitializeCameraFields()
		{
			Action<double> doubleFieldChanged = (double value) =>
			{
				if (selectedLocationComponent)
				{
					selectedLocationComponent.OnPositionOrRotationChanged();
				}
			};

			Action<int> intFieldChanged = (int value) =>
			{
				if (selectedLocationComponent)
				{
					selectedLocationComponent.OnPositionOrRotationChanged();
				}

				MapCreatorUtilities.UpdateArcGISPointLabels(LocationFieldX, LocationFieldY, LocationFieldZ, LocationFieldSR.value);
			};

			LocationFieldX = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-position-x", null, doubleFieldChanged);
			LocationFieldY = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-position-y", null, doubleFieldChanged);
			LocationFieldZ = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-position-z", null, doubleFieldChanged);
			LocationFieldSR = MapCreatorUtilities.InitializeIntegerField(rootElement, "cam-position-wkid", null, intFieldChanged);
			LocationFieldVC = MapCreatorUtilities.InitializeIntegerField(rootElement, "cam-position-vwkid", null, intFieldChanged);

			RotationFieldHeading = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-rotation-heading", null, doubleFieldChanged);
			RotationFieldPitch = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-rotation-pitch", null, doubleFieldChanged);
			RotationFieldRoll = MapCreatorUtilities.InitializeDoubleField(rootElement, "cam-rotation-roll", null, doubleFieldChanged);

			if (selectedLocationComponent)
			{
				var cameraLocationSerializedObject = new SerializedObject(selectedLocationComponent);
				rootElement.Bind(cameraLocationSerializedObject);
			}
			else
			{
				var mapComponent = (ArcGISMapComponent)MapCreatorUtilities.MapComponent;

				rootElement.Unbind();

				LocationFieldX.SetValueWithoutNotify(0);
				LocationFieldY.SetValueWithoutNotify(0);
				LocationFieldZ.SetValueWithoutNotify(0);
				LocationFieldSR.SetValueWithoutNotify(mapComponent ? mapComponent.OriginPosition.SpatialReference.WKID : SpatialReferenceWkid.WGS84);
				LocationFieldVC.SetValueWithoutNotify(mapComponent ? mapComponent.OriginPosition.SpatialReference.VerticalWKID : 0);

				RotationFieldHeading.SetValueWithoutNotify(0);
				RotationFieldPitch.SetValueWithoutNotify(0);
				RotationFieldRoll.SetValueWithoutNotify(0);

				MapCreatorUtilities.UpdateArcGISPointLabels(LocationFieldX, LocationFieldY, LocationFieldZ, LocationFieldSR.value);
			}
		}

		private void InitAlignCameraToViewButton()
		{
			Button AlignCameraToViewButton = rootElement.Query<Button>(name: "button-transfer-to-camera");
			AlignCameraToViewButton.clickable.activators.Clear();
			AlignCameraToViewButton.RegisterCallback<MouseDownEvent>(evnt =>
			{
				if (Application.isPlaying)
				{
					return;
				}

				if (!lastSceneView || lastSceneView != SceneView.lastActiveSceneView)
				{
					lastSceneView = SceneView.lastActiveSceneView;
				}

				if (selectedCamera)
				{
					var cameraTransform = selectedCamera.GetComponent<HPTransform>();
					var mapComponent = cameraTransform.GetComponentInParent<ArcGISMapComponent>();

					cameraTransform.transform.position = lastSceneView.camera.transform.position;
					cameraTransform.transform.rotation = lastSceneView.camera.transform.rotation;

					var worldPosition = math.inverse(mapComponent.WorldMatrix).HomogeneousTransformPoint(SceneView.lastActiveSceneView.camera.transform.position.ToDouble3());
					var geoPosition = mapComponent.View.WorldToGeographic(worldPosition);

					geoPosition = GeoUtils.ProjectToSpatialReference(geoPosition, new ArcGISSpatialReference(LocationFieldSR.value, LocationFieldVC.value));

					if (!Double.IsNaN(geoPosition.X) && !Double.IsNaN(geoPosition.Y) && !Double.IsNaN(geoPosition.Z))
					{
						var worldRotation = mapComponent.UniverseRotation * SceneView.lastActiveSceneView.camera.transform.rotation;
						var geoRotation = GeoUtils.FromCartesianRotation(worldPosition, quaternionExtensions.ToQuaterniond(worldRotation),
							new ArcGISSpatialReference(LocationFieldSR.value, LocationFieldVC.value), mapComponent.MapType);

						if (selectedLocationComponent)
						{
							selectedLocationComponent.SyncPositionWithHPTransform();
							selectedLocationComponent.Position = geoPosition;
							selectedLocationComponent.Rotation = geoRotation;
						}
					}
				}
			});
		}
	}
}
