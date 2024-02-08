#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class TagAudioSourcesInAllScenes : EditorWindow
{
    [MenuItem("Tools/Tag AudioSources in All Scenes")]
    public static void ShowWindow()
    {
        GetWindow<TagAudioSourcesInAllScenes>("Tag AudioSources");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Tag All AudioSources as SFX"))
        {
            TagAllAudioSources();
        }
    }

    static void TagAllAudioSources()
    {
        // Mantieni traccia della scena correntemente aperta
        string originalScenePath = SceneManager.GetActiveScene().path;

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                // Carica la scena
                Scene openedScene = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                GameObject[] rootGameObjects = openedScene.GetRootGameObjects();
                foreach (GameObject rootGameObject in rootGameObjects)
                {
                    // Cerca ricorsivamente tutti gli AudioSource nei GameObject disattivati
                    TagAudioSourcesRecursively(rootGameObject);
                }

                // Salva le modifiche fatte alla scena
                EditorSceneManager.SaveScene(openedScene);
            }
        }

        // Riapri la scena originariamente aperta
        EditorSceneManager.OpenScene(originalScenePath);

        Debug.Log("Tutti gli AudioSource sono stati taggati come SFX nelle scene abilitate.");
    }

    static void TagAudioSourcesRecursively(GameObject gameObject)
    {
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource != null && gameObject.tag != "Music")
        {
            gameObject.tag = "SFX";
            EditorUtility.SetDirty(gameObject); // Marca l'oggetto come modificato
        }

        // Itera su tutti i figli del GameObject
        foreach (Transform child in gameObject.transform)
        {
            TagAudioSourcesRecursively(child.gameObject);
        }
    }
}
#endif
