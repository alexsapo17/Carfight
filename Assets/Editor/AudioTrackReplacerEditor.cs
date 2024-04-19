using UnityEngine;
using UnityEditor;

public class AudioTrackReplacerEditor : EditorWindow
{
    [MenuItem("Tools/Replace Audio Tracks")]
    public static void ShowWindow()
    {
        GetWindow<AudioTrackReplacerEditor>("Replace Audio Tracks");
    }

    private void OnGUI()
    {
        GUILayout.Label("Premi il pulsante per sostituire le tracce audio:");
        if (GUILayout.Button("Sostituisci"))
        {
            ReplaceAudioTracks();
        }
    }

    private void ReplaceAudioTracks()
    {
        // Trova tutti gli oggetti nella scena
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // Trova tutti i componenti AudioSource nell'oggetto
            AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>();

            foreach (AudioSource audioSource in audioSources)
            {
                // Se il nome della traccia audio è "TUUC", sostituiscilo con "ExtremeSTrap"
                if (audioSource.clip != null && audioSource.clip.name == "TUUC")
                {
                    audioSource.clip = Resources.Load<AudioClip>("ExtremeSTrap");
                }
            }
        }
    }
}
