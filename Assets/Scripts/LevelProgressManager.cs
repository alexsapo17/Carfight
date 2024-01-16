using UnityEngine;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI; 

[System.Serializable]
public class LevelProgress
{
    public float bestTime;
    public int stars;
    public bool isUnlocked;
            public bool coinsAwarded;

    public LevelProgress()
    {
        bestTime = float.MaxValue;
        stars = 0;
        isUnlocked = false;
        coinsAwarded = false;

    }
}
[System.Serializable]
public class LevelStarRequirements
{
    public float timeForThreeStars;
    public float timeForTwoStars;
    public float timeForOneStar;
 

    public LevelStarRequirements(float threeStars, float twoStars, float oneStar)
    {
        timeForThreeStars = threeStars;
        timeForTwoStars = twoStars;
        timeForOneStar = oneStar;
        
    }
}
public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance;
public Text coinsText;
    public Dictionary<int, LevelProgress> levelsProgress = new Dictionary<int, LevelProgress>();
public Text bestTimeText;
    public GameObject levelUIPrefab; // Prefab per UI del livello
    public Transform levelsContainer; // Contenitore per gli UI dei livelli
    private LevelManager levelManager;
        public Dictionary<int, LevelStarRequirements> starRequirements;
           private DatabaseReference databaseReference;
private int playerCoins;

    void Awake()
    {
            levelManager = FindObjectOfType<LevelManager>();

 
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
 LoadCoinsAndUpdateUI();
        InitializeLevels();
                InitializeStarRequirements();

    }
   


private void SavePlayerCoins() {
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("coins").SetValueAsync(playerCoins);
    UpdateCoinsUI(); // Aggiorna l'UI ogni volta che salvi le monete
}

private void AwardCoinsForLevel(int levelId, int starsEarned) {
    if (!levelsProgress.ContainsKey(levelId)) return;

    var levelProgress = levelsProgress[levelId];

    if (levelProgress.coinsAwarded) return;

    int coinsToAward = 0;
    switch (starsEarned) {
        case 1:
            coinsToAward = 50;
            break;
        case 2:
            coinsToAward = 150; // 100 + 50
            break;
        case 3:
            coinsToAward = 300; // 150 + 100 + 50
            break;
        default:
            return;
    }

  
    playerCoins += coinsToAward;
    SavePlayerCoins();

    levelProgress.coinsAwarded = true;
    SaveLevelData(levelId);
}

void InitializeLevels()
{
    // Sblocca il livello 0 di default e crea l'UI
    levelsProgress[0] = new LevelProgress { isUnlocked = true };
    CreateLevelUI(0);
    LoadBestTime(0);

    // Inizializza gli altri livelli
    for (int i = 1; i < 10; i++)
    {
        if (!levelsProgress.ContainsKey(i))
        {
            levelsProgress[i] = new LevelProgress();
        }
        CreateLevelUI(i); // Assicurati di chiamare questa funzione per ogni livello
        LoadBestTime(i);  // Carica il miglior tempo per il livello
    }

    LoadLevelsData();
}

// Chiamata quando un livello viene completato
public void FinishLevel(int levelId, float time)
{
    UpdateLevelProgress(levelId, time);
    CheckAndUnlockNextLevel(levelId);
    int newStars = CalculateStars(levelId, time);
    AwardCoinsForLevel(levelId, newStars);
}
private void CheckAndUnlockNextLevel(int completedLevelId)
{
    int nextLevelId = completedLevelId + 1;
    if (nextLevelId < levelsProgress.Count)
    {
        if (!levelsProgress.ContainsKey(nextLevelId))
        {
            // Inizializza il livello successivo solo se non esiste
            levelsProgress[nextLevelId] = new LevelProgress();
        }

        if (levelsProgress[completedLevelId].stars > 0 && !levelsProgress[nextLevelId].isUnlocked)
        {
            // Sblocca il livello successivo solo se il livello attuale è stato completato con almeno una stella
            levelsProgress[nextLevelId].isUnlocked = true;
            // Aggiorna solo lo stato di sblocco su Firebase
            UpdateUnlockStatusOnFirebase(nextLevelId);
        }
    }
    UpdateLevelUI(nextLevelId); // Aggiorna l'UI del livello successivo
}

private void UpdateUnlockStatusOnFirebase(int levelId)
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .Child(userId)
        .Child("levelsProgress")
        .Child(levelId.ToString())
        .Child("isUnlocked")
        .SetValueAsync(levelsProgress[levelId].isUnlocked);
}



    void InitializeStarRequirements()
    {
        starRequirements = new Dictionary<int, LevelStarRequirements>
        {
            { 0, new LevelStarRequirements(5f, 8f, 10f) },
            { 1, new LevelStarRequirements(5f, 8f, 10f) },
            { 2, new LevelStarRequirements(5f, 8f, 10f) },
            { 3, new LevelStarRequirements(10f, 15f, 20f) },
            { 4, new LevelStarRequirements(5f, 10f, 20f) },
            { 5, new LevelStarRequirements(10f, 15f, 20f) },
        };
    }
