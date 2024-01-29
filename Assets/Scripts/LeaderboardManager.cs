using Firebase.Database;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardEntryPrefab; // Prefab per la voce della classifica
    [SerializeField] private Transform leaderboardContent; // Il contenitore per le voci della classifica

    // Struttura dati per mantenere i dati della classifica
    private class LeaderboardEntry
    {
        public string nickname;
        public float bestTime;
    }

    public void LoadLeaderboardForLevel(int levelId)
    {
        FirebaseDatabase.DefaultInstance
            .GetReference("levelLeaderboards")
            .Child("level" + levelId)
            .OrderByChild("bestTime")
            .LimitToFirst(5)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Errore nel caricamento della classifica per il livello " + levelId);
                }
                else if (task.IsCompleted)
                {
                    // Pulisci le vecchie voci della classifica
                    foreach (Transform child in leaderboardContent)
                    {
                        Destroy(child.gameObject);
                    }

                    DataSnapshot snapshot = task.Result;
                    // Converti i dati del punteggio in un elenco ordinato
                    var leaderboardEntries = snapshot.Children
                        .Select(child => JsonUtility.FromJson<LeaderboardEntry>(child.GetRawJsonValue()))
                        .OrderBy(entry => entry.bestTime)
                        .ToList();

                    foreach (var entry in leaderboardEntries)
                    {
                        // Crea e configura una nuova voce della classifica
                        GameObject newEntry = Instantiate(leaderboardEntryPrefab, leaderboardContent);
                        newEntry.GetComponentInChildren<Text>().text = $"{entry.nickname}: {entry.bestTime}";
                    }
                }
            });
    }
}
