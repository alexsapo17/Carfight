#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AssignAudioSourcesToMixer : EditorWindow
{
    static AudioMixerGroup musicGroup, sfxGroup;

    [MenuItem("Tools/Assign AudioSource to Mixer Groups")]
    public static void ShowWindow()
    {
        GetWindow(typeof(AssignAudioSourcesToMixer));
    }

    void OnGUI()
    {
        GUILayout.Label("Assign all AudioSources to Mixer Groups", EditorStyles.boldLabel);
        musicGroup = EditorGUILayout.ObjectField("Music Group", musicGroup, typeof(AudioMixerGroup), false) as AudioMixerGroup;
        sfxGroup = EditorGUILayout.ObjectField("SFX Group", sfxGroup, typeof(AudioMixerGroup), false) as AudioMixerGroup;

        if (GUILayout.Button("Assign"))
        {
            AssignAudioSources();
        }
    }

    static void AssignAudioSources()
    {
        if (musicGroup == null || sfxGroup == null)
        {
            Debug.LogError("Please assign both music and SFX Audio Mixer Groups.");
            return;
        }

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                Scene currentScene = EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Single);
                AudioSource[] sources = Resources.FindObjectsOfTypeAll<AudioSource>() as AudioSource[];

                foreach (AudioSource source in sources)
                {
                    if (source.CompareTag("Music"))
                    {
                        source.outputAudioMixerGroup = musicGroup;
                    }
                    else if (source.CompareTag("SFX"))
                    {
                        source.outputAudioMixerGroup = sfxGroup;
                    }
                }

                EditorSceneManager.SaveScene(currentScene);
            }
        }

        Debug.Log("All AudioSources have been assigned to their respective Mixer Groups.");
    }
}
#endif
