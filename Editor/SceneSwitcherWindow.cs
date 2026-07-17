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

            EditorGUILayout.PropertyField(_triggerProperty, new GUIContent("Scene View trigger"));

            EditorGUILayout.Space(6f);
            if (_scenesProperty.arraySize == 0) {
                var buildSceneCount = SceneSwitcherSettings.GetBuildSettingsScenes().Count;
                EditorGUILayout.HelpBox(
                    buildSceneCount > 0
                        ? $"No scenes configured. Scene View menu uses {buildSceneCount} enabled scene(s) from Build Settings."
                        : "No scenes configured and Build Settings has no enabled scenes.",
                    MessageType.Info);

                using (new EditorGUI.DisabledScope(buildSceneCount == 0)) {
                    if (GUILayout.Button("Import from build settings"))
                        ImportFromBuildSettings();
                }
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
            EditorGUILayout.PropertyField(_scenesProperty, new GUIContent("Scenes"), true);
            EditorGUILayout.EndScrollView();

            if (_serializedSettings.ApplyModifiedProperties())
                _settings.MarkDirty();
        }

        void ImportFromBuildSettings() {
            _settings.ImportFromBuildSettings();
            _serializedSettings.Update();
            Repaint();
        }
    }
}
