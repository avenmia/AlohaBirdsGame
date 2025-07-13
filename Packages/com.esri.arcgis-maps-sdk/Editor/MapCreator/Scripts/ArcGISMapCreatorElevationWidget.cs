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
using Esri.HPFramework;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Esri.ArcGISMapsSDK.Editor.UI
{
	public class ArcGISMapCreatorElevationWidget : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<ArcGISMapCreatorElevationWidget> { }

		public class Item
		{
			public bool CanBeRemoved;
			public Texture2D Image;
			public bool IsEnabled;
			public string Name;
			public object UserData;
		}

		public static ArcGISElevationSourceInstanceData DefaultElevationSourceData
		{
			get { return ArcGISMapComponent.DefaultElevationSourceData; }
		}

		public event Action<bool> OnEnableAllToggleValueChanged;

		private VisualTreeAsset elevationItemTemplate;
		private SerializedPropertyList elevationSourcesList;
		private ListView listView;
		private ArcGISMapComponent mapComponent;

		public ArcGISMapCreatorElevationWidget()
		{
			styleSheets.Add(MapCreatorUtilities.Assets.LoadStyleSheet("MapCreator/ElevationCardStyle.uss"));

			elevationItemTemplate = MapCreatorUtilities.Assets.LoadVisualTreeAsset("MapCreator/ElevationCardTemplate.uxml");

			SetupListView();
		}

		public void Rebuild()
		{
			if (listView == null)
			{
				SetupListView();
			}
			else if (IsMapComponentInvalid())
			{
				listView.Unbind();
				BindListView();
			}

			listView.Rebuild();
		}

		private void SetupListView()
		{
			listView = new ListView
			{
				selectionType = SelectionType.None,
				showBoundCollectionSize = false,
			};

			BindListView();

			listView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

			Add(listView);
		}

		private void BindListView()
		{
			mapComponent = MapCreatorUtilities.MapComponent as ArcGISMapComponent;

			if (!mapComponent)
			{
				return;
			}

			var serializedObject = new SerializedObject(mapComponent);
			var elevationSources = serializedObject.FindProperty("mapElevation").FindPropertyRelative("ElevationSources");
			elevationSourcesList = new SerializedPropertyList(elevationSources);

			listView.bindingPath = elevationSources.propertyPath;
			listView.Bind(serializedObject);

			listView.makeItem = () =>
			{
				var element = new VisualElement();

				elevationItemTemplate.CloneTree(element);

				var image = element.Q<Image>();

				image.scaleMode = ScaleMode.ScaleAndCrop;

				return element;
			};

			listView.bindItem = (element, index) =>
			{
				elevationSourcesList.Update();

				var image = element.Q<Image>();
				var label = element.Q<Label>();
				var textField = element.Q<TextField>();
				var toggle = element.Q<Toggle>();
				var toolbarMenu = element.Q<ToolbarMenu>();

				var itemSerializedProperty = elevationSourcesList.Get(elevationSourcesList.arraySize - 1 - index);
				var itemSerializedObject = itemSerializedProperty.serializedObject;

				element.userData = itemSerializedProperty;

				image.image = GetElevationThumbnail(itemSerializedProperty.FindPropertyRelative("Source").stringValue);

				image.TrackPropertyValue(itemSerializedProperty.FindPropertyRelative("Source"), action =>
				{
					var source = action.stringValue;

					if (source != null)
					{
						image.image = GetElevationThumbnail(action.stringValue);
					}
				});

				label.bindingPath = itemSerializedProperty.FindPropertyRelative("Name").propertyPath;

				textField.bindingPath = itemSerializedProperty.FindPropertyRelative("Name").propertyPath;

				toggle.bindingPath = itemSerializedProperty.FindPropertyRelative("IsEnabled").propertyPath;

				element.Bind(itemSerializedObject);

				textField.RegisterCallback<BlurEvent>(evnt =>
				{
					var centerVisualElement = element.Q<VisualElement>(className: "center");

					centerVisualElement.RemoveFromClassList("rename");
				});

				toggle.RegisterValueChangedCallback(evnt =>
				{
					UpdateItemStyle(element);

					mapComponent.UpdateElevation();

					OnEnableAllToggleValueChanged?.Invoke(GetEnableAllToggleValue());
				});

				toolbarMenu.menu.AppendAction("Move up", action =>
				{
					elevationSourcesList.Update();

					if (index < 1)
					{
						return;
					}

					elevationSourcesList.Move(elevationSourcesList.arraySize - 1 - index, elevationSourcesList.arraySize - 1 - index + 1);

					elevationSourcesList.Apply();

					mapComponent.UpdateElevation();

					Rebuild();
				}, action =>
				{
					return index < 1 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal;
				});

				toolbarMenu.menu.AppendAction("Move down", action =>
				{
					elevationSourcesList.Update();

					if (index > elevationSourcesList.arraySize - 2)
					{
						return;
					}

					elevationSourcesList.Move(elevationSourcesList.arraySize - 1 - index, elevationSourcesList.arraySize - 1 - index - 1);

					elevationSourcesList.Apply();

					mapComponent.UpdateElevation();

					Rebuild();
				}, action =>
				{
					return index > elevationSourcesList.arraySize - 2 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal;
				});

				toolbarMenu.menu.AppendAction("Zoom to", action =>
				{
					elevationSourcesList.Update();

					var elevationSourceInstanceData = elevationSourcesList.Get(elevationSourcesList.arraySize - 1 - index).GetValue() as ArcGISElevationSourceInstanceData;

					if (elevationSourceInstanceData != null)
					{
						ZoomTo(elevationSourceInstanceData);
					}
				});

				toolbarMenu.menu.AppendAction("Rename", action =>
				{
					var centerVisualElement = element.Q<VisualElement>(className: "center");

					centerVisualElement.AddToClassList("rename");

					textField.value = label.text;

					textField.Focus();
				});

				toolbarMenu.menu.AppendAction("Remove", action =>
				{
					elevationSourcesList.Update();

					var source = elevationSourcesList.Get(elevationSourcesList.arraySize - 1 - index).FindPropertyRelative("Source").stringValue;

					if (IsDefaultElevation(source))
					{
						return;
					}

					elevationSourcesList.RemoveAt(elevationSourcesList.arraySize - 1 - index);

					elevationSourcesList.Apply();

					OnEnableAllToggleValueChanged?.Invoke(GetEnableAllToggleValue());

					mapComponent.UpdateElevation();
				}, action =>
				{
					elevationSourcesList.Update();

					var source = elevationSourcesList.Get(elevationSourcesList.arraySize - 1 - index).FindPropertyRelative("Source").stringValue;

					if (IsDefaultElevation(source))
					{
						return DropdownMenuAction.Status.Disabled;
					}

					return DropdownMenuAction.Status.Normal;
				});

				UpdateItemStyle(element);
			};

			listView.unbindItem = (element, index) =>
			{
				element.userData = null;
			};
		}

		public bool GetEnableAllToggleValue()
		{
			elevationSourcesList.Update();

			for (int i = 0; i < elevationSourcesList.arraySize; ++i)
			{
				if (!elevationSourcesList.Get(i).FindPropertyRelative("IsEnabled").boolValue)
				{
					return false;
				}
			}

			return true;
		}

		public void SetEnableAllToggleValue(bool newValue)
		{
			elevationSourcesList.Update();

			for (int i = 0; i < elevationSourcesList.arraySize; ++i)
			{
				elevationSourcesList.Get(i).FindPropertyRelative("IsEnabled").boolValue = newValue;
			}

			elevationSourcesList.Apply();

			mapComponent.UpdateElevation();
		}

		private async void ZoomTo(ArcGISElevationSourceInstanceData elevationSource)
		{
			var EditorCameraComponent = mapComponent.GetComponentInChildren<ArcGISEditorCameraComponent>();

			if (!EditorCameraComponent || !EditorCameraComponent.enabled)
			{
				return;
			}

			bool success = await mapComponent.ZoomToElevationSource(EditorCameraComponent.gameObject, elevationSource);

			if (!success)
			{
				return;
			}

			var hP = EditorCameraComponent.GetComponent<HPTransform>();
			SceneView.lastActiveSceneView.pivot = new Vector3(0, 0, 0);
			SceneView.lastActiveSceneView.LookAt(new Vector3(0, 0, 0), Quaternion.Euler(90, 0, 0));
			mapComponent.UniversePosition = hP.UniversePosition;
			SceneView.lastActiveSceneView.Repaint();
		}

		private Texture GetElevationThumbnail(string source)
		{
			if (IsDefaultElevation(source))
			{
				return MapCreatorUtilities.Assets.LoadImage("MapCreator/ElevationTool/Default.png");
			}

			return MapCreatorUtilities.Assets.LoadImage("MapCreator/ElevationTool/Custom.png");
		}

		private void UpdateItemStyle(VisualElement visualElement)
		{
			var elevationItem = (SerializedProperty)visualElement.userData;

			if (elevationItem != null)
			{
				var isEnabledProperty = elevationItem.FindPropertyRelative("IsEnabled");

				if (isEnabledProperty == null || !isEnabledProperty.boolValue)
				{
					visualElement.hierarchy[0].AddToClassList("disabled");
				}
				else
				{
					visualElement.hierarchy[0].RemoveFromClassList("disabled");
				}
			}
		}

		private bool IsDefaultElevation(string url)
		{
			return url.ToLower() == ArcGISMapCreatorElevationWidget.DefaultElevationSourceData.Source.ToLower();
		}

		private bool IsMapComponentInvalid()
		{
			return !ReferenceEquals(mapComponent, MapCreatorUtilities.MapComponent);
		}
	}
}
