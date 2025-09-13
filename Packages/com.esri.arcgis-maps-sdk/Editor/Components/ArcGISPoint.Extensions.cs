// COPYRIGHT 1995-2023 ESRI
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
	public static class ArcGISPointExtensions
	{
		public static void ApplyToSerializedProperty(this ArcGISPoint point, SerializedProperty serializedProperty)
		{
			point.OnBeforeSerialize();

			serializedProperty.FindPropertyRelative("x").doubleValue = point.x;
			serializedProperty.FindPropertyRelative("y").doubleValue = point.y;
			serializedProperty.FindPropertyRelative("z").doubleValue = point.z;
			point.spatialReference.ApplyToSerializedProperty(serializedProperty.FindPropertyRelative("spatialReference"));
		}
	}
}
