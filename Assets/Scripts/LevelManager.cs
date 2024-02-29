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
    public PrometeoCarController carController; // Riferimento al controller della macchina
    public Text finishTimeText; 
    public Text countdownText;
    private int currentLevelIndex = 0;
    private bool isLevelReady = false;
    public LevelProgressManager progressManager;
    public GameObject gameControlsUI;
    public GameObject levelLockedPanel; 
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

private InterstitialAd interstitialAd;



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
   if (canvas != null)
        {
            // Distruggi la camera correntemente in uso se non è null
            if (canvas.worldCamera != null)
            {
                Destroy(canvas.worldCamera.gameObject);
            }

            // Trova e assegna la camera chiamata "Main Camera"
            Camera mainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
            if (mainCamera != null)
            {
                canvas.worldCamera = mainCamera;
                // Assicurati che la camera "Main Camera" sia attiva
                mainCamera.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("No camera named 'Main Camera' found in the scene.");
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera; // Cambia la modalità del Canvas
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
  if (canvas != null)
        {
            // Distruggi la camera correntemente in uso se non è null
            if (canvas.worldCamera != null)
            {
                Destroy(canvas.worldCamera.gameObject);
            }

            // Trova e assegna la camera chiamata "Main Camera"
            Camera mainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
            if (mainCamera != null)
            {
                canvas.worldCamera = mainCamera;
                // Assicurati che la camera "Main Camera" sia attiva
                mainCamera.gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("No camera named 'Main Camera' found in the scene.");
            }

            canvas.renderMode = RenderMode.ScreenSpaceCamera; // Cambia la modalità del Canvas
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
