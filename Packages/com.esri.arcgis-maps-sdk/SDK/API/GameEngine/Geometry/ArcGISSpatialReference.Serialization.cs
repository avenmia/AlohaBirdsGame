// COPYRIGHT 1995-2025 ESRI
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

namespace Esri.GameEngine.Geometry
{
	[Serializable]
	public partial class ArcGISSpatialReference : ISerializationCallbackReceiver
	{
		[SerializeField]
		internal int wkid = 4326;

		[SerializeField]
		internal int verticalWKID;

		public void OnBeforeSerialize()
		{
			if (Handle != IntPtr.Zero)
			{
				wkid = WKID;
				verticalWKID = VerticalWKID;
			}
		}

		public void OnAfterDeserialize()
		{
			var errorHandler = Unity.ArcGISErrorManager.CreateHandler();

			int previousWKID = 4326;
			int previousVerticalWKID = 0;

			if (Handle != IntPtr.Zero)
			{
				previousWKID = WKID;
				previousVerticalWKID = VerticalWKID;

				PInvoke.RT_SpatialReference_destroy(Handle, errorHandler);
				Handle = IntPtr.Zero;

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}

			try
			{
				errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				Handle = PInvoke.RT_SpatialReference_createVerticalWKID(wkid, verticalWKID, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
			catch
			{
				errorHandler = Unity.ArcGISErrorManager.CreateHandler();

				Handle = PInvoke.RT_SpatialReference_createVerticalWKID(previousWKID, previousVerticalWKID, errorHandler);

				Unity.ArcGISErrorManager.CheckError(errorHandler);
			}
		}
	}
}
