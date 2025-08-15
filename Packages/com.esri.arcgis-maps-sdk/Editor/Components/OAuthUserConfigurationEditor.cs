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
using Esri.GameEngine.Authentication;
using System;
using UnityEditor;
using UnityEngine;

namespace Assets.ArcGISMapsSDK.Editor.Components
{
	[CustomPropertyDrawer(typeof(ArcGISOAuthUserConfiguration))]
	public class OAuthUserConfigurationEditor : PropertyDrawer
	{
		private const int RowCount = 5;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var portalProp = property.FindPropertyRelative("portalURL");
			var nameProp = property.FindPropertyRelative("name");
			var clientProp = property.FindPropertyRelative("clientId");
			var redirectProp = property.FindPropertyRelative("redirectURL");

			int rectIndex = 0;
			Func<Rect> GetRect = () =>
			{
				return new Rect(position.x, position.y + rectIndex++ * EditorGUIUtility.singleLineHeight + (rectIndex - 1) * EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight);
			};

			using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
			using (var checkScope = new EditorGUI.ChangeCheckScope())
			{
				EditorGUI.LabelField(GetRect(), nameProp.stringValue);

				using (new EditorGUI.IndentLevelScope())
				{
					EditorGUI.PropertyField(GetRect(), portalProp);
					EditorGUI.PropertyField(GetRect(), nameProp);
					EditorGUI.PropertyField(GetRect(), clientProp);
					EditorGUI.PropertyField(GetRect(), redirectProp);
				}
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return RowCount * EditorGUIUtility.singleLineHeight + (RowCount - 1) * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
