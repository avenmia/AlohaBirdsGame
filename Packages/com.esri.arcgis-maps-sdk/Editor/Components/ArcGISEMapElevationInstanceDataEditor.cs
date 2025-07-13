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
using System.Data;
using System.Data.SqlClient;
using Esri.ArcGISMapsSDK.Components;
using UnityEditor;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Editor.Components
{
	[CustomPropertyDrawer(typeof(ArcGISMapElevationInstanceData))]
	public class ArcGISMapElevationInstanceDataEditor : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var exaggerationFactorProp = property.FindPropertyRelative("ExaggerationFactor");
			var elevationSourcesProp = property.FindPropertyRelative("ElevationSources");

			DrawExaggerationFactor(position, exaggerationFactorProp, label);

			EditorGUILayout.PropertyField(elevationSourcesProp);
		}

		private void DrawExaggerationFactor(Rect position, SerializedProperty exaggerationFactorProp, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, exaggerationFactorProp);

			GUI.Label(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), "Exaggeration Factor");

			float sliderPadding = 10.0f;
			float sliderWidth = position.width - EditorGUIUtility.labelWidth - EditorGUIUtility.fieldWidth - sliderPadding;
			float sliderValue = Math.Clamp(exaggerationFactorProp.floatValue, 0.0f, 100.0f);

			float newSliderValue = GUI.HorizontalSlider(new Rect(position.x + EditorGUIUtility.labelWidth, position.y, sliderWidth, EditorGUIUtility.singleLineHeight), sliderValue, 0.0f, 100.0f);

			bool sliderMoved = sliderValue != newSliderValue;

			float floatFieldPadding = 12.0f;
			float floatFieldValue = EditorGUI.FloatField(new Rect(position.x + EditorGUIUtility.labelWidth + sliderWidth + floatFieldPadding, position.y, EditorGUIUtility.fieldWidth, EditorGUIUtility.singleLineHeight),
				sliderMoved ? newSliderValue : exaggerationFactorProp.floatValue);

			exaggerationFactorProp.floatValue = floatFieldValue;

			EditorGUI.EndProperty();
		}
	}
}
