using UnityEngine;
using System.Collections.Generic; // Aggiungi questo per utilizzare i dizionari
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;


public class LevelManager : MonoBehaviour
{
    public GameObject[] levelPrefabs; // Array di prefab dei livelli
    public GameObject[] carPrefabs; // Array di prefab delle macchine
    public Dictionary<int, int> levelCarMap; // Mappa i livelli alle macchine
    public GameObject finishPanel;
    public GameObject levelsPanel;
    private GameObject currentLevel; // Riferimento al livello corrente
    private GameObject currentCar; // Riferimento alla macchina corrente
    public float countdownTime = 3f; // Tempo per il conto alla rovescia
    private float raceTimer; // Timer della gara
    private bool raceStarted = false;
    public Text raceTimerText; // Riferimento al testo del timer nel UI
    public PrometeoCarController carController; // Riferimento al controller della macchina
public Text finishTimeText; 
public Text countdownText;
    private int currentLevelIndex = 0;
    private bool isLevelReady = false;
public LevelProgressManager progressManager;
public GameObject gameControlsUI;
    public GameObject levelLockedPanel; 

    void Start()
    {
         PhotonNetwork.OfflineMode = true;

             // Crea una stanza offline
    PhotonNetwork.CreateRoom("OfflineRoom");

        // Esempio di come impostare la mappa
        levelCarMap = new Dictionary<int, int>
        {
            { 0, 0 }, // Livello 0 usa la macchina 0
            { 1, 0 }, // livello 1 usa la macchina 1
            { 2, 1 }, // Livelli 2-3 usano la macchina 1
            { 3, 1 }
        };
    }

    public void RestartLevel()
    {
        Time.timeScale = 1;
        LoadLevel(currentLevelIndex); // Ricarica il livello corrente
        finishPanel.SetActive(false); // Nasconde il pannello di fine livello
        gameControlsUI.SetActive(true);
    }

public void LoadNextLevel()
{
    int nextLevelIndex = currentLevelIndex + 1;
    if (nextLevelIndex >= levelPrefabs.Length) 
    {
        nextLevelIndex = 0;
    }

    if (progressManager.levelsProgress[nextLevelIndex].isUnlocked)
    {
        LoadLevel(nextLevelIndex);
        finishPanel.SetActive(false);
        gameControlsUI.SetActive(true);
    }
    else
    {
        levelLockedPanel.SetActive(true);
        StartCoroutine(DisablePanelAfterDelay(levelLockedPanel, 3f));
        return;
    }
            Time.timeScale = 1;


}

private IEnumerator DisablePanelAfterDelay(GameObject panel, float delay)
{
    yield return new WaitForSeconds(delay);
    panel.SetActive(false);
}


public void LoadLevel(int levelIndex)
{
        
    currentLevelIndex = levelIndex;
    Debug.Log($"Caricamento del livello {levelIndex}");

    if (currentLevel != null)
        PhotonNetwork.Destroy(currentLevel);
    if (currentCar != null)
        PhotonNetwork.Destroy(currentCar);
    Time.timeScale = 1;
    gameControlsUI.SetActive(true);
    currentLevel = PhotonNetwork.Instantiate(levelPrefabs[levelIndex].name, new Vector3(0, 0, 0), Quaternion.identity);
    int carIndex = levelCarMap[levelIndex];
    currentCar = PhotonNetwork.Instantiate(carPrefabs[carIndex].name, new Vector3(0, 1, 0), Quaternion.identity);

    // Ottieni il componente PrometeoCarController dall'auto appena istanziata
    carController = currentCar.GetComponent<PrometeoCarController>();

    Camera.main.gameObject.SetActive(false);
    isLevelReady = true;
    countdownTime = 3f;
    countdownText.gameObject.SetActive(true);
    raceTimerText.gameObject.SetActive(false);
    raceStarted = false;

    levelsPanel.SetActive(false);
}
    public void EliminatedPlayer()
    {
  
  gameControlsUI.SetActive(false);
    raceStarted = false;
    carController.controlsEnabled = false;
    finishPanel.SetActive(true);
    raceTimerText.gameObject.SetActive(false); // Nasconde il testo del timer della gara
    Time.timeScale = 0;
isLevelReady = false;
    }
void Update()
{
    if (isLevelReady && !raceStarted)
    {
        if (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            countdownText.text = Mathf.CeilToInt(countdownTime).ToString();
        }
        else
        {
            StartRace();
        }
    }
     else if (raceStarted )
    {
        UpdateRaceTimer(); 
    }


}


private void UpdateRaceTimer()
{
    if (!raceStarted)
    {
        return; // Non aggiornare il timer se la gara è finita
    }
    
    raceTimer += Time.deltaTime;
    raceTimerText.text = "Tempo: " + raceTimer.ToString("F2");
}


private void StartRace()
{
    raceStarted = true;
    if (carController != null)
    {
        carController.EnableControls();
    }
    else
    {
        Debug.LogError("carController è null in StartRace");
    }

    countdownText.gameObject.SetActive(false);
    raceTimer = 0;
    raceTimerText.gameObject.SetActive(true);
}

public void FinishRace()
{
gameControlsUI.SetActive(false);
    raceStarted = false;
    carController.controlsEnabled = false;
    finishTimeText.text = "Tempo Finale: " + raceTimer.ToString("F2") + " secondi";
    finishPanel.SetActive(true);
    raceTimerText.gameObject.SetActive(false); // Nasconde il testo del timer della gara
    Time.timeScale = 0;
isLevelReady = false;
    // Aggiorna i progressi del livello
    progressManager.UpdateLevelProgress(currentLevelIndex, raceTimer);
        progressManager.FinishLevel(currentLevelIndex, raceTimer);

}




    // Altri metodi possono essere aggiunti qui
}
