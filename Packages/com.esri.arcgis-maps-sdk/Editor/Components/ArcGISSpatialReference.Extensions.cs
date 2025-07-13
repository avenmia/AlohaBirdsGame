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
using Esri.GameEngine.Geometry;
using UnityEditor;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	public static class ArcGISSpatialReferenceExtensions
	{
		public static void ApplyToSerializedProperty(this ArcGISSpatialReference spatialReference, SerializedProperty serializedProperty)
		{
			spatialReference.OnBeforeSerialize();

			serializedProperty.FindPropertyRelative("wkid").intValue = spatialReference.wkid;
			serializedProperty.FindPropertyRelative("verticalWKID").intValue = spatialReference.verticalWKID;
		}
	}
}
