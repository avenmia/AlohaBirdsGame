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
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Editor.Utils;
using UnityEditor;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	public static class ArcGISLayerInstanceDataExtensions
	{
		public static void ApplyToSerializedProperty(this ArcGISLayerInstanceData layerInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("AuthenticationType").intValue = (int)layerInstanceData.AuthenticationType;
			serializedProperty.FindPropertyRelative("IsVisible").boolValue = layerInstanceData.IsVisible;
			serializedProperty.FindPropertyRelative("Name").stringValue = layerInstanceData.Name;
			serializedProperty.FindPropertyRelative("Opacity").floatValue = layerInstanceData.Opacity;
			serializedProperty.FindPropertyRelative("Source").stringValue = layerInstanceData.Source;
			serializedProperty.FindPropertyRelative("Type").intValue = (int)layerInstanceData.Type;
		}

		public static void ApplyToSerializedProperty(this ArcGISMeshModificationInstanceData modificationInstanceData, SerializedProperty serializedProperty)
		{
			modificationInstanceData.Polygon.ApplyToSerializedProperty(serializedProperty.FindPropertyRelative("Polygon"));
			serializedProperty.FindPropertyRelative("Type").intValue = (int)modificationInstanceData.Type;
		}

		public static void ApplyMeshModificationsToSerializedProperty(this ArcGISMeshModificationsInstanceData modificationsInstanceData, SerializedProperty serializedProperty)
		{
			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("MeshModifications"));

			serializedPropertyList.Resize(modificationsInstanceData.MeshModifications.Count);

			for (int i = 0; i < modificationsInstanceData.MeshModifications.Count; i++)
			{
				modificationsInstanceData.MeshModifications[i].ApplyToSerializedProperty(serializedPropertyList.Get(i));
			}
		}

		public static void ApplyToSerializedProperty(this ArcGISMeshModificationsInstanceData modificationsInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("IsEnabled").boolValue = modificationsInstanceData.IsEnabled;

			modificationsInstanceData.ApplyMeshModificationsToSerializedProperty(serializedProperty);
		}

		public static void ApplyToSerializedProperty(this ArcGISPolygonInstanceData polygonInstanceData, SerializedProperty serializedProperty)
		{
			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("Points"));

			serializedPropertyList.Resize(polygonInstanceData.Points.Count);

			for (int i = 0; i < polygonInstanceData.Points.Count; i++)
			{
				polygonInstanceData.Points[i].ApplyToSerializedProperty(serializedPropertyList.Get(i));
			}
		}

		public static void ApplyPolygonsToSerializedProperty(this ArcGISSpatialFeatureFilterInstanceData spatialFeatureFilterInstanceData, SerializedProperty serializedProperty)
		{
			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("Polygons"));

			serializedPropertyList.Resize(spatialFeatureFilterInstanceData.Polygons.Count);

			for (int i = 0; i < spatialFeatureFilterInstanceData.Polygons.Count; i++)
			{
				spatialFeatureFilterInstanceData.Polygons[i].ApplyToSerializedProperty(serializedPropertyList.Get(i));
			}
		}

		public static void ApplyTypeToSerializedProperty(this ArcGISSpatialFeatureFilterInstanceData spatialFeatureFilterInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("Type").intValue = (int)spatialFeatureFilterInstanceData.Type;
		}

		public static void ApplyToSerializedProperty(this ArcGISSpatialFeatureFilterInstanceData spatialFeatureFilterInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("IsEnabled").boolValue = spatialFeatureFilterInstanceData.IsEnabled;

			spatialFeatureFilterInstanceData.ApplyPolygonsToSerializedProperty(serializedProperty);

			spatialFeatureFilterInstanceData.ApplyTypeToSerializedProperty(serializedProperty);
		}
	}
}
