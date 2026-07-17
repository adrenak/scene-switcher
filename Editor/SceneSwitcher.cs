using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Adrenak.SceneSwitcher {
    [InitializeOnLoad]
    static class SceneSwitcher {
        const EventModifiers ModifierMask =
            EventModifiers.Control | EventModifiers.Alt | EventModifiers.Shift | EventModifiers.Command;

        const string PlayModeTitle = "Cannot Switch Scenes";
        const string PlayModeMessage = "Exit Play Mode before switching scenes with the Scene Switcher.";

        static SceneSwitcher() => SceneView.duringSceneGui += _ => TryHandleEvent(Event.current);

        public static bool TryOpenScene(SceneAsset scene) => TryOpenScene(SceneSwitcherSettings.PathOf(scene));

        public static bool TryOpenScene(string scenePath) {
            if (string.IsNullOrEmpty(scenePath)) {
                Debug.LogWarning("[Scene Switcher] Scene path is empty.");
                return false;
            }

            if (!File.Exists(scenePath)) {
                Debug.LogWarning($"[Scene Switcher] Scene not found at '{scenePath}'.");
                return false;
            }

            if (IsActiveScene(scenePath))
                return true;

            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) {
                EditorUtility.DisplayDialog(PlayModeTitle, PlayModeMessage, "OK");
                return false;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return false;

            EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            return true;
        }

        public static bool IsActiveScene(SceneAsset scene) => IsActiveScene(SceneSwitcherSettings.PathOf(scene));

        public static bool IsActiveScene(string scenePath) {
            var activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid())
                return false;

            return string.Equals(
                Path.GetFullPath(activeScene.path),
                Path.GetFullPath(scenePath),
                System.StringComparison.OrdinalIgnoreCase);
        }

        static void TryHandleEvent(Event evt) {
            if (evt == null)
                return;

            var settings = SceneSwitcherSettings.LoadOrCreate();

            if (!MatchesTrigger(evt, settings.TriggerModifiers, settings.TriggerMouseButton))
                return;

            evt.Use();
            ShowMenu(settings);
        }

        static void ShowMenu(SceneSwitcherSettings settings) {
            var menu = new GenericMenu();
            var hasScenes = false;

            foreach (var scene in settings.Scenes) {
                hasScenes = true;
                AddSceneItem(menu, scene);
            }

            if (hasScenes)
                menu.AddSeparator("");

            menu.AddItem(new GUIContent("Settings"), false, SceneSwitcherWindow.Open);
            menu.ShowAsContext();
        }

        static void AddSceneItem(GenericMenu menu, SceneAsset scene) {
            if (!SceneSwitcherSettings.IsValid(scene)) {
                menu.AddDisabledItem(new GUIContent($"{SceneSwitcherSettings.NameOf(scene)} (Missing)"));
                return;
            }

            if (IsActiveScene(scene)) {
                menu.AddDisabledItem(new GUIContent($"✓ {SceneSwitcherSettings.NameOf(scene)}"));
                return;
            }

            menu.AddItem(
                new GUIContent(SceneSwitcherSettings.NameOf(scene)),
                false,
                () => TryOpenScene(scene));
        }

        static bool MatchesTrigger(Event evt, EventModifiers requiredModifiers, int mouseButton) {
            if (evt.type != EventType.MouseDown && evt.type != EventType.ContextClick)
                return false;

            if (evt.type == EventType.MouseDown && evt.button != mouseButton)
                return false;

            var required = requiredModifiers & ModifierMask;
            return (evt.modifiers & required) == required;
        }
    }
}
