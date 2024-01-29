using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Auth;
using System.Linq;
using System.Collections.Generic;

public class LeaderboardUIManager : MonoBehaviour
{
    public GameObject levelButtonPrefab; // Prefab per i pulsanti dei livelli
    public Transform levelButtonsContainer; // Contenitore per i pulsanti dei livelli
    public Text leaderboardText; // Testo dove mostrare la leaderboard
    public GameObject leaderboardPanel; // Pannello della leaderboard
    public GameObject scrollView; // Riferimento allo ScrollView dei pulsanti dei livelli
    public Button backButton;

    void Start()
    {
        InitializeLevelButtons();
        backButton.onClick.AddListener(HideLeaderboard); // Aggiungi il listener al pulsante Indietro
    }

    private void InitializeLevelButtons()
    {
        // Assumiamo che ci siano 10 livelli, ma puoi modificare secondo il tuo gioco
        for (int i = 0; i < 10; i++)
        {
            int levelId = i;
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonsContainer);
            buttonObj.GetComponentInChildren<Text>().text = $"Livello {levelId}";
            buttonObj.GetComponent<Button>().onClick.AddListener(() => LoadAndShowLeaderboard(levelId));
        }
    }

    public void LoadAndShowLeaderboard(int levelId)
{
    scrollView.SetActive(false); // Nasconde lo ScrollView
    leaderboardPanel.SetActive(true); // Mostra la leaderboard

    // Leggi i dati di tutti gli utenti per il livello specifico dal database
    FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Errore nel caricamento dei dati degli utenti.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string leaderboardDisplay = $"Top 5 Tempi - Livello {levelId}:\n";

                List<KeyValuePair<string, double>> topTimes = new List<KeyValuePair<string, double>>();

                foreach (DataSnapshot userSnapshot in snapshot.Children)
                {
                    if (userSnapshot.Child("levelsProgress").HasChild(levelId.ToString()))
                    {
                        double bestTime = double.Parse(userSnapshot.Child("levelsProgress").Child(levelId.ToString()).Child("bestTime").Value.ToString());
                        string nickname = userSnapshot.Child("nickname").Value.ToString(); // Prendi il nickname dell'utente
                        topTimes.Add(new KeyValuePair<string, double>(nickname, bestTime)); // Aggiungi il nickname invece dell'UserID
                    }
                }

                // Ordina i tempi in ordine crescente
                topTimes = topTimes.OrderBy(entry => entry.Value).ToList();

                // Prendi solo i primi 5 tempi
                topTimes = topTimes.Take(5).ToList();

                foreach (var entry in topTimes)
                {
                    string nickname = entry.Key;
                    double time = entry.Value;
                    leaderboardDisplay += $"{nickname} - {time:F2}\n";
                }

                // Ora puoi visualizzare la classifica nel tuo testo della leaderboard
                leaderboardText.text = leaderboardDisplay;
            }
        });
}


    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false); // Nasconde la leaderboard
        scrollView.SetActive(true); // Mostra lo ScrollView
    }
}
