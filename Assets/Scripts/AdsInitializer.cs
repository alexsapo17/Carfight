using UnityEngine;
using UnityEngine.Advertisements;
 
public class AdsInitializer : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] string _androidGameId;
    [SerializeField] string _iOSGameId;
    [SerializeField] bool _testMode = true;
    private string _gameId;
 
private static AdsInitializer _instance; // Dichiarazione del campo _instance

    public static AdsInitializer Instance { get { return _instance; } }

    void Awake()
    {
        // Assicurati che ci sia solo un'istanza di AdsInitializer
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject); // Distruggi il duplicato
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject); // Mantieni questo oggetto attivo tra le scene
        InitializeAds();
    }
    public void InitializeAds()
    {
    #if UNITY_IOS
            _gameId = _iOSGameId;
    #elif UNITY_ANDROID
            _gameId = _androidGameId;
    #elif UNITY_EDITOR
            _gameId = _androidGameId; //Only for testing the functionality in the Editor
    #endif
        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(_gameId, _testMode, this);
        }
    }

 
 public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        // Una volta che gli annunci sono stati inizializzati con successo, carica l'annuncio
        if (Advertisement.isInitialized)
        {
            // Ottieni l'istanza di RewardedAdsButton e chiama LoadAd()
            RewardedAdsButton adsButton = FindObjectOfType<RewardedAdsButton>();
            if (adsButton != null)
            {
                adsButton.LoadAd();
            }
            else
            {
                Debug.LogWarning("RewardedAdsButton non trovato per caricare l'annuncio.");
            }
        }
    }

 
    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }
}