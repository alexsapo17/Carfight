using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance = null;

    public static BackgroundMusic Instance
    {
        get { return instance; }
    }

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
        if (scene.name == "GameScene")
        {
            // Qui puoi fermare la musica, cambiarla o abbassarne il volume
            GetComponent<AudioSource>().Stop(); // Per fermare la musica
            // GetComponent<AudioSource>().Play(); // Se vuoi cambiare la traccia, imposta prima l'AudioClip
        }
        else
        {
            // Assicurati che la musica di sottofondo sia in riproduzione nelle altre scene
            if (!GetComponent<AudioSource>().isPlaying)
            {
                GetComponent<AudioSource>().Play();
            }
        }
    }

    void OnDestroy()
    {
        // Rimuovi il listener per evitare errori se l'oggetto viene distrutto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
