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
        LoadExperienceAndLevel();
    }

    public void AddExperience(int amount)
    {
        playerExperience += amount;
        CalculateLevel();
        SaveExperienceAndLevel();
    }

private void LoadExperienceAndLevel()
{
    var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

    // Carica l'esperienza
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
                playerExperience = int.Parse(snapshot.Value.ToString());
            }
            else
            {
                playerExperience = 0; // Imposta l'esperienza a 0 solo se non esiste un valore salvato
            }
            UpdateUI();
        }
    });

    // Carica il livello
    databaseReference.Child("users").Child(userId).Child("level").GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Error loading level: " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (snapshot.Value != null)
            {
                playerLevel = int.Parse(snapshot.Value.ToString());
            }
            else
            {
                playerLevel = 1; // Imposta il livello a 1 solo se non esiste un valore salvato
            }
            UpdateUI();
        }
    });
}

private void UpdateUI()
{
    // Cerca ExperienceUIUpdater nella scena corrente e aggiorna l'UI
    ExperienceUIUpdater updater = FindObjectOfType<ExperienceUIUpdater>();
    if (updater != null)
    {
        updater.UpdateExperienceUI(playerExperience, playerLevel);
    }
}


    private void SaveExperienceAndLevel()
    {
        var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("experience").SetValueAsync(playerExperience);
        databaseReference.Child("users").Child(userId).Child("level").SetValueAsync(playerLevel);

        // Cerca ExperienceUIUpdater nella scena corrente e aggiorna l'UI
        ExperienceUIUpdater updater = FindObjectOfType<ExperienceUIUpdater>();
        if (updater != null)
        {
updater.UpdateExperienceUI(playerExperience, playerLevel);
        }
    }

private void CalculateLevel()
{
    int expForNextLevel = 100; // Esperienza iniziale richiesta per il prossimo livello
    int level = 1;
    while (playerExperience >= expForNextLevel)
    {
        level++;
        expForNextLevel += 100 * level; // Incrementa l'esperienza necessaria per il prossimo livello
    }

     if (playerLevel != level)
    {
        playerLevel = level;
        SaveExperienceAndLevel(); // Salva se il livello è cambiato

        // Mostra il pannello delle ricompense
        int coins = CalcolaMonetePerLivello(level); // Sostituisci con la tua logica
        int gems = CalcolaGemmePerLivello(level); // Sostituisci con la tua logica
        RewardPanelController rewardPanel = FindObjectOfType<RewardPanelController>();
        if (rewardPanel != null)
        {
            rewardPanel.ShowRewardPanel(coins, gems);
        }
    }

}

private int CalcolaMonetePerLivello(int level)
{
    // Logica per calcolare le monete in base al livello
    // Qui un esempio semplice, puoi modificare secondo le tue esigenze
    return 100 * level;
}

private int CalcolaGemmePerLivello(int level)
{
    // Logica per calcolare le gemme in base al livello
    // Qui un esempio semplice, puoi modificare secondo le tue esigenze
    return 10 * level;
}

}
