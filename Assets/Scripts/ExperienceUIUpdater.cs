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
                int playerExperience = 0;
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    playerExperience = int.Parse(snapshot.Value.ToString());
                }
                // Carica il livello dopo aver caricato l'esperienza
                LoadLevel(playerExperience, userId);
            }
        });
    }

    private void LoadLevel(int playerExperience, string userId)
    {
        databaseReference.Child("users").Child(userId).Child("level").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading level: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                int playerLevel = 1;
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    playerLevel = int.Parse(snapshot.Value.ToString());
                }
                UpdateExperienceUI(playerExperience, playerLevel);
            }
        });
    }

    public void UpdateExperienceUI(int playerExperience, int playerLevel)
    {
        int expForCurrentLevel = (playerLevel - 1) * 100 * playerLevel / 2;
        int expForNextLevel = playerLevel * 100 * (playerLevel + 1) / 2;
        float experienceFraction = (float)(playerExperience - expForCurrentLevel) / (expForNextLevel - expForCurrentLevel);

        if (experienceSlider != null)
        {
            experienceSlider.value = experienceFraction;
        }

        if (levelText != null)
        {
            levelText.text = $"{playerLevel}";
        }
    }
}
