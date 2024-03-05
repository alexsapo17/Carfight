using UnityEngine;
using System.Collections.Generic; // Aggiungi questo per utilizzare i dizionari
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

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
    public Text survivalTimerText;
    public PrometeoCarController carController; // Riferimento al controller della macchina
    public Text finishTimeText; 
    public Text countdownText;
    private int currentLevelIndex = 0;
    private bool isLevelReady = false;
    public LevelProgressManager progressManager;
    public GameObject gameControlsUI;
    public GameObject levelLockedPanel; 
        public GameObject firstPanel; 
                public GameObject quitSurvivalButton; 
                public GameObject quitButton; 

public GameObject gameOverPanel;

    public Canvas canvas;
    public Image imageOnCanvas; // Aggiungi questa per l'immagine nel canvas
public Image[] childCanvasImages; // Array di immagini nel canvas
    public Image[] starImages; // Array di immagini delle stelle
    public Sprite fullStarSprite; // Sprite per la stella piena
public Sprite emptyStarSprite; // Sprite per la stella vuota
public Text gemsText;
public Animator transitionAnimator;
        public GameObject TutorialSingleplayer2Panel;

        public GameObject TutorialSingleplayer3Panel;
        public Transform survivalSpawnPoint;
        public GameObject retryButton;
private InterstitialAd interstitialAd;
    private float survivalTime = 0f;
    private bool gameIsOver = false;


    void Start()
    {
         PhotonNetwork.OfflineMode = true;

             // Crea una stanza offline
    PhotonNetwork.CreateRoom("OfflineRoom");


        // Esempio di come impostare la mappa
        levelCarMap = new Dictionary<int, int>
        {
            { 0, 0 }, // Livello 0 usa la macchina 0
            { 1, 1 }, // livello 1 usa la macchina 1
            { 2, 0 },
            { 3, 0 },
            { 4, 1 },
            { 5, 1 },
            { 6, 0 }, 
            { 7, 0 },
            { 8, 0 },
            { 9, 1 },
    
        };
        UpdateGemsUI();
            interstitialAd = GameObject.Find("AdsManager").GetComponent<InterstitialAd>();

    }

public void RestartLevel()
{
canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    LoadLevel(currentLevelIndex); // Ricarica il livello corrente
    finishPanel.SetActive(false); // Nasconde il pannello di fine livello
    gameControlsUI.SetActive(true);
            retryButton.gameObject.SetActive(true);

    SetImageTransparency(imageOnCanvas, 0); // Rendi trasparente l'immagine

    // Disattiva ogni GameObject associato a ciascuna Image nell'array
    foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(false);
    }

    if (interstitialAd != null)
    {
        interstitialAd.LoadAd();
    }
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
canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        LoadLevel(nextLevelIndex);
        finishPanel.SetActive(false);
        gameControlsUI.SetActive(true);
                    retryButton.gameObject.SetActive(true);

    SetImageTransparency(imageOnCanvas, 0); // Rendi trasparente l'immagine
    foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(false);
    }
        }
    else
    {
        levelLockedPanel.SetActive(true);
        StartCoroutine(DisablePanelAfterDelay(levelLockedPanel, 3f));
        return;
    }

   if (interstitialAd != null)
    {
        interstitialAd.LoadAd();
    }
}

private IEnumerator DisablePanelAfterDelay(GameObject panel, float delay)
{
    yield return new WaitForSeconds(delay);
    panel.SetActive(false);
}

   private void UpdateGemsUI()
    {
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null && gemsText != null)
        {
            currencyManager.gemsText = gemsText;
            currencyManager.UpdateGemsUI();
        }
    }

