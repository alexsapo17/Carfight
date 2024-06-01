using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System;
using System.Threading.Tasks;
using Firebase.Auth;

public class DailyRewardButton : MonoBehaviour
{
    public Button dailyRewardButton;
    private Text countdownText;
    private CurrencyManager currencyManager;
    private DatabaseReference databaseReference;
    private const int RewardAmount = 500;
    private const int GemsRewardAmount = 5;
    private string userId;
    private DateTime? nextClaimTime;
    private Animator buttonAnimator;

    async void Start() // Modifica Start in async
    {
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        countdownText = transform.Find("TimerText").GetComponent<Text>();
        currencyManager = FindObjectOfType<CurrencyManager>();
        buttonAnimator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();

        await UpdateButtonStateAsync(); // Utilizza await per attendere il completamento
    }

    public async void ClaimReward() // Modifica ClaimReward in async
    {
        if (currencyManager == null) return;
        dailyRewardButton.interactable = false;

        currencyManager.ModifyCoins(RewardAmount);
        currencyManager.ModifyGems(GemsRewardAmount);
        await SaveLastClaimedTimeAsync(DateTime.UtcNow); // Utilizza await per attendere il completamento
        await UpdateButtonStateAsync(); // Utilizza await per attendere il completamento
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
                buttonAnimator.SetBool("IsAvailable", false);
            }
            else
            {
                dailyRewardButton.interactable = true;
                dailyRewardButton.GetComponent<Image>().color = Color.yellow;
                countdownText.text = "Claim Now!";
                buttonAnimator.SetBool("IsAvailable", true);
            }
        }
        else
        {
            dailyRewardButton.interactable = true;
            dailyRewardButton.GetComponent<Image>().color = Color.yellow;
            countdownText.text = "Claim Now!";
            buttonAnimator.SetBool("IsAvailable", true);
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
                _ = UpdateButtonStateAsync(); // Utilizza await per attendere il completamento, ma ignora il Task restituito
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
