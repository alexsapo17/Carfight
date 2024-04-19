using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement; // Aggiungi questo namespace

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioMixer audioMixer; // Assicurati di collegare l'AudioMixer nell'Inspector

    // Rimuovi i riferimenti pubblici e trovali in modo dinamico
    private Slider musicVolumeSlider;
    private Slider sfxVolumeSlider;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Aggiungi il listener per il caricamento delle scene
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
    }
void Start() {
    LoadVolumeSettings();
}
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Trova gli slider nella scena appena caricata
        FindSlidersInScene();
  
    }

 void FindSlidersInScene()
{
    // Trova gli slider per nome, tag, o un altro metodo che funzioni per il tuo gioco
    musicVolumeSlider = GameObject.Find("MusicVolumeSlider")?.GetComponent<Slider>();
    sfxVolumeSlider = GameObject.Find("SFXVolumeSlider")?.GetComponent<Slider>();

    // Imposta i listener per i nuovi slider trovati senza impostare il valore qui
    if (musicVolumeSlider != null)
    {
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
    }
    if (sfxVolumeSlider != null)
    {
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }
          // Aggiorna i valori degli slider in base alle impostazioni salvate
        LoadVolumeSettings();
}


    void OnDestroy()
    {
        // Ricorda di rimuovere il listener quando l'oggetto viene distrutto
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

void LoadVolumeSettings()
{
    // Recupera i valori salvati
    float musicVolume = PlayerPrefs.GetFloat("MusicVolume");
    float sfxVolume = PlayerPrefs.GetFloat("SFXVolume");

    // Verifica se i valori sono già stati impostati, altrimenti imposta i valori predefiniti
    if (musicVolume == 0 && sfxVolume == 0 && !PlayerPrefs.HasKey("VolumeInitialized"))
    {
        musicVolume = 0.3f; // Imposta il volume della musica al 30%
        sfxVolume = 0.5f;   // Imposta il volume degli effetti sonori al 50%

        // Applica i valori predefiniti all'AudioMixer
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);

        // Salva i nuovi valori
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

        // Imposta una chiave per indicare che i valori predefiniti sono stati inizializzati
        PlayerPrefs.SetInt("VolumeInitialized", 1);
    }
    else
    {
        // Applica i valori salvati all'AudioMixer
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
    }

    // Assicurati anche di aggiornare gli slider, se non sono già stati aggiornati
    if (musicVolumeSlider != null)
    {
        musicVolumeSlider.value = musicVolume;
    }
    if (sfxVolumeSlider != null)
    {
        sfxVolumeSlider.value = sfxVolume;
    }
}


public void SetMusicVolume(float volume)
{
    if (volume > 0)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
    else
    {
        // Imposta il volume a -80dB quando lo slider è a 0 per silenziare l'audio
        audioMixer.SetFloat("MusicVolume", -80);
    }
    PlayerPrefs.SetFloat("MusicVolume", volume);
}

public void SetSFXVolume(float volume)
{
    if (volume > 0)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
    else
    {
        // Imposta il volume a -80dB quando lo slider è a 0 per silenziare l'audio
        audioMixer.SetFloat("SFXVolume", -80);
    }
    PlayerPrefs.SetFloat("SFXVolume", volume);
}

}
