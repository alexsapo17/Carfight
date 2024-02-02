using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using Firebase.Auth;
public class DailyRewardButton : MonoBehaviour
{
    public Button dailyRewardButton;
    private Text countdownText; // Modifica per trovare automaticamente il componente
    private CurrencyManager currencyManager; // Modifica per trovare automaticamente il componente
    private DatabaseReference databaseReference;
    private const int RewardAmount = 100;
    private string userId;
    private DateTime? nextClaimTime;
 private Animator buttonAnimator;
    void Start()
    {
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        countdownText = transform.Find("TimerText").GetComponent<Text>(); // Trova il componente Text automaticamente
        currencyManager = FindObjectOfType<CurrencyManager>(); // Trova il CurrencyManager nella scena
         buttonAnimator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();

        UpdateButtonStateAsync();
    }

    public async void ClaimReward()
    {
        if (currencyManager == null) return;

        currencyManager.ModifyCoins(RewardAmount);
        await SaveLastClaimedTimeAsync(DateTime.UtcNow);
        await UpdateButtonStateAsync();
    }

    private async Task SaveLastClaimedTimeAsync(DateTime time)
    {
        string timeString = time.ToUniversalTime().ToString("o");
        await databaseReference.Child("users").Child(userId).Child("lastClaimedDailyReward").SetValueAsync(timeString);
    }

    private async Task UpdateButtonStateAsync()
    {
        DateTime? lastClaimed = await GetLastClaimedTimeAsync();

        if (lastClaimed.HasValue)
        {
            nextClaimTime = lastClaimed.Value.Add(TimeSpan.FromDays(1));
            TimeSpan timeUntilNextClaim = nextClaimTime.Value - DateTime.UtcNow;

            if (timeUntilNextClaim > TimeSpan.Zero)
            {
                dailyRewardButton.interactable = false;
                dailyRewardButton.GetComponent<Image>().color = Color.gray;
                countdownText.text = FormatTimeSpan(timeUntilNextClaim);
                buttonAnimator.SetBool("IsAvailable", false); // Disattiva l'animazione
            }
            else
            {
                dailyRewardButton.interactable = true;
                dailyRewardButton.GetComponent<Image>().color = Color.yellow;
                countdownText.text = "Claim Now!";
                buttonAnimator.SetBool("IsAvailable", true); // Attiva l'animazione
            }
        }
        else
        {
            dailyRewardButton.interactable = true;
            dailyRewardButton.GetComponent<Image>().color = Color.yellow;
            countdownText.text = "Claim Now!";
            buttonAnimator.SetBool("IsAvailable", true); // Assicura che l'animazione sia attiva
        }
    }

    void Update()
    {
        if (!dailyRewardButton.interactable && nextClaimTime.HasValue)
        {
            TimeSpan timeUntilNextClaim = nextClaimTime.Value - DateTime.UtcNow;
            if (timeUntilNextClaim <= TimeSpan.Zero)
            {
                dailyRewardButton.interactable = true;
                dailyRewardButton.GetComponent<Image>().color = Color.yellow;
                countdownText.text = "Claim Now!";
                UpdateButtonStateAsync(); // Aggiorna lo stato del pulsante
            }
            else
            {
                countdownText.text = FormatTimeSpan(timeUntilNextClaim);
            }
        }
    }

    private async Task<DateTime?> GetLastClaimedTimeAsync()
    {
        var dataSnapshot = await databaseReference.Child("users").Child(userId).Child("lastClaimedDailyReward").GetValueAsync();
        if (dataSnapshot.Exists && DateTime.TryParse(dataSnapshot.Value.ToString(), out DateTime lastClaimed))
        {
            return lastClaimed.ToUniversalTime();
        }
        return null;
    }

    private string FormatTimeSpan(TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
}