using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;

namespace GambitUtils
{
    public class FindMissingScriptsEditor : Editor
    {
        private const string MissingScripts = "FindMissingScriptsEditor";
        private const string MissingAny = "FindAnyMissingEditor";

        private const string SCENES_FOLDER_PATH = "GameplayAssets/Scenes/Production";

        private const string MENU_ITEM_PATH = "Tools/Find Missing Scripts";

        [MenuItem(MENU_ITEM_PATH + "/Find Missing Scripts In Active Scene")]
        public static void OnFindMissingScriptsInActiveSceneClick()
        {
            FindMissingScripts(false, false, true, false);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Any Missing In Active Scene")]
        public static void OnFindAnyMissingInActiveSceneClick()
        {
            FindMissingScripts(false, false, true, true);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Missing Scripts In Scenes")]
        public static void OnFindMissingScriptsInScenesClick()
        {
            FindMissingScripts(true, false, false, false);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Any Missing In Scenes")]
        public static void OnFindAnyMissingInScenesClick()
        {
            FindMissingScripts(true, false, false, true);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Missing Scripts In Prefabs")]
        public static void OnFindMissingScriptsInPrefabsClick()
        {
            FindMissingScripts(false, true, false, false);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Any Missing In Prefabs")]
        public static void OnFindAnyMissingInPrefabsClick()
        {
            FindMissingScripts(false, true, false, true);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Missing Scripts In Project")]
        public static void OnFindMissingScriptsInProjectClick()
        {
            FindMissingScripts(true, true, false, false);
        }

        [MenuItem(MENU_ITEM_PATH + "/Find Any Missing In Project")]
        public static void OnFindAnyMissingInProjectClick()
        {
            FindMissingScripts(true, true, false, true);
        }

        [MenuItem(MENU_ITEM_PATH + "/Clear Progress Bar")]
        public static void ClearProgressBar()
        {
            EditorUtility.ClearProgressBar();
        }

        private static bool LogSearchResult(string missingCountString, string noMissingFoundString, out string messageLine, int missingCount, bool any)
        {
            var text = $"{(any ? MissingAny : MissingScripts)}: ";
            if (missingCount > 0)
            {
                messageLine = string.Format(missingCountString, missingCount);
                Debug.LogWarning(text + messageLine);
            }
            else
            {
                messageLine = noMissingFoundString;
                Debug.Log(text + messageLine);
            }
            return missingCount > 0;
        }

        private static void FindMissingScripts(bool searchScenes, bool searchPrefabs, bool searchActiveScene, bool any)
        {
            string message = "";
            string messageLine;
            bool missingScriptsFound = false;
            if (searchScenes)
            {
                int missingCount = 0;
                string scenesSuffix = (EditorApplication.isPlaying ? "current scene." : "scenes from Assets/" + SCENES_FOLDER_PATH + ".");
                FindMissingScriptsInScenes(ref missingCount, any);
                LogSearchResult("Found {0} objects with missing scripts in " + scenesSuffix, "No missing scripts in " + scenesSuffix, out messageLine, missingCount, any);
                missingScriptsFound |= missingCount > 0;
                message += messageLine + "\n";
            }
            if (searchPrefabs)
            {
                int missingCount = 0;
                FindMissingScriptsInPrefabs(ref missingCount, any);
                LogSearchResult("Found {0} prefabs with missing scripts in Assets.", "No missing scripts in prefabs.", out messageLine, missingCount, any);
                missingScriptsFound |= missingCount > 0;
                message += messageLine + "\n";
            }
            if (searchActiveScene)
            {
                int missingCount = 0;
                string scenesSuffix = (EditorApplication.isPlaying ? "current scene." : "scenes from Assets/" + SCENES_FOLDER_PATH + ".");
                FindMissingScriptsInActiveScene(ref missingCount, any);
                LogSearchResult("Found {0} objects with missing scripts in " + scenesSuffix, "No missing scripts in " + scenesSuffix, out messageLine, missingCount, any);
                missingScriptsFound |= missingCount > 0;
                message += messageLine + "\n";
            }
            if (missingScriptsFound)
            {
                message += "\nPress OK to see the full list in Console window";
            }

            GC.Collect();

            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog("Find Missing Scripts", message, "OK");
        }

        private static void FindMissingScriptsInScene(Scene scene, ref int missingCount, bool any)
        {
            if (!scene.IsValid())
                return;
            List<GameObject> rootGameObjects = new List<GameObject>();
            scene.GetRootGameObjects(rootGameObjects);
            for (int i = 0; i < rootGameObjects.Count; i++)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Processing Scene " + scene.name + " object: " + i + "/" + rootGameObjects.Count, rootGameObjects[i].name, (float)i / (float)rootGameObjects.Count))
                    break;
                FindMissingComponentsInGameObject(rootGameObjects[i], "(Scene: " + scene.name + ")", ref missingCount, any);
            }
        }

        private static void FindMissingScriptsInActiveScene(ref int missingCount, bool any)
        {
            FindMissingScriptsInScene(EditorSceneManager.GetActiveScene(), ref missingCount, any);
        }

        private static void FindMissingScriptsInScenes(ref int missingCount, bool any)
        {
            EditorUtility.DisplayProgressBar("Searching scenes", "", 0.0f);

            if (EditorApplication.isPlaying)
            {
                FindMissingScriptsInScene(EditorSceneManager.GetActiveScene(), ref missingCount, any);
                FindMissingScriptsInScene(SceneUtils.GetDontDestroyOnLoadScene(), ref missingCount, any);
            }
            else
            {
                string currentScenePath = EditorSceneManager.GetActiveScene().path;

                string[] scenePaths = Directory.GetFiles(Path.Combine(Application.dataPath, SCENES_FOLDER_PATH), "*.unity", SearchOption.AllDirectories);
                foreach (var scenePath in scenePaths)
                {
                    Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
                    FindMissingScriptsInScene(scene, ref missingCount, any);
                }
                EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
            }
        }

        private static void FindMissingScriptsInPrefabs(ref int missingCount, bool any)
        {
            EditorUtility.DisplayProgressBar("Searching Prefabs", "", 0.0f);

            string[] files = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
            EditorUtility.DisplayCancelableProgressBar("Searching Prefabs", "Found " + files.Length + " prefabs", 0.0f);

            for (int i = 0; i < files.Length; i++)
            {
                string prefabPath = files[i].Replace(Application.dataPath, "Assets");
                if (EditorUtility.DisplayCancelableProgressBar("Processing Prefabs " + i + "/" + files.Length, prefabPath, (float)i / (float)files.Length))
                    break;

                GameObject gameObject = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)) as GameObject;

                if (gameObject != null)
                {
                    FindMissingComponentsInGameObject(gameObject, "(Prefab)", ref missingCount, any);
                    EditorUtility.UnloadUnusedAssetsImmediate(true);
                }
            }

            EditorUtility.DisplayProgressBar("Cleanup", "Cleaning up", 1.0f);

            EditorUtility.UnloadUnusedAssetsImmediate(true);
        }

        private static void FindMissingComponentsInGameObject(GameObject gameObject, string suffix, ref int missingCount, bool any)
        {
            var text = $"{(any ? MissingAny : MissingScripts)}: ";
            Component[] components = gameObject.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missingCount++;
                    Transform t = gameObject.transform;

                    string componentPath = gameObject.name;
                    while (t.parent != null)
                    {
                        componentPath = t.parent.name + "/" + componentPath;
                        t = t.parent;
                    }
                    Debug.LogWarning($"{text}The referenced script on this Behaviour (" + componentPath + ") is missing! " + suffix, gameObject);
                }
                else if (any)
                {
                    using (var so = new SerializedObject(components[i]))
                    {
                        using (var prop = so.GetIterator())
                        {
                            do
                            {
                                if (prop.propertyType == SerializedPropertyType.ObjectReference && prop.objectReferenceInstanceIDValue != 0 && prop.objectReferenceValue == null)
                                {
                                    missingCount++;
                                    Debug.LogWarning($"{text}In script: {components[i].GetType().Name}, variable {prop.displayName}({prop.propertyPath}) is missing! " + suffix, components[i]);
                                }
                            }
                            while (prop.NextVisible(true));
                        }
                    }
                }
            }

            foreach (Transform child in gameObject.transform)
            {
                FindMissingComponentsInGameObject(child.gameObject, suffix, ref missingCount, any);
            }
        }
    }
}
