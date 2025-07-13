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
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Esri.GameEngine.Geometry
{
	[Serializable]
	public partial class ArcGISPoint : ISerializationCallbackReceiver
	{
		[FormerlySerializedAs("X")]
		[SerializeField]
		internal double x;

		[FormerlySerializedAs("Y")]
		[SerializeField]
		internal double y;

		[FormerlySerializedAs("Z")]
		[SerializeField]
		internal double z;

		[SerializeField]
		internal int SRWkid = 4326;

		[SerializeField]
		internal ArcGISSpatialReference spatialReference;

		public void OnBeforeSerialize()
		{
			try
			{
				x = X;
				y = Y;
				z = Z;

				spatialReference = SpatialReference;

				spatialReference?.OnBeforeSerialize();
			}
			catch
			{
				x = 0;
				y = 0;
				z = 0;
				spatialReference = new ArcGISSpatialReference(4326);
			}
		}

		public enum Version
		{
			// Before any version changes were made
			BeforeCustomVersionWasAdded = 0,

			// Move from only HC in SR system to HC and VC in SR system
			VerticalCoordinates_2_0 = 1,

			// -----<new versions can be added above this line>-------------------------------------------------
			VersionPlusOne,

			LatestVersion = VersionPlusOne - 1
		}

		[SerializeField]
		Version version = Version.BeforeCustomVersionWasAdded;

		public void OnAfterDeserialize()
		{
			if (version < Version.VerticalCoordinates_2_0)
			{
				try
				{
					spatialReference = new ArcGISSpatialReference(SRWkid);
				}
				catch
				{
					spatialReference = new ArcGISSpatialReference(4326);
				}
			}
			else
			{
				spatialReference?.OnAfterDeserialize();
			}

			var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

			if (Handle != IntPtr.Zero)
			{
				PInvoke.RT_Geometry_destroy(Handle, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}

			errorHandler = Unity.ArcGISErrorManager.CreateHandler();

			var localSpatialReference = spatialReference == null ? IntPtr.Zero : spatialReference.Handle;

			Handle = PInvoke.RT_Point_createWithXYZSpatialReference(x, y, z, localSpatialReference, errorHandler);

			Unity.ArcGISErrorManager.CheckError(errorHandler);

			version = Version.LatestVersion;
		}
	}
}