public void LoadLevel(int levelIndex)
{
                 if (PlayerPrefs.GetInt("ShowTutorialSingleplayerPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        TutorialSingleplayer3Panel.SetActive(true);
               TutorialSingleplayer2Panel.SetActive(false);
       PlayerPrefs.SetInt("ShowTutorialSingleplayerPanel", 0);


    }

    currentLevelIndex = levelIndex;
    Debug.Log($"Caricamento del livello {levelIndex}");

    if (currentLevel != null)
        PhotonNetwork.Destroy(currentLevel);
    if (currentCar != null)
        PhotonNetwork.Destroy(currentCar);
    gameControlsUI.SetActive(true);
                retryButton.gameObject.SetActive(true);
   quitSurvivalButton.gameObject.SetActive(false);
    quitButton.gameObject.SetActive(true);
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
canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    levelsPanel.SetActive(false);
    SetImageTransparency(imageOnCanvas, 0); // Rendi trasparente l'immagine
    foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(false);
    }       if (interstitialAd != null)
    {
        interstitialAd.LoadAd();
    }
}
    public void EliminatedPlayer()
    {
                    // Verifica se il pannello TutorialSingleplayer3Panel è attivo
        if (TutorialSingleplayer3Panel.activeSelf)
        {
            // Disattiva il pannello se è attivo
            TutorialSingleplayer3Panel.SetActive(false);
        }
    if (carController != null)
    {
        carController.SetKinematic(true);
    } 
 // Trova i canvas nella scena
Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
Canvas canvas2 = GameObject.Find("Canvas2").GetComponent<Canvas>();

// Assicurati che canvas sia quello desiderato e non null
if (canvas != null)
{

    // Trova tutte le camere nella scena, comprese quelle disattivate
    Camera[] cameras = FindObjectsOfType<Camera>(true); // true per includere oggetti disattivati
foreach (Camera cam in cameras)
{
    if (cam.gameObject.name == "PlayerCamera(Clone)")
    {
        Destroy(cam.gameObject);
        break; // Interrompe il ciclo una volta trovata e distrutta la PlayerCamera(Clone)
    }
}

    Camera mainCamera = null;

    foreach (Camera cam in cameras)
    {
        if (cam.gameObject.name == "Main Camera")
        {
            mainCamera = cam;
            break; // Interrompe il ciclo una volta trovata la Main Camera
        }
  
    }

    if (mainCamera != null)
    {
        // Assicurati di assegnare la main camera al Canvas corretto
        canvas.worldCamera = mainCamera;
        
        // Attiva la Main Camera in caso fosse disattivata
        mainCamera.gameObject.SetActive(true);
        
        // Cambia la modalità del Canvas
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
    else
    {
        Debug.LogError("No camera named 'Main Camera' found in the scene.");
    }
}
else
{
    Debug.LogError("Canvas not found in the scene.");
}



  gameControlsUI.SetActive(false);
    raceStarted = false;
    carController.controlsEnabled = false;
    finishPanel.SetActive(true);
    raceTimerText.gameObject.SetActive(false); // Nasconde il testo del timer della gara
isLevelReady = false;
    SetImageTransparency(imageOnCanvas, 1); // Rendi trasparente l'immagine
    foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(true);
    }
    UpdateStarDisplay(0);
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

        if (!gameIsOver)
        {
            survivalTime += Time.deltaTime;
            UpdateTimerUI(survivalTime);
        }
}
 public void StartSurvivalLevel()
{
    // Assicurati che il gioco sia in modalità offline se stai lavorando in singleplayer
    PhotonNetwork.OfflineMode = true;


        gameOverPanel.SetActive(false);

    // Distruggi il livello e la macchina correnti se presenti
    if (currentLevel != null)
        PhotonNetwork.Destroy(currentLevel);
    if (currentCar != null)
        PhotonNetwork.Destroy(currentCar);

    // Carica il nome della macchina selezionata dalle PlayerPrefs
    string selectedCarName = PlayerPrefs.GetString("SelectedCar");
        retryButton.gameObject.SetActive(false);

gameControlsUI.SetActive(true);
    quitSurvivalButton.gameObject.SetActive(true);
    quitButton.gameObject.SetActive(false);

    // Istanza la macchina selezionata al punto di spawn del livello di sopravvivenza
    currentCar = PhotonNetwork.Instantiate(selectedCarName, survivalSpawnPoint.position, survivalSpawnPoint.rotation);
    
    // Ottieni il componente PrometeoCarController dalla macchina appena istanziata
    carController = currentCar.GetComponent<PrometeoCarController>();

        
    
    
    // Prepara la scena per il livello di sopravvivenza
    Camera.main.gameObject.SetActive(false);

    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
    firstPanel.SetActive(false);
    SetImageTransparency(imageOnCanvas, 0); // Rendi trasparente l'immagine
    foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(false);
    }

    // Carica l'annuncio pubblicitario, se disponibile
    if (interstitialAd != null)
    {
        interstitialAd.LoadAd();
    }
