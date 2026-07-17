using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Adrenak.SceneSwitcher {
    public class SceneSwitcherWindow : EditorWindow {
        const string WindowTitle = "Scene Switcher";

        SceneSwitcherSettings _settings;
        SerializedObject _serializedSettings;
        SerializedProperty _triggerProperty;
        SerializedProperty _scenesProperty;
        Vector2 _scrollPosition;

        [MenuItem("Tools/Scene Switcher", false, 150)]
        public static void Open() {
            var window = GetWindow<SceneSwitcherWindow>();
            window.titleContent = new GUIContent(WindowTitle);
            window.minSize = new Vector2(460f, 560f);
            window.Show();
        }

        void OnEnable() {
            _settings = SceneSwitcherSettings.LoadOrCreate();
            _serializedSettings = new SerializedObject(_settings);
            _triggerProperty = _serializedSettings.FindProperty("trigger");
            _scenesProperty = _serializedSettings.FindProperty("scenes");
        }

        void OnGUI() {
            _serializedSettings.Update();

            if (GUILayout.Button("Ping Settings Asset", GUILayout.Width(160f)))
                EditorGUIUtility.PingObject(_settings);

            EditorGUILayout.Space(8f);

            EditorGUILayout.LabelField(WindowTitle, EditorStyles.boldLabel);
            DrawHelpDropBox();

            EditorGUILayout.PropertyField(_triggerProperty, new GUIContent("Scene View trigger"));

            EditorGUILayout.Space(6f);
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            EditorGUILayout.PropertyField(_scenesProperty, new GUIContent("Scenes"), true);
            EditorGUILayout.EndScrollView();

            if (_serializedSettings.ApplyModifiedProperties())
                _settings.MarkDirty();
        }

        void DrawHelpDropBox() {
            const string message =
                "Drag scenes here, then use the trigger chord in the Scene View to open the scene menu.";
            const float minDropHeight = 56f;

            var content = new GUIContent(message);
            var textHeight = EditorStyles.helpBox.CalcHeight(content, EditorGUIUtility.currentViewWidth);
            var height = Mathf.Max(textHeight, minDropHeight);
            var dropRect = GUILayoutUtility.GetRect(content, EditorStyles.helpBox, GUILayout.ExpandWidth(true), GUILayout.Height(height));
            EditorGUI.HelpBox(dropRect, message, MessageType.Info);
            HandleDragAndDrop(dropRect);
        }

        void HandleDragAndDrop(Rect dropRect) {
            var evt = Event.current;
            if (!dropRect.Contains(evt.mousePosition))
                return;

            switch (evt.type) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!TryGetScenesFromDrag(out _)) {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        break;
                    }

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    if (evt.type == EventType.DragPerform) {
                        DragAndDrop.AcceptDrag();
                        AddScenesFromDrag();
                    }

                    evt.Use();
                    break;
            }
        }

        static bool TryGetScenesFromDrag(out List<SceneAsset> sceneAssets) {
            sceneAssets = new List<SceneAsset>();
            foreach (var obj in DragAndDrop.objectReferences) {
                if (obj is SceneAsset scene)
                    sceneAssets.Add(scene);
            }

            return sceneAssets.Count > 0;
        }

        void AddScenesFromDrag() {
            if (!TryGetScenesFromDrag(out var sceneAssets))
                return;

            var addedAny = false;
            foreach (var scene in sceneAssets) {
                if (_settings.TryAddScene(scene))
                    addedAny = true;
            }

            if (!addedAny)
                return;

            _serializedSettings.Update();
            Repaint();
        }
    }
}
