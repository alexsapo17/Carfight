using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabAdder : EditorWindow
{
    private GameObject prefabToAdd;

    [MenuItem("Tools/Prefab Adder")]
    public static void ShowWindow()
    {
        GetWindow<PrefabAdder>("Prefab Adder");
    }

    void OnGUI()
    {
        GUILayout.Label("Select Prefab to Add", EditorStyles.boldLabel);
        prefabToAdd = (GameObject)EditorGUILayout.ObjectField("Prefab", prefabToAdd, typeof(GameObject), false);

        if (GUILayout.Button("Add Prefab to Levels"))
        {
            AddPrefabToAllLevels();
        }
    }

    private void AddPrefabToAllLevels()
    {
        string[] allLevelPaths = Directory.GetFiles(Application.dataPath, "level*.prefab", SearchOption.AllDirectories);

        foreach (string levelPath in allLevelPaths)
        {
            string relativePath = "Assets" + levelPath.Substring(Application.dataPath.Length);
            GameObject levelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);

            if (levelPrefab != null && prefabToAdd != null)
            {
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(levelPrefab) as GameObject;

                GameObject childPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToAdd);
                childPrefabInstance.transform.SetParent(prefabInstance.transform, false);

                PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.AutomatedAction);
                DestroyImmediate(prefabInstance); // Clean up
            }
        }
    }
}