private void CreateLevelUI(int levelId)
{
    GameObject levelUIObject = Instantiate(levelUIPrefab, levelsContainer);
    LevelUI levelUI = levelUIObject.GetComponent<LevelUI>();

    levelUI.Initialize(levelId, this);

    // Aggiungi un listener al pulsante che verifica lo stato di sblocco del livello
    levelUI.SetButtonListener(() => {
        if (levelsProgress[levelId].isUnlocked)
        {
            levelManager.LoadLevel(levelId);
        }
        else
        {
            levelUI.ShowLockedLevelPanel();
        }
    });
}

public void UpdateLevelUI(int levelId)
{
    foreach (Transform child in levelsContainer)
    {
        LevelUI levelUI = child.GetComponent<LevelUI>();
        if (levelUI != null && levelUI.LevelId == levelId)
        {
            LevelProgress levelProgress = levelsProgress[levelId];
            levelUI.UpdateUI(levelProgress.bestTime, levelProgress.stars);
            break;
        }
    }
}
private void LoadCoinsAndUpdateUI() {
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("coins").GetValueAsync().ContinueWithOnMainThread(task => {
        if (task.IsFaulted) {
            // Gestire l'errore
        } else if (task.IsCompleted) {
            DataSnapshot snapshot = task.Result;
            playerCoins = snapshot.Value != null ? int.Parse(snapshot.Value.ToString()) : 0;
            UpdateCoinsUI();
        }
    });
}

  private void UpdateCoinsUI()
{
    if (coinsText != null)
    {
        coinsText.text = "" + playerCoins;
    }
}

 public void LoadBestTime(int levelId)
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

    FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .Child(userId)
        .Child("levelsProgress")
        .Child(levelId.ToString())
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Errore nel caricamento dei dati del livello " + levelId);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    LevelProgress levelProgress = JsonUtility.FromJson<LevelProgress>(snapshot.GetRawJsonValue());
                    levelsProgress[levelId] = levelProgress;
                                Debug.Log($"Caricato miglior tempo: {levelProgress.bestTime} e stelle: {levelProgress.stars} per livello {levelId}");

                    UpdateLevelUI(levelId); // Aggiorna l'UI del livello con i dati caricati
                }

            }
        });
}

    private void UpdateBestTimeUI(float bestTime)
    {
        if (bestTimeText != null)
        {
            bestTimeText.text = "Miglior Tempo: " + bestTime.ToString("F2");
        }
    }
 public void UpdateLevelProgress(int levelId, float time)
{
    if (!levelsProgress.ContainsKey(levelId)) return;

    var levelProgress = levelsProgress[levelId];

    Debug.Log($"Aggiornamento livello {levelId}. Tempo: {time}, Miglior tempo: {levelProgress.bestTime}");

    if (time < levelProgress.bestTime)
    {
        levelProgress.bestTime = time;
        levelProgress.stars = CalculateStars(levelId, time);

        Debug.Log($"Nuovo record! Tempo: {time}, Stelle: {levelProgress.stars}");
        SaveLevelData(levelId); // Assicurati che questa chiamata venga eseguita
    }
}

 private int CalculateStars(int levelId, float time)
{
    if (!starRequirements.ContainsKey(levelId))
    {
        Debug.LogError("Star requirements not set for level " + levelId);
        return 0;
    }

    var requirements = starRequirements[levelId];
    Debug.Log($"Calcolo stelle per livello {levelId}. Tempo: {time}");

    if (time <= requirements.timeForThreeStars)
    {
        Debug.Log("Assegnate 3 stelle");
        return 3;
    }
    if (time <= requirements.timeForTwoStars)
    {
        Debug.Log("Assegnate 2 stelle");
        return 2;
    }
    if (time <= requirements.timeForOneStar)
    {
        Debug.Log("Assegnata 1 stella");
        return 1;
    }

    Debug.Log("Nessuna stella assegnata");
    return 0;
}


private void LoadLevelsData()
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;

    FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .Child(userId)
        .Child("levelsProgress")
        .GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Gestire l'errore
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    int levelId = int.Parse(childSnapshot.Key);
                    LevelProgress levelProgress = JsonUtility.FromJson<LevelProgress>(childSnapshot.GetRawJsonValue());
                    levelsProgress[levelId] = levelProgress;
                }
            }
        });
}


private void SaveLevelData(int levelId)
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    var levelProgress = levelsProgress[levelId];
    Debug.Log($"Salvataggio dati per livello {levelId}. Tempo: {levelProgress.bestTime}, Stelle: {levelProgress.stars}");

    // Serializza i dati del livello per il salvataggio
    string jsonData = JsonUtility.ToJson(levelProgress);

    // Salva i dati su Firebase
    FirebaseDatabase.DefaultInstance
        .GetReference("users")
        .Child(userId)
        .Child("levelsProgress")
        .Child(levelId.ToString())
        .SetRawJsonValueAsync(jsonData);
}
private void SaveTotalStars()
{
    // Calcola il totale delle stelle
    int totalStars = 0;
    foreach (var level in levelsProgress.Values)
    {
        totalStars += level.stars;
    }

    // Salva il totale delle stelle su Firebase
    // (implementa il salvataggio effettivo qui)
}
}
