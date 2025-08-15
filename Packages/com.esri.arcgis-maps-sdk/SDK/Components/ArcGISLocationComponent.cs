// COPYRIGHT 1995-2020 ESRI
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
using Esri.ArcGISMapsSDK.Utils;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.ArcGISMapsSDK.Utils.Math;
using Esri.GameEngine.Elevation;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[RequireComponent(typeof(HPTransform))]
	[AddComponentMenu("Tools/ArcGIS Maps SDK/ArcGIS Location")]
	public class ArcGISLocationComponent : MonoBehaviour
	{
		private enum SyncAction
		{
			None,
			Pull,
			Push
		};

		private ArcGISElevationMonitor elevationMonitor;
		private HPTransform hpTransform;
		protected ArcGISMapComponent mapComponent;
		private bool surfacePlacementChanged = true;
		private SyncAction syncAction = SyncAction.None;
		private double3 universePosition;
		private quaternion universeRotation;

		[SerializeField]
		[OnChangedCall("OnPositionChanged")]
		private ArcGISPoint position = null;
		public ArcGISPoint Position
		{
			get => position;
			set
			{
				SetPosition(value);
			}
		}

		[SerializeField]
		[OnChangedCall("OnRotationChanged")]
		private ArcGISRotation rotation;
		public ArcGISRotation Rotation
		{
			get => rotation;
			set
			{
				if (rotation == value)
				{
					return;
				}

				rotation = value;

				OnRotationChanged();
			}
		}

		[SerializeField]
		[OnChangedCall("OnSurfacePlacementChanged")]
		private ArcGISSurfacePlacementMode surfacePlacementMode = ArcGISSurfacePlacementMode.AbsoluteHeight;
		public ArcGISSurfacePlacementMode SurfacePlacementMode
		{
			get => surfacePlacementMode;
			set
			{
				if (surfacePlacementMode == value)
				{
					return;
				}

				surfacePlacementMode = value;

				OnSurfacePlacementChanged();
			}
		}

		[SerializeField]
		[OnChangedCall("OnSurfacePlacementChanged")]
		private double surfacePlacementOffset = 0.0;
		public double SurfacePlacementOffset
		{
			get => surfacePlacementOffset;
			set
			{
				if (surfacePlacementOffset == value)
				{
					return;
				}

				surfacePlacementOffset = value;

				OnSurfacePlacementChanged();
			}
		}

		protected void Awake()
		{
			Initialize();

			// If position is null, it means the component has just been added so we need to
			// calculate position and rotation from the transform
			StartSync(position == null ? SyncAction.Pull : SyncAction.Push, true);
		}

		private void Initialize()
		{
			hpTransform = GetComponent<HPTransform>();

			mapComponent = gameObject.GetComponentInParent<ArcGISMapComponent>();

			if (!mapComponent)
			{
				Debug.LogWarning("Unable to find a parent ArcGISMapComponent.");

				return;
			}

			// When SR changes recalculate from geographic position
			mapComponent.View.SpatialReferenceChanged += () =>
			{
				syncAction = position == null ? SyncAction.Pull : SyncAction.Push;

				surfacePlacementChanged = true;
			};
		}

		protected void LateUpdate()
		{
			// If changes to HPTransform were not pushed by us, recalculate position and rotation
			var hasPositionChanged = !universePosition.Equals(hpTransform.UniversePosition);
			var hasRotationChanged = !universeRotation.Equals(hpTransform.UniverseRotation);

			StartSync(hasPositionChanged || hasRotationChanged ? SyncAction.Pull : syncAction, surfacePlacementChanged || hasPositionChanged);
		}

		protected void OnEnable()
		{
			// Our OnEnable runs after HPTransform's
			universePosition = hpTransform.UniversePosition;
			universeRotation = hpTransform.UniverseRotation;
		}

		private void OnPositionChanged(bool surfacePlacementChanged = true)
		{
			StartSync(SyncAction.Push, surfacePlacementChanged);
		}

		private void OnRotationChanged()
		{
			StartSync(SyncAction.Push, false);
		}

		private void OnSurfacePlacementChanged()
		{
			StartSync(SyncAction.None, true);
		}

		protected void OnTransformParentChanged()
		{
			Initialize();

			// We consider the parent change a transform change so we need to calculate position
			// and rotation from the transform
			StartSync(SyncAction.Pull, true);
		}

		private void PullChangesFromHPTransform()
		{
			universePosition = hpTransform.UniversePosition;
			universeRotation = hpTransform.UniverseRotation;

			var cartesianPosition = universePosition;
			var cartesianRotation = universeRotation.ToQuaterniond();

			var newPosition = mapComponent.View.WorldToGeographic(cartesianPosition); // May result in NaN position

			if (position != null && position.IsValid)
			{
				// this try catch is necessary because the below mentioned example could try to project between 2 SR's that cannot be projected between
				try
				{
					// When creating a location component with a specific SR and then sliding it around or updating the HPTransform
					// this method can change the SR of the Location component which is strange behavior
					position = GeoUtils.ProjectToSpatialReference(newPosition, position.SpatialReference); // this is a no-op if the sr is already the same
				}
				catch
				{
					position = newPosition;
				}
			}
			else
			{
				position = newPosition;
			}

			rotation = GeoUtils.FromCartesianRotation(cartesianPosition, cartesianRotation, mapComponent.View.SpatialReference, mapComponent.MapType);
		}

		private void PushChangesToHPTransform()
		{
			if (!position)
			{
				return;
			}

			var cartesianPosition = mapComponent.View.GeographicToWorld(position);

			if (!cartesianPosition.IsValid())
			{
				// If the geographic position is not a valid cartesian position, ignore it
				PullChangesFromHPTransform(); // Reset position from current, assumed value, cartesian position

				return;
			}

			var cartesianRotation = GeoUtils.ToCartesianRotation(cartesianPosition, rotation, mapComponent.View.SpatialReference, mapComponent.MapType);

			universePosition = cartesianPosition;
			universeRotation = cartesianRotation.ToQuaternion();

			hpTransform.UniversePosition = universePosition;
			hpTransform.UniverseRotation = universeRotation;
		}

		public static void SetPositionAndRotation(GameObject gameObject, ArcGISPoint geographicPosition, ArcGISRotation geographicRotation)
		{
			var locationComponent = gameObject.GetComponent<ArcGISLocationComponent>();

			if (locationComponent)
			{
				locationComponent.Position = geographicPosition;
				locationComponent.Rotation = geographicRotation;

				return;
			}

			var hpTransform = gameObject.GetComponent<HPTransform>();

			if (!hpTransform)
			{
				throw new InvalidOperationException(gameObject.name + " requires an HPTransform");
			}

			var mapComponent = gameObject.GetComponentInParent<ArcGISMapComponent>();

			if (!mapComponent)
			{
				throw new InvalidOperationException(gameObject.name + " should be child of a ArcGISMapComponent");
			}

			var spatialReference = mapComponent.View.SpatialReference;

			if (!spatialReference)
			{
				throw new InvalidOperationException("View must have a spatial reference");
			}

			var cartesianPosition = mapComponent.View.GeographicToWorld(geographicPosition);

			hpTransform.UniversePosition = cartesianPosition;
			hpTransform.UniverseRotation = GeoUtils.ToCartesianRotation(cartesianPosition, geographicRotation, spatialReference, mapComponent.MapType).ToQuaternion();
		}

		internal void OnPositionOrRotationChanged()
		{
			StartSync(SyncAction.Push, true);
		}

		private void SetPosition(ArcGISPoint point, bool surfacePlacementChanged = true)
		{
			if (position == point)
			{
				return;
			}

			position = point;

			OnPositionChanged(surfacePlacementChanged);
		}

		private void StartSync(SyncAction syncAction, bool surfacePlacementChanged)
		{
			this.syncAction = syncAction;

			this.surfacePlacementChanged = surfacePlacementChanged;

			SyncPositionWithHPTransform();

			UpdateElevationMonitor();
		}

		internal void SyncPositionWithHPTransform()
		{
			if (syncAction == SyncAction.None || !mapComponent || !mapComponent.HasSpatialReference())
			{
				return;
			}

			if (syncAction == SyncAction.Push)
			{
				PushChangesToHPTransform();
			}
			else
			{
				PullChangesFromHPTransform();
			}

			syncAction = SyncAction.None;
		}

		private void UpdateElevationMonitor()
		{
			if (!surfacePlacementChanged)
			{
				return;
			}

			if (elevationMonitor)
			{
				elevationMonitor.PositionChanged = null;
			}

			if (!mapComponent)
			{
				return;
			}

			surfacePlacementChanged = false;

			if (elevationMonitor)
			{
				mapComponent.View.ElevationProvider.UnregisterMonitor(elevationMonitor);
			}

			if (surfacePlacementMode == ArcGISSurfacePlacementMode.AbsoluteHeight || !position)
			{
				return;
			}

			elevationMonitor = new ArcGISElevationMonitor(
				position,
				ArcGISElevationMode.RelativeToGround,
				surfacePlacementMode == ArcGISSurfacePlacementMode.OnTheGround ? 0 : surfacePlacementOffset
			);

			elevationMonitor.PositionChanged += delegate (ArcGISPoint position)
			{
				// Avoid forcing an update of the elevation monitor
				SetPosition(position, false);
			};

			mapComponent.View.ElevationProvider.RegisterMonitor(elevationMonitor);
		}
	}
}
