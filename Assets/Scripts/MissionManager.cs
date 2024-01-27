using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;
    public GameObject missionUIPrefab;
    public Transform missionsPanel; // Imposta questo riferimento nell'Inspector
private Mission currentMission;

    private DatabaseReference databaseReference;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadOrCreateMission();
                    Debug.Log("loadorcreatmission chiamato da start");

    }

   private void LoadOrCreateMission()
{
    var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("missions").Child("reachLevel").GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Error loading missions: " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            if (!snapshot.Exists)
            {
                CreateAndSaveMission(userId, 2);
            }
            else
            {
                // Carica la missione esistente
                currentMission = JsonUtility.FromJson<Mission>(snapshot.GetRawJsonValue());
                InstantiateMissionUI();
            }
        }
    });
}

    private void CreateAndSaveMission(string userId, int level)
    {
        Mission newMission = new Mission
        {
            id = $"reachLevel_{level}",
            description = $"Raggiungi il livello {level}",
            reward = new Reward { coins = 100, gems = 5 }
        };

        databaseReference.Child("users").Child(userId).Child("missions").Child("reachLevel").SetRawJsonValueAsync(JsonUtility.ToJson(newMission))
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Failed to save mission: " + task.Exception);
                }
                else
                {
                    // Missione salvata, istanzia l'UI della missione
    currentMission = newMission;
    InstantiateMissionUI();                }
            });
    }

private void InstantiateMissionUI()
{
    if (missionUIPrefab != null && missionsPanel != null)
    {
        GameObject missionUI = Instantiate(missionUIPrefab, missionsPanel);

        Text descriptionText = missionUI.transform.Find("DescriptionText").GetComponent<Text>();
        Text coinsText = missionUI.transform.Find("CoinsText").GetComponent<Text>();
        Text gemsText = missionUI.transform.Find("GemsText").GetComponent<Text>();
        Button rewardButton = missionUI.transform.Find("RewardButton").GetComponent<Button>();

        if (descriptionText == null || coinsText == null || gemsText == null || rewardButton == null)
        {
            Debug.LogError("Uno o più componenti UI non trovati nel prefab.");
            return;
        }

        if (currentMission != null)
        {
            descriptionText.text = currentMission.description;
            coinsText.text = $"Monete: {currentMission.reward.coins}";
            gemsText.text = $"Gemme: {currentMission.reward.gems}";
            rewardButton.interactable = currentMission.isCompleted;
            Debug.Log("UI aggiornata con i dati della missione.");
        }
        else
        {
            Debug.LogError("Mission corrente non trovata.");
        }
    }
    else
    {
        Debug.LogError("Prefab della missione o pannello delle missioni non impostati.");
    }
}



    public void CheckMissionProgress(int playerLevel)
{
    if (currentMission != null && playerLevel >= currentMission.target)
    {
        currentMission.isCompleted = true;
        Debug.Log($"Missione {currentMission.id} completata.");
        UpdateUI();
    }
}

private void CreateNextMission()
{
    int nextLevel = int.Parse(currentMission.id.Split('_')[1]) + 1;
    currentMission = new Mission
    {
        id = $"reachLevel_{nextLevel}",
        description = $"Raggiungi il livello {nextLevel}",
        reward = new Reward { coins = 100 * nextLevel, gems = 5 * nextLevel },
        isCompleted = false
    };

    var userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("missions").Child("reachLevel").SetRawJsonValueAsync(JsonUtility.ToJson(currentMission))
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Failed to save next mission: " + task.Exception);
            }
            else
            {
                Debug.Log("Next mission created.");
                UpdateUI();
            }
        });
}
private void UpdateUI()
{
    // Qui va la logica per aggiornare l'interfaccia utente
    // Questo potrebbe includere l'aggiornamento di testi e pulsanti
    // sulla base della missione corrente
}
private void OnRewardButtonClicked()
{
    Debug.Log($"Premi riscattati: {currentMission.reward.coins} monete, {currentMission.reward.gems} gemme.");
    // Qui dovresti aggiungere la logica per aggiungere i premi all'account dell'utente
    CreateNextMission();
}


}

[System.Serializable]
public class Mission
{
    public string id;
    public string description;
    public int target; // Aggiungi questa linea
    public bool isCompleted; // Aggiungi questa linea
    public Reward reward;
}

[System.Serializable]
public class Reward
{
    public int coins;
    public int gems;
}
