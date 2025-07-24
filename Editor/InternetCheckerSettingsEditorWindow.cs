#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace EasyAssets.InternetChecker.Editor
{
    using Runtime;
    using Scripts;

    public class InternetCheckerSettingsEditorWindow : EditorWindow
    {
        private InternetCheckerSettings settings;
        private SerializedObject serializedSettings;
        private string folderPath = "Assets/EasyAssets/Settings/InternetChecker";
        private string fileName = "InternetCheckerSettings.asset";

        [MenuItem("Tools/Network/Internet Checker Settings")]
        public static void ShowWindow()
        {
            GetWindow<InternetCheckerSettingsEditorWindow>("Internet Checker Settings");
        }

        private void OnEnable()
        {
            LoadOrCreateSettings();
        }

        private void LoadOrCreateSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:InternetCheckerSettings");
            if (guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                settings = AssetDatabase.LoadAssetAtPath<InternetCheckerSettings>(path);
            }
            else
            {
                EnsureFolderExists(folderPath);

                settings = CreateInstance<InternetCheckerSettings>();
                var assetPath = $"{folderPath}/{fileName}";
                AssetDatabase.CreateAsset(settings, assetPath);
                AssetDatabase.SaveAssets();
            }

            serializedSettings = new SerializedObject(settings);
        }

        private void OnGUI()
        {
            if (settings == null || serializedSettings == null)
            {
                LoadOrCreateSettings();
                return;
            }

            serializedSettings.Update();

            EditorGUILayout.LabelField("Internet Checker Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("method"));

            if (settings.method == CaptivePortalMethod.Custom)
            {
                EditorGUILayout.PropertyField(serializedSettings.FindProperty("customUrl"));
            }

            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Fallback Methods", EditorStyles.boldLabel);
            SerializedProperty fallbackList = serializedSettings.FindProperty("fallbackMethods");
            EditorGUILayout.PropertyField(fallbackList, true);

            EditorGUILayout.Space(5);
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("customUrl"));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("timeoutSeconds"));
            EditorGUILayout.PropertyField(serializedSettings.FindProperty("minIntervalSeconds"));

            serializedSettings.ApplyModifiedProperties();

            EditorGUILayout.Space(10);
            if (GUILayout.Button("Test Internet Connection"))
            {
                _ = InternetCheckManager.Instance.IsInternetAvailable().ContinueWith(task =>
                {
                    Debug.Log("Internet Available: " + task.Result);
                });
            }
        }

        private static void EnsureFolderExists(string fullPath)
        {
            if (AssetDatabase.IsValidFolder(fullPath)) return;

            string[] parts = fullPath.Split('/');
            string currentPath = parts[0]; // thường là "Assets"

            for (int i = 1; i < parts.Length; i++)
            {
                string nextPath = $"{currentPath}/{parts[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, parts[i]);
                }

                currentPath = nextPath;
            }
        }
    }
}
#endif