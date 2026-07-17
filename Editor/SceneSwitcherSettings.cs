using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Adrenak.SceneSwitcher {
    public enum SceneSwitcherTrigger {
        [InspectorName("Ctrl+Alt+Right Click")]
        CtrlAltRightClick,

        [InspectorName("Ctrl+Shift+Right Click")]
        CtrlShiftRightClick,
    }

    public class SceneSwitcherSettings : ScriptableObject {
        public const string AssetPath = "Assets/Editor/SceneSwitcher/SceneSwitcherSettings.asset";

        [SerializeField] SceneSwitcherTrigger trigger = SceneSwitcherTrigger.CtrlAltRightClick;
        [SerializeField] List<SceneAsset> scenes = new List<SceneAsset>();

        public SceneSwitcherTrigger Trigger => trigger;
        public EventModifiers TriggerModifiers => trigger switch {
            SceneSwitcherTrigger.CtrlShiftRightClick => EventModifiers.Control | EventModifiers.Shift,
            _ => EventModifiers.Control | EventModifiers.Alt,
        };
        public int TriggerMouseButton => 1;
        public IReadOnlyList<SceneAsset> Scenes => scenes;
        public bool UsesBuildSettingsFallback => scenes.Count == 0;

        public IReadOnlyList<SceneAsset> GetMenuScenes() =>
            UsesBuildSettingsFallback ? GetBuildSettingsScenes() : scenes;

        public static List<SceneAsset> GetBuildSettingsScenes() {
            var result = new List<SceneAsset>();
            foreach (var buildScene in EditorBuildSettings.scenes) {
                if (!buildScene.enabled || string.IsNullOrEmpty(buildScene.path))
                    continue;

                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(buildScene.path);
                if (scene != null)
                    result.Add(scene);
            }

            return result;
        }

        public static SceneSwitcherSettings LoadOrCreate() {
            EnsureAssetFolderExists();

            var settings = AssetDatabase.LoadAssetAtPath<SceneSwitcherSettings>(AssetPath);
            if (settings != null)
                return settings;

            settings = CreateInstance<SceneSwitcherSettings>();
            AssetDatabase.CreateAsset(settings, AssetPath);
            AssetDatabase.SaveAssets();
            return settings;
        }

        public static string PathOf(SceneAsset scene) =>
            scene != null ? AssetDatabase.GetAssetPath(scene) : string.Empty;

        public static string NameOf(SceneAsset scene) =>
            scene != null ? scene.name : "(Missing Scene)";

        public static bool IsValid(SceneAsset scene) =>
            scene != null && !string.IsNullOrEmpty(PathOf(scene));

        public void ImportFromBuildSettings() {
            scenes.Clear();
            scenes.AddRange(GetBuildSettingsScenes());
            MarkDirty();
        }

        public void MarkDirty() {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        static void EnsureAssetFolderExists() {
            if (!AssetDatabase.IsValidFolder("Assets/Editor"))
                AssetDatabase.CreateFolder("Assets", "Editor");
            if (!AssetDatabase.IsValidFolder("Assets/Editor/SceneSwitcher"))
                AssetDatabase.CreateFolder("Assets/Editor", "SceneSwitcher");
        }
    }
}
