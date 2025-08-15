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
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Editor.Utils;
using UnityEditor;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	[CustomEditor(typeof(ArcGISLocationComponent))]
	public class ArcGISLocationComponentEditor : UnityEditor.Editor
	{
		private bool showLocation = true;
		private bool showSurfacePlacement = true;

		public override void OnInspectorGUI()
		{
			var locationComponent = target as ArcGISLocationComponent;

			if (locationComponent.GetComponentInParent<ArcGISMapComponent>() == null)
			{
				EditorGUILayout.HelpBox("GameObjects with an ArcGIS Location Component attached must be child to a GameObject with an ArcGIS Map Component attached.", MessageType.Warning);
			}

			if (!locationComponent.enabled)
			{
				return;
			}

			showLocation = EditorGUILayout.BeginFoldoutHeaderGroup(showLocation, "Location");

			if (showLocation)
			{
				EditorUtilities.PropertyField(serializedObject, "position");
				EditorUtilities.PropertyField(serializedObject, "rotation");
			}

			EditorGUILayout.EndFoldoutHeaderGroup();

			showSurfacePlacement = EditorGUILayout.BeginFoldoutHeaderGroup(showSurfacePlacement, "Surface Placement");

			if (showSurfacePlacement)
			{
				EditorUtilities.PropertyField(serializedObject, "surfacePlacementMode");

				if ((ArcGISSurfacePlacementMode)serializedObject.GetValue("surfacePlacementMode") == ArcGISSurfacePlacementMode.RelativeToGround)
				{
					EditorUtilities.PropertyField(serializedObject, "surfacePlacementOffset");
				}
			}

			EditorGUILayout.EndFoldoutHeaderGroup();
		}
	}
}
