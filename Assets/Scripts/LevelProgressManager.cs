using UnityEngine;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI; 
using System.Linq;

[System.Serializable]
public class LevelProgress
{
    public float bestTime;
    public int stars;
    public bool isUnlocked;
public int[] coinsAwardedForStars;
            public bool gemsAwarded;

    public LevelProgress()
    {
        bestTime = float.MaxValue;
        stars = 0;
        isUnlocked = false;
        coinsAwardedForStars = new int[3]; // Inizializza per 3 possibili quantità di stelle
        gemsAwarded = false;

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
public Text gemsText;
public Text coinsToAwardText;
    public Dictionary<int, LevelProgress> levelsProgress = new Dictionary<int, LevelProgress>();
public Text bestTimeText;
    public GameObject levelUIPrefab; // Prefab per UI del livello
    public Transform levelsContainer; // Contenitore per gli UI dei livelli
    private LevelManager levelManager;
        public Dictionary<int, LevelStarRequirements> starRequirements;
           private DatabaseReference databaseReference;
private int playerCoins;
public Animator coinsAnimator;
private SlotMachineEffect slotMachineEffect;
    void Awake()
    {

          CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null && coinsText != null)
        {
            currencyManager.coinsText = coinsText;
            currencyManager.UpdateCoinsUI();
        }
            levelManager = FindObjectOfType<LevelManager>();
    slotMachineEffect = coinsText.GetComponent<SlotMachineEffect>();
 
 
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

 LoadCoinsAndUpdateUI();
        InitializeLevels();
                InitializeStarRequirements();

    }




public void UpdateLevelProgress(int levelId, float time)
{
    if (!levelsProgress.ContainsKey(levelId)) return;

    var levelProgress = levelsProgress[levelId];

    if (time < levelProgress.bestTime)
    {
        levelProgress.bestTime = time;
        levelProgress.stars = CalculateStars(levelId, time);

        SaveLevelData(levelId); // Salva i dati del livello

    }
}


private void SavePlayerCoins() {
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("coins").SetValueAsync(playerCoins);
    UpdateCoinsUI(); // Aggiorna l'UI ogni volta che salvi le monete
}

    private void AwardCoinsForLevel(int levelId, int starsEarned) {
        if (!levelsProgress.ContainsKey(levelId)) return;

        var levelProgress = levelsProgress[levelId];

        // Definisce le monete da assegnare per ogni quantità di stelle
        int[] coinsForStars = new int[] { 50, 100, 150 }; // Monete aggiuntive per 1, 2 e 3 stelle
        int coinsToAward = 0;

        for (int i = 0; i < starsEarned; i++) {
            if (levelProgress.coinsAwardedForStars[i] == 0) {
                coinsToAward += coinsForStars[i];
                levelProgress.coinsAwardedForStars[i] = coinsForStars[i]; // Segna come assegnate
            }
        }
 if (coinsToAward == 0) {
    coinsToAwardText.text = "0";

    }
        if (coinsToAward > 0) {
            int startValue = playerCoins;
            playerCoins += coinsToAward;
            SavePlayerCoins(); // Aggiorna le monete del giocatore

            // Aggiorna il testo UI con il nuovo totale di monete
            if (coinsToAwardText != null) {
    coinsToAwardText.text = coinsToAward.ToString();
            }

            // Animazione e aggiornamento UI
            if (coinsAnimator != null) {
                coinsAnimator.SetTrigger("AddedCoins");
            }

            if (slotMachineEffect != null) {
                slotMachineEffect.AnimateText(startValue, playerCoins); // Aggiorna con il nuovo totale
            }

            SaveLevelData(levelId); // Non dimenticare di salvare i progressi aggiornati
        }
    }




  

void InitializeLevels()
{
    // Sblocca il livello 0 di default e crea l'UI
    levelsProgress[0] = new LevelProgress { isUnlocked = true };
    CreateLevelUI(0);
    LoadBestTime(0);

    // Inizializza gli altri livelli
    for (int i = 1; i < 11; i++)
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
            { 0, new LevelStarRequirements(7f, 9f, 10f) },
            { 1, new LevelStarRequirements(8.4f, 10f, 15f) },
            { 2, new LevelStarRequirements(15f, 18f, 26f) },
            { 3, new LevelStarRequirements(55f, 65f, 90f) },
            { 4, new LevelStarRequirements(21f, 25f, 30f) },
            { 5, new LevelStarRequirements(60f, 80f, 100f) },
            { 6, new LevelStarRequirements(20f, 25f, 40f) },
            { 7, new LevelStarRequirements(13f, 17f, 20f) },
            { 8, new LevelStarRequirements(45f, 55f, 70f) },
            { 9, new LevelStarRequirements(85f, 95f, 120f) },
            { 10, new LevelStarRequirements(65f, 75f, 120f) },

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
    private void UpdateBestTimeUI(float bestTime)
    {
        if (bestTimeText != null)
        {
                            string language = PlayerPrefs.GetString("Language", "en"); // Ottieni la lingua corrente
 if (language == "it")
        {
            bestTimeText.text = "Miglior Tempo: " + bestTime.ToString("F2");
        }
         if (language == "en")
        {
            bestTimeText.text = "Best time: " + bestTime.ToString("F2");
        }
         if (language == "es")
        {
            bestTimeText.text = "Mejor tiempo: " + bestTime.ToString("F2");
        }
         if (language == "fr")
        {
            bestTimeText.text = "Meilleur temps: " + bestTime.ToString("F2");
        }
        }
    }



 public int CalculateStars(int levelId, float time)
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


        UpdateLevelUI(levelId);
}
private void SaveTotalStars()
{
    // Calcola il totale delle stelle
    int totalStars = 0;
    foreach (var level in levelsProgress.Values)
    {
        totalStars += level.stars;
    }

  
}
}
