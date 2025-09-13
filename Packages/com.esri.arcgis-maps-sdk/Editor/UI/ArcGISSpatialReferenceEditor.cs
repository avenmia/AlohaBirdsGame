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
using Esri.GameEngine.Geometry;
using UnityEditor;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	[CustomPropertyDrawer(typeof(ArcGISSpatialReference))]
	public class ArcGISSpatialReferenceEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var srProp = property.FindPropertyRelative("wkid");
			var vcProp = property.FindPropertyRelative("verticalWKID");

			var srLabel = "Horizontal";
			var vcLabel = "Vertical";

			using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
			using (var checkScope = new EditorGUI.ChangeCheckScope())
			{
				const float padding = 6.0f;

				EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), "Spatial Reference");

				var fieldWidth = Mathf.Floor((position.width - EditorGUIUtility.labelWidth - padding) / 2.0f);

				float prevLabelWidth = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 70.0f;

				EditorGUI.DelayedIntField(
					new Rect(position.x + prevLabelWidth, position.y, fieldWidth, EditorGUIUtility.singleLineHeight),
					srProp, new GUIContent(srLabel));

				EditorGUIUtility.labelWidth = 60.0f;

				EditorGUI.DelayedIntField(
					new Rect(position.x + prevLabelWidth + fieldWidth + padding, position.y, fieldWidth, EditorGUIUtility.singleLineHeight),
					vcProp, new GUIContent(vcLabel));

				EditorGUIUtility.labelWidth = prevLabelWidth;

				if (checkScope.changed)
				{
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
			}
		}
	}
}
