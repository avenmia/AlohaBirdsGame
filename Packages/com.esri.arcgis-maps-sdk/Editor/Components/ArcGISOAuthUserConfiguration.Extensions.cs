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
using UnityEditor;

namespace Esri.GameEngine.Authentication
{
	internal static class ArcGISOAuthUserConfigurationExtensions
	{
		internal static void ApplyToSerializedProperty(this ArcGISOAuthUserConfiguration authenticationConfiguration, SerializedProperty serializedProperty)
		{
			serializedProperty.FindPropertyRelative("clientId").stringValue = authenticationConfiguration.clientId;
			serializedProperty.FindPropertyRelative("name").stringValue = authenticationConfiguration.name;
			serializedProperty.FindPropertyRelative("portalURL").stringValue = authenticationConfiguration.portalURL;
			serializedProperty.FindPropertyRelative("redirectURL").stringValue = authenticationConfiguration.redirectURL;
		}
	}
}
