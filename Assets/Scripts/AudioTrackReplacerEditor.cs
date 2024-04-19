using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AudioTrackReplacerEditor : EditorWindow
{
    [MenuItem("Tools/Audio/Replace Audio Tracks")]
    public static void ShowAudioTrackReplacerWindow()
    {
        EditorWindow.GetWindow<AudioTrackReplacerEditor>("Replace Audio Tracks");
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
        Debug.Log("Inizio sostituzione tracce audio...");

        // Trova tutte le tracce audio nella cartella "Assets/Resources"
        AudioClip[] audioClips = Resources.LoadAll<AudioClip>("");

        // Trova tutti gli AudioSource nella scena, inclusi quelli disattivati
        List<AudioSource> allAudioSources = new List<AudioSource>();
        FindAudioSourcesRecursively(allAudioSources, GameObject.FindObjectsOfType<GameObject>());

        // Per ogni traccia audio trovata
        foreach (AudioClip clip in audioClips)
        {
            // Se il nome della traccia audio è "TUC"
            if (clip.name == "TUC")
            {
                // Sostituisci la traccia audio con "TinyTransiotions_Whoosh_Source_Tube_03"
                foreach (AudioSource audioSource in allAudioSources)
                {
                    // Se l'AudioSource utilizza la traccia audio "TUC"
                    if (audioSource.clip == clip)
                    {
                        // Carica e assegna la nuova traccia audio
                        AudioClip newClip = Resources.Load<AudioClip>("TinyTransiotions_Whoosh_Source_Tube_03");
                        if (newClip != null)
                        {
                            audioSource.clip = newClip;
                            EditorUtility.SetDirty(audioSource);
                            Debug.Log("Traccia audio 'TUC' sostituita con successo");
                        }
                        else
                        {
                            Debug.LogError("Impossibile trovare la nuova traccia audio 'ExtremeSTrap'.");
                        }
                    }
                }
            }
        }

        Debug.Log("Sostituzione tracce audio completata.");
    }

    private void FindAudioSourcesRecursively(List<AudioSource> audioSources, GameObject[] objects)
    {
        foreach (GameObject obj in objects)
        {
            if (obj == null) continue;

            AudioSource audioSource = obj.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSources.Add(audioSource);
            }

            FindAudioSourcesRecursively(audioSources, GetChildObjects(obj));
        }
    }

    private GameObject[] GetChildObjects(GameObject obj)
    {
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in obj.transform)
        {
            childObjects.Add(child.gameObject);
        }
        return childObjects.ToArray();
    }
}