// Assumi che EnemyManager sia assegnato tramite l'Inspector o trovalo qui
EnemyManager enemyManager = FindObjectOfType<EnemyManager>();
if (enemyManager != null)
{
    enemyManager.BeginEnemySpawn();
}
else
{
    Debug.LogError("EnemyManager non trovato.");
}

       survivalTime = 0f;
        gameIsOver = false;
        // Trova il SurvivalPickupsManager nella scena
    SurvivalPickupsManager pickupsManager = FindObjectOfType<SurvivalPickupsManager>();
    if (pickupsManager != null)
    {
        
                pickupsManager.StartRace(); // Termina lo spawn e pulisce i pickup

    }
    // Qui potresti voler avviare logiche specifiche per il livello di sopravvivenza,
    // come timer, spawn di nemici, ecc.
    StartCoroutine(EnableControlsAfterDelay(5f));
}


    IEnumerator EnableControlsAfterDelay(float delay)
    {
        // Mostra il countdown all'utente
        while (delay > 0)
        {
                countdownText.gameObject.SetActive(true);

            countdownText.text = "" + delay;
            yield return new WaitForSeconds(1);
            delay--;
        }

        // Azioni finali prima di abilitare i controlli
        countdownText.text = "";

        yield return new WaitForSeconds(0); // Opzionale: attendi un altro secondo prima di nascondere il testo
        
        countdownText.gameObject.SetActive(false); // Nasconde il testo del countdown

        carController.EnableControls(); // Abilita i controlli
    }


     public void FinishSurvivalGame()
    {
    
        gameIsOver = true;
        gameOverPanel.SetActive(true);
        gameControlsUI.SetActive(false);
    if (currentCar != null)
        PhotonNetwork.Destroy(currentCar);
 
    SetImageTransparency(imageOnCanvas, 1); // Rendi trasparente l'immagine
    foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(true);
    }
        // Distruggi tutti i nemici
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        // Trova i canvas nella scena
Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
Canvas canvas2 = GameObject.Find("Canvas2").GetComponent<Canvas>();

// Assicurati che canvas sia quello desiderato e non null
if (canvas != null)
{

    // Trova tutte le camere nella scena, comprese quelle disattivate
    Camera[] cameras = FindObjectsOfType<Camera>(true); // true per includere oggetti disattivati
foreach (Camera cam in cameras)
{
    if (cam.gameObject.name == "PlayerCamera(Clone)")
    {
        Destroy(cam.gameObject);
        break; // Interrompe il ciclo una volta trovata e distrutta la PlayerCamera(Clone)
    }
}

    Camera mainCamera = null;

    foreach (Camera cam in cameras)
    {
        if (cam.gameObject.name == "Main Camera")
        {
            mainCamera = cam;
            break; // Interrompe il ciclo una volta trovata la Main Camera
        }
  
    }

    if (mainCamera != null)
    {
        // Assicurati di assegnare la main camera al Canvas corretto
        canvas.worldCamera = mainCamera;
        
        // Attiva la Main Camera in caso fosse disattivata
        mainCamera.gameObject.SetActive(true);
        
        // Cambia la modalità del Canvas
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
    else
    {
        Debug.LogError("No camera named 'Main Camera' found in the scene.");
    }
}
else
{
    Debug.LogError("Canvas not found in the scene.");
}
// Trova il SurvivalPickupsManager nella scena
    SurvivalPickupsManager pickupsManager = FindObjectOfType<SurvivalPickupsManager>();
    if (pickupsManager != null)
    {
        pickupsManager.AssignCollectedItems();
                pickupsManager.EndRace(); // Termina lo spawn e pulisce i pickup

    }
    }
    

    void UpdateTimerUI(float time)
    {
        survivalTimerText.text = $"Survival Time: {time.ToString("F2")}";
    }



