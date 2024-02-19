using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using Firebase.Auth;

public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] Button _showAdButton;
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
     [SerializeField] Text countdownText;
    private string _adUnitId = null;
    private CurrencyManager currencyManager;
    private DatabaseReference databaseReference;
    private string userId;
    private DateTime? nextClaimTime;
    private TimeSpan rewardCooldown = TimeSpan.FromMinutes(20);

    void Awake()
    {
#if UNITY_IOS 
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        _showAdButton.interactable = false;
        currencyManager = FindObjectOfType<CurrencyManager>();
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        UpdateButtonStateAsync();
    }

    public async void LoadAd()
    {
        Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);
        if (adUnitId.Equals(_adUnitId))
        {
            _showAdButton.onClick.AddListener(ShowAd);
            _showAdButton.interactable = true;
        }
    }

    public void ShowAd()
    {
        _showAdButton.interactable = false;
        Advertisement.Show(_adUnitId, this);
    }

    public async void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            if (currencyManager != null)
            {
                currencyManager.ModifyCoins(200);
                await SaveLastClaimedTimeAsync(DateTime.UtcNow);
            }
            else
            {
                Debug.Log("CurrencyManager non trovato per aggiungere premio Ad");
            }
        }
    }

    private async Task SaveLastClaimedTimeAsync(DateTime time)
    {
        string timeString = time.ToUniversalTime().ToString("o");
        await databaseReference.Child("users").Child(userId).Child("lastClaimedReward").SetValueAsync(timeString);
        await UpdateButtonStateAsync();
    }

    private async Task UpdateButtonStateAsync()
    {
        DateTime? lastClaimed = await GetLastClaimedTimeAsync();

        if (lastClaimed.HasValue)
        {
            nextClaimTime = lastClaimed.Value.Add(rewardCooldown);
            TimeSpan timeUntilNextClaim = nextClaimTime.Value - DateTime.UtcNow;

            if (timeUntilNextClaim > TimeSpan.Zero)
            {
                _showAdButton.interactable = false;
            }
            else
            {
                _showAdButton.interactable = true;
            }
        }
        else
        {
            _showAdButton.interactable = true;
        }
    }
void Update()
{
    // Tentativo di riassegnare le referenze se sono null e ci si trova nella scena corretta.
    if (countdownText == null || _showAdButton == null)
    {
        TryAssignReferences();
    }

    if (nextClaimTime.HasValue)
    {
        TimeSpan timeUntilNextClaim = nextClaimTime.Value - DateTime.UtcNow;
        if (countdownText != null && _showAdButton != null)
        {
            if (timeUntilNextClaim <= TimeSpan.Zero)
            {
                _showAdButton.interactable = true;
                countdownText.text = "Ready!";
                UpdateButtonStateAsync();
            }
            else
            {
                _showAdButton.interactable = false;
                countdownText.text = FormatTimeSpan(timeUntilNextClaim);
            }
        }
    }
}

void TryAssignReferences()
{
    // Verifica se gli oggetti esistono prima di tentare di assegnare le referenze.
    GameObject countdownTextObject = GameObject.Find("CountdownText");
    if (countdownTextObject != null && countdownText == null)
    {
        countdownText = countdownTextObject.GetComponent<Text>();
    }

    GameObject showAdButtonObject = GameObject.Find("ButtonAdsReward");
    if (showAdButtonObject != null && _showAdButton == null)
    {
        _showAdButton = showAdButtonObject.GetComponent<Button>();
    }
}



    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
    private async Task<DateTime?> GetLastClaimedTimeAsync()
    {
        var dataSnapshot = await databaseReference.Child("users").Child(userId).Child("lastClaimedReward").GetValueAsync();
        if (dataSnapshot.Exists && DateTime.TryParse(dataSnapshot.Value.ToString(), out DateTime lastClaimed))
        {
            return lastClaimed.ToUniversalTime();
        }
        return null;
    }

public void OnUnityAdsShowStart(string adUnitId) 
{
    // Logica opzionale da eseguire all'inizio della visualizzazione dell'annuncio
    Debug.Log("Annuncio iniziato: " + adUnitId);
}

public void OnUnityAdsShowClick(string adUnitId)
{
    // Logica opzionale da eseguire quando l'annuncio viene cliccato
    Debug.Log("Annuncio cliccato: " + adUnitId);
}

public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
{
    // Logica opzionale da eseguire in caso di fallimento nella visualizzazione dell'annuncio
    Debug.LogError($"Errore durante la mostra dell'annuncio {adUnitId}: {error.ToString()} - {message}");
}
    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }
 

    }