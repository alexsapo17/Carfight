using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using UnityEngine.UI;
using Firebase.Extensions;
public class ExperienceManager : MonoBehaviour
{
    public static ExperienceManager Instance;

    private int playerExperience = 0;
    private int playerLevel = 1;

    private DatabaseReference databaseReference;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

   private void SaveExperience()
{
    var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("experience").SetValueAsync(playerExperience).ContinueWithOnMainThread(task =>
    {
        if (!task.IsFaulted)
        {
            // Cerca ExperienceUIUpdater nella scena corrente e aggiorna l'UI
            ExperienceUIUpdater updater = FindObjectOfType<ExperienceUIUpdater>();
            if (updater != null)
            {
                updater.ForceUpdateUI();
            }
        }
    });
}

    public void AddExperience(int amount)
    {
        playerExperience += amount;
        SaveExperience();
    }




}