private void UpdateStarDisplay(int starsEarned)
{
    for (int i = 0; i < starImages.Length; i++)
    {
        starImages[i].sprite = i < starsEarned ? fullStarSprite : emptyStarSprite;
    }
}

private void SetImageTransparency(Image image, float alpha)
{
    if (image != null)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
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
public void OnShopButtonClicked()
{
    transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadShopScene", 1f); // Sostituisce la coroutine con Invoke
}

// Metodo separato per caricare la scena, chiamato dopo il ritardo
void LoadShopScene()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
}
public void FinishRace()
{
            // Verifica se il pannello TutorialSingleplayer3Panel è attivo
        if (TutorialSingleplayer3Panel.activeSelf)
        {
            // Disattiva il pannello se è attivo
            TutorialSingleplayer3Panel.SetActive(false);
        }
        if (carController != null)
    {
        carController.SetKinematic(true);
    }
 // Trova i canvas nella scena
Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
Canvas canvas2 = GameObject.Find("Canvas2").GetComponent<Canvas>();

// Assicurati che canvas sia quello desiderato e non null
if (canvas != null)
{

    // Trova tutte le camere nella scena, comprese quelle disattivate
    Camera[] cameras = FindObjectsOfType<Camera>(true); // true per includere oggetti disattivati
foreach (Camera cam in cameras)
{
    if (cam.gameObject.name == "PlayerCamera(Clone)")
    {
        Destroy(cam.gameObject);
        break; // Interrompe il ciclo una volta trovata e distrutta la PlayerCamera(Clone)
    }
}

    Camera mainCamera = null;

    foreach (Camera cam in cameras)
    {
        if (cam.gameObject.name == "Main Camera")
        {
            mainCamera = cam;
            break; // Interrompe il ciclo una volta trovata la Main Camera
        }
  
    }

    if (mainCamera != null)
    {
        // Assicurati di assegnare la main camera al Canvas corretto
        canvas.worldCamera = mainCamera;
        
        // Attiva la Main Camera in caso fosse disattivata
        mainCamera.gameObject.SetActive(true);
        
        // Cambia la modalità del Canvas
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
    }
    else
    {
        Debug.LogError("No camera named 'Main Camera' found in the scene.");
    }
}
else
{
    Debug.LogError("Canvas not found in the scene.");
}


gameControlsUI.SetActive(false);
    raceStarted = false;
    carController.controlsEnabled = false;
    finishTimeText.text = "Tempo Finale: " + raceTimer.ToString("F2") + " secondi";
    finishPanel.SetActive(true);
    raceTimerText.gameObject.SetActive(false); // Nasconde il testo del timer della gara
    isLevelReady = false;
   int starsEarned = progressManager.CalculateStars(currentLevelIndex, raceTimer);
    UpdateStarDisplay(starsEarned);
    
    progressManager.UpdateLevelProgress(currentLevelIndex, raceTimer);
    progressManager.FinishLevel(currentLevelIndex, raceTimer);
    SetImageTransparency(imageOnCanvas, 1); // Rendi trasparente l'immagine
   foreach (Image img in childCanvasImages)
    {
        img.gameObject.SetActive(true);
    }        // Mostra l'annuncio interstiziale quando finisci il livello
    if (interstitialAd != null)
    {
        interstitialAd.ShowAd();
    }

}


public void ReturnToLobby()
{
    transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadLobbyScene", 1f); // Sostituisce la coroutine con Invoke
}



   public void LoadLobbyScene()
    {
        // Disattiva la modalità offline di Photon e disconnettiti
        PhotonNetwork.OfflineMode = false;
        PhotonNetwork.Disconnect();
        // Carica la scena della lobby
        SceneManager.LoadScene("DemoAsteroids-LobbyScene");
    }}
