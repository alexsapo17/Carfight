using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Extensions;
public class ExperienceUIUpdater : MonoBehaviour
{
    public Slider experienceSlider; // Assegna questo nell'Inspector
    public Text levelText;         // Assegna questo nell'Inspector
    private DatabaseReference databaseReference;

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadExperienceAndLevel();
    }

    private void LoadExperienceAndLevel()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("experience").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading experience: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    int playerExperience = int.Parse(snapshot.Value.ToString());
                    int playerLevel = CalculateLevel(playerExperience);
                    UpdateExperienceUI(playerExperience, playerLevel);
                }
            }
        });
    }

private int CalculateLevel(int playerExperience)
{
    // Esempio di calcolo del livello
    int expThreshold = 10;
    int playerLevel = 1;
    while (playerExperience >= expThreshold)
    {
        playerLevel++;
        expThreshold += 12 + (playerLevel - 1) * 2; // Incrementa la soglia per il prossimo livello
    }

    return playerLevel;
}

private void UpdateExperienceUI(int playerExperience, int playerLevel)
{
    // Calcola la frazione dell'esperienza per il livello corrente
    int prevThreshold = (playerLevel - 1) * (10 + (playerLevel - 1) * 12) / 2;
    int nextThreshold = playerLevel * (10 + playerLevel * 12) / 2;
    float experienceFraction = (float)(playerExperience - prevThreshold) / (nextThreshold - prevThreshold);

    if (experienceSlider != null)
    {
        experienceSlider.value = experienceFraction;
    }

    if (levelText != null)
    {
        levelText.text = playerLevel.ToString();
    }
}
    public void ForceUpdateUI()
    {
        LoadExperienceAndLevel();
    }
}

