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
	public static class ArcGISBuildingSceneLayerFilterInstanceDataExtensions
	{
		public static void ApplyToSerializedProperty(this ArcGISBuildingAttributeFilterInstanceData buildingAttributeFilterInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("Description").stringValue = buildingAttributeFilterInstanceData.Description;
			serializedProperty.FindPropertyRelative("FilterID").stringValue = buildingAttributeFilterInstanceData.FilterID;
			serializedProperty.FindPropertyRelative("Name").stringValue = buildingAttributeFilterInstanceData.Name;
			buildingAttributeFilterInstanceData.SolidFilterDefinition.ApplyToSerializedProperty(serializedProperty.FindPropertyRelative("SolidFilterDefinition"));
		}

		public static void ApplyToSerializedProperty(this ArcGISBuildingSceneLayerAttributeStatisticsInstanceData buildingSceneLayerAttributeStatisticsInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("FieldName").stringValue = buildingSceneLayerAttributeStatisticsInstanceData.FieldName;

			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("MostFrequentValues"));

			serializedPropertyList.Resize(buildingSceneLayerAttributeStatisticsInstanceData.MostFrequentValues.Count);

			for (int i = 0; i < buildingSceneLayerAttributeStatisticsInstanceData.MostFrequentValues.Count; i++)
			{
				serializedPropertyList.Get(i).stringValue = buildingSceneLayerAttributeStatisticsInstanceData.MostFrequentValues[i];
			}
		}

		public static void ApplyAttributeFiltersToSerializedProperty(this ArcGISBuildingSceneLayerFilterInstanceData buildingSceneLayerFilterInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("ActiveBuildingAttributeFilterIndex").intValue = buildingSceneLayerFilterInstanceData.ActiveBuildingAttributeFilterIndex;

			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("BuildingAttributeFilters"));

			serializedPropertyList.Resize(buildingSceneLayerFilterInstanceData.BuildingAttributeFilters.Count);

			for (int i = 0; i < buildingSceneLayerFilterInstanceData.BuildingAttributeFilters.Count; i++)
			{
				buildingSceneLayerFilterInstanceData.BuildingAttributeFilters[i].ApplyToSerializedProperty(serializedPropertyList.Get(i));
			}
		}

		public static void ApplyDisciplinesCategoriesToSerializedProperty(this ArcGISBuildingSceneLayerFilterInstanceData buildingSceneLayerFilterInstanceData, SerializedProperty serializedProperty)
		{
			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("EnabledCategories"));

			serializedPropertyList.Resize(buildingSceneLayerFilterInstanceData.EnabledCategories.Count);

			for (int i = 0; i < buildingSceneLayerFilterInstanceData.EnabledCategories.Count; i++)
			{
				serializedPropertyList.Get(i).longValue = buildingSceneLayerFilterInstanceData.EnabledCategories[i];
			}

			serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("EnabledDisciplines"));

			serializedPropertyList.Resize(buildingSceneLayerFilterInstanceData.EnabledDisciplines.Count);

			for (int i = 0; i < buildingSceneLayerFilterInstanceData.EnabledDisciplines.Count; i++)
			{
				serializedPropertyList.Get(i).intValue = (int)buildingSceneLayerFilterInstanceData.EnabledDisciplines[i];
			}
		}

		public static void ApplyToSerializedProperty(this ArcGISBuildingSceneLayerFilterInstanceData buildingSceneLayerFilterInstanceData, SerializedProperty serializedProperty)
		{
			buildingSceneLayerFilterInstanceData.ApplyAttributeFiltersToSerializedProperty(serializedProperty);

			serializedProperty.FindPropertyRelative("IsBuildingAttributeFilterEnabled").boolValue = buildingSceneLayerFilterInstanceData.IsBuildingAttributeFilterEnabled;
			serializedProperty.FindPropertyRelative("IsBuildingDisciplinesCategoriesFilterEnabled").boolValue = buildingSceneLayerFilterInstanceData.IsBuildingDisciplinesCategoriesFilterEnabled;

			buildingSceneLayerFilterInstanceData.ApplyDisciplinesCategoriesToSerializedProperty(serializedProperty);
		}

		public static void ApplyToSerializedProperty(this ArcGISSolidBuildingFilterDefinitionInstanceData solidBuildingFilterDefinitionInstanceData, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("Title").stringValue = solidBuildingFilterDefinitionInstanceData.Title;
			serializedProperty.FindPropertyRelative("WhereClause").stringValue = solidBuildingFilterDefinitionInstanceData.WhereClause;

			var serializedPropertyList = new SerializedPropertyList(serializedProperty.FindPropertyRelative("EnabledStatistics"));

			serializedPropertyList.Resize(solidBuildingFilterDefinitionInstanceData.EnabledStatistics.Count);

			for (int i = 0; i < solidBuildingFilterDefinitionInstanceData.EnabledStatistics.Count; i++)
			{
				solidBuildingFilterDefinitionInstanceData.EnabledStatistics[i].ApplyToSerializedProperty(serializedPropertyList.Get(i));
			}
		}
	}
}
