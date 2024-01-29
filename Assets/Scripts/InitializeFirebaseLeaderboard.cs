using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Collections.Generic;

public class InitializeFirebaseLeaderboard : MonoBehaviour
{
    private DatabaseReference databaseReference;

    void Start()
    {
        InitializeFirebase();
                            Debug.Log("Initializefirebase chiamato");

    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Failed to initialize Firebase with {task.Exception}");
                return;
            }

            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            CreateLeaderboardStructureForUser();
        });
    }

    private void CreateLeaderboardStructureForUser()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

        // Se non conosci l'userId o vuoi creare la struttura per tutti gli utenti,
        // dovrai eseguire questa operazione per ciascun utente individualmente.

        int numberOfLevels = 10; // Numero di livelli nel gioco
        for (int i = 0; i < numberOfLevels; i++)
        {
            string leaderboardPath = $"users/{userId}/leaderboards/Level_{i}";
            // Creazione di un percorso vuoto per ogni livello nella leaderboard sotto l'utente
            databaseReference.Child(leaderboardPath).Child("topTimes").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"Failed to check leaderboard structure for level {i}: {task.Exception}");
                    // Qui puoi anche aggiungere la creazione della struttura se non esiste
                    CreateEmptyLeaderboardStructure(userId, i);
                }
                else
                {
                    Debug.Log($"Leaderboard structure exists for level {i} for user {userId}");
                }
            });
        }
    }

    private void CreateEmptyLeaderboardStructure(string userId, int level)
    {
        string leaderboardPath = $"users/{userId}/leaderboards/Level_{level}";
        // Creazione di un percorso vuoto per ogni livello nella leaderboard sotto l'utente
        databaseReference.Child(leaderboardPath).Child("topTimes").SetValueAsync(new Dictionary<string, object>())
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"Failed to create leaderboard structure for level {level}: {task.Exception}");
                }
                else
                {
                    Debug.Log($"Leaderboard structure created for level {level} for user {userId}");
                }
            });
    }
}
