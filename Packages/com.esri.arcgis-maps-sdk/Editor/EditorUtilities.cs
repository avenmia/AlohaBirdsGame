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
using Esri.ArcGISMapsSDK.Utils;
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.GameEngine.Geometry;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[assembly: InternalsVisibleTo("EditTests")]
namespace Esri.ArcGISMapsSDK.Editor.Components
{
	public class EditorUtilities
	{
		private static void Double(ref double value)
		{
			value = EditorGUILayout.DoubleField(value, GUILayout.ExpandWidth(true));
		}

		private static void Int(ref int value)
		{
			value = EditorGUILayout.IntField(value, GUILayout.ExpandWidth(true));
		}

		public static T EnumField<T>(string label, T value) where T : Enum
		{
			Rect rect = EditorGUILayout.BeginHorizontal();

			GUILayout.Label(label, GUILayout.Width(150.0f));

			var result = EditorGUILayout.EnumPopup(value);

			EditorGUILayout.EndHorizontal();

			return (T)result;
		}

		private static void Float(ref float value)
		{
			value = EditorGUILayout.FloatField(value, GUILayout.ExpandWidth(true));
		}

		public static float FloatField(string label, float value)
		{
			Rect rect = EditorGUILayout.BeginHorizontal();

			GUILayout.Label(label, GUILayout.Width(150.0f));

			Float(ref value);

			EditorGUILayout.EndHorizontal();

			return value;
		}

		public static bool BoolField(string label, bool value)
		{
			Rect rect = EditorGUILayout.BeginHorizontal();

			GUILayout.Label(label, GUILayout.Width(150.0f));

			var result = EditorGUILayout.Toggle(value);

			EditorGUILayout.EndHorizontal();

			return result;
		}

		private static void Label(string str)
		{
			GUIContent labelContent = new GUIContent(str);
			float width = GUI.skin.GetStyle("Label").CalcSize(labelContent).x;
			GUILayout.Label(labelContent, GUILayout.Width(width));
		}

		public static ArcGISPoint ArcGISPointField(string label, ArcGISPoint value, bool hideZ = false)
		{
			Rect rect = EditorGUILayout.BeginHorizontal();

			GUILayout.Label(label, GUILayout.Width(150.0f));
			double x = 0;
			double y = 0;
			double z = 0;
			int srwkid = SpatialReferenceWkid.WGS84;

			if (value && value.IsValid)
			{
				x = value.X;
				y = value.Y;
				z = value.Z;
				if (value.SpatialReference)
				{
					srwkid = value.SpatialReference.WKID;
				}
			}

			if (srwkid == SpatialReferenceWkid.WGS84 || srwkid == SpatialReferenceWkid.CGCS2000)
			{
				Label("Longitude");
				Double(ref x);
				Label("Latitude");
				Double(ref y);
				if (!hideZ)
				{
					Label("Altitude");
					Double(ref z);
				}
			}
			else
			{
				Label("X");
				Double(ref x);
				Label("Y");
				Double(ref y);
				if (!hideZ)
				{
					Label("Z");
					Double(ref z);
				}
			}

			Label("Spatial Ref WKID");
			Int(ref srwkid);

			if (!value || !value.IsValid)
			{
				value = new ArcGISPoint(x, y, z, new ArcGISSpatialReference(SpatialReferenceWkid.WGS84));
			}
			else if (x != value.X || y != value.Y || z != value.Z || srwkid != value.SpatialReference.WKID)
			{
				ArcGISSpatialReference spatialReference;
				try
				{
					spatialReference = new ArcGISSpatialReference(srwkid);
				}
				catch
				{
					spatialReference = value.SpatialReference;
				}
				value = new ArcGISPoint(x, y, z, spatialReference);
			}

			EditorGUILayout.EndHorizontal();

			return value;
		}

		public static ArcGISRotation ArcGISRotationField(string label, ArcGISRotation value)
		{
			Rect rect = EditorGUILayout.BeginHorizontal();

			GUILayout.Label(label, GUILayout.Width(150.0f));

			Label("Heading");
			Double(ref value.Heading);
			Label("Pitch");
			Double(ref value.Pitch);
			Label("Roll");
			Double(ref value.Roll);

			EditorGUILayout.EndHorizontal();

			return value;
		}

		public static void PropertyField(SerializedObject serializedObject, string propertyName, GUIContent label = null)
		{
			var property = serializedObject.FindProperty(propertyName);

			EditorGUI.BeginChangeCheck();
			if (label != null)
			{
				EditorGUILayout.PropertyField(property, label);
			}
			else
			{
				EditorGUILayout.PropertyField(property);
			}
			if (!EditorGUI.EndChangeCheck())
			{
				return;
			}

			property.serializedObject.ApplyModifiedProperties();

			var fieldInfo = serializedObject.targetObject
				.GetType()
				.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(fieldInfo => fieldInfo.Name == propertyName).FirstOrDefault();

			Debug.Assert(fieldInfo != null);

			var onChangedCallAttribute = fieldInfo?.GetCustomAttributes<OnChangedCallAttribute>()?.FirstOrDefault();

			if (onChangedCallAttribute == null)
			{
				return;
			}

			var method = serializedObject.targetObject.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Where(methodInfo => methodInfo.Name == onChangedCallAttribute.OnChangedHandler).FirstOrDefault();

			if (method == null)
			{
				return;
			}

			var parameters = method.GetParameters();

			var arguments = new object[parameters.Length];

			for (int i = 0; i < arguments.Length; i++)
			{
				if (parameters[i].HasDefaultValue)
				{
					arguments[i] = parameters[i].DefaultValue;
				}
				else
				{
					Debug.LogError("Cannot invoke a method with non-defaulted parameters");

					return;
				}
			}

			method.Invoke(property.serializedObject.targetObject, arguments);
		}

		// Copied from https://discussions.unity.com/t/is-there-no-placeholder-text-property-for-textfield/806946/8
		public static void SetPlaceholderText(TextField textField, string placeholder, FontStyle placeHolderFontStyle = FontStyle.Italic)
		{
			string placeholderClass = TextField.inputUssClassName + "--placeholder";

			var originalFontStyle = textField.style.unityFontStyleAndWeight.value;
			textField.style.unityFontStyleAndWeight = placeHolderFontStyle;

			textField.RegisterCallback<FocusInEvent>(evt => onFocusIn());
			textField.RegisterCallback<FocusOutEvent>(evt => onFocusOut());

			void onFocusIn()
			{
				if (textField.ClassListContains(placeholderClass))
				{
					textField.value = string.Empty;
					textField.RemoveFromClassList(placeholderClass);
					textField.style.unityFontStyleAndWeight = originalFontStyle;
				}
			}

			void onFocusOut()
			{
				if (string.IsNullOrEmpty(textField.text))
				{
					textField.SetValueWithoutNotify(placeholder);
					textField.AddToClassList(placeholderClass);
					textField.style.unityFontStyleAndWeight = placeHolderFontStyle;
				}
			}

			onFocusOut();
		}

		public static string GetTextFieldValue(TextField textField)
		{
			string placeholderClass = TextField.inputUssClassName + "--placeholder";

			return textField.ClassListContains(placeholderClass) ? string.Empty : textField.value;
		}
	}
}
