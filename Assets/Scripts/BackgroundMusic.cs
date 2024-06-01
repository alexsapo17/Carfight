using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance = null;
    public static BackgroundMusic Instance => instance;

    public AudioSource[] gameSceneAudioSources;
    public AudioSource[] otherSceneAudioSources;

    private AudioSource currentAudioSource;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);

        // Aggiungi un listener per quando la scena cambia
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Metodo per gestire il cambio della scena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Interrompi la riproduzione del vecchio AudioSource, se presente
        if (currentAudioSource != null && currentAudioSource.isPlaying)
        {
            currentAudioSource.Stop();
        }

        // Scegli l'array di AudioSource corretto in base alla scena
        AudioSource[] audioSources = scene.name == "GameScene" ? gameSceneAudioSources : otherSceneAudioSources;

        // Scegli casualmente un AudioSource dall'array
        currentAudioSource = audioSources[Random.Range(0, audioSources.Length)];

        // Riproduci l'AudioSource scelto
        currentAudioSource.Play();
    }

    void OnDestroy()
    {
        // Rimuovi il listener per evitare errori se l'oggetto viene distrutto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
