using UnityEngine;
using System.Collections;
public class SplashScreen : MonoBehaviour
{
    public GameObject splashScreenPanel; // Riferimento al pannello del tuo splash screen
    public float splashScreenDuration = 2f; // Durata del tuo splash screen in secondi
    private bool hasShownSplashScreen = false; // Flag per tenere traccia se il splash screen è già stato mostrato

    // Singleton pattern per mantenere l'oggetto tra le scene
    private static SplashScreen instance;
    public static SplashScreen Instance => instance;

    void Awake()
    {
        // Implementazione del Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Se un'altra istanza esiste già, distruggi questa
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Assicurati che l'oggetto persista tra le scene
    }

    void Start()
    {
        // Controlla se il flag è già impostato a true (splash screen già mostrato in precedenza durante la sessione)
        if (!hasShownSplashScreen)
        {
            // Mostra il pannello del tuo splash screen
            ShowSplashScreen();

            // Imposta il flag a true per indicare che il splash screen è stato mostrato
            hasShownSplashScreen = true;

            // Avvia una coroutine per nascondere il pannello dopo la durata specificata
            StartCoroutine(HideSplashScreenAfterDelay());
        }
        else
        {
splashScreenPanel.SetActive(false);    
    }
    }
    

    void ShowSplashScreen()
    {
        // Attiva il pannello del tuo splash screen
        splashScreenPanel.SetActive(true);
    }

    IEnumerator HideSplashScreenAfterDelay()
    {
        // Attendi per la durata specificata
        yield return new WaitForSeconds(splashScreenDuration);

        // Nascondi il pannello del tuo splash screen
        splashScreenPanel.SetActive(false);
    }
}
