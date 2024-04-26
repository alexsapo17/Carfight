using UnityEngine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;





public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject carPrefab; // Prefab della macchina
    public static GameObject selectedCarPrefab;

    public Text countdownText;
    public float preRaceTimer = 7.0f; // Tempo prima dell'inizio della gara
    public float startRaceTimer = 5.0f; // Timer per l'inizio della gara
       public Text preRaceCountdownText; 
    public Text startRaceCountdownText;
        public AudioSource audioSource;

      public Transform[] spawnPoints; // Punti di spawn delle macchine
    public Transform[] startLineSpawnPoints;
    public GameObject finishPlane;
    public GameObject resultsPanel; // Pannello per i risultati
    public Text resultsText;
    public Button returnToLobbyButton; // Pulsante per tornare alla lobby
    
    public GameObject brickWallPrefab; // Prefab del muro di mattoni
    public Transform[] brickWallSpawnPoints; // Punti di spawn del muro di mattoni
public Camera mainCamera;

public Image handbrakeButton; 
public Image throttleButton;
public Image reverseButton;
public RectTransform abilityButtons;
public HorizontalJoystick horizontalJoystick;
    public RectTransform button1RectTransform; // Assicurati di assegnare questi nel Unity Inspector
    public RectTransform button2RectTransform;

public Text coinsAwardText;
    [SerializeField]
    private GameObject[] aiCarPrefabs;
        private List<GameObject> instantiatedCars = new List<GameObject>(); // Lista delle macchine istanziate
    private List<GameObject> finishedCars = new List<GameObject>(); // Macchine che hanno finito la gara
    private Dictionary<GameObject, float> carFinishTimes = new Dictionary<GameObject, float>(); // Tempi di arrivo delle macchine
    private float raceStartTime; // Tempo di inizio della gara
    private List<GameObject> eliminatedPlayers = new List<GameObject>();
    private GameObject lastPlayerStanding;
    private Dictionary<string, int> playerPositions = new Dictionary<string, int>();
//public GameObject arrowPrefab;
 // public ArrowDirection arrowScript;
public Animator transitionAnimator;
    public Dictionary<string, GameObject> carPrefabs;
    public GameObject eliminatedTextPrefab;
    public GameObject winTextPrefab;
public Canvas canvas;
public GameObject playerEliminatedPrefab;
public GameObject playerWinPrefab;
public Transform finishPrefabspawnPoint; // Assicurati di assegnare questo dall'Inspector
            public int experienceWinnerGain;
            public int experienceEliminatedGain;


    [System.Serializable]
    public class RewardTier
    {
        public int playerCount;
        public int[] rewards;
    }

    public RewardTier[] rewardTiers;
void Start()
{

    string selectedCarName = PlayerPrefs.GetString("SelectedCar", "DefaultCarName");
    GameObject carPrefab = Resources.Load<GameObject>(selectedCarName);  

    if (carPrefab == null)
    {
        Debug.LogError("Impossibile caricare il prefab della macchina: " + selectedCarName);
        return;
    }

    if (!PhotonNetwork.IsConnectedAndReady)
    {
        Debug.LogError("Photon non è connesso e pronto.");
        return;
    }

   int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
    if (spawnIndex < spawnPoints.Length)
    {
    GameObject car = PhotonNetwork.Instantiate(carPrefab.name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
        
     //  GameObject arrow = PhotonNetwork.Instantiate(arrowPrefab.name, car.transform.position + Vector3.up * 2, Quaternion.identity);
/*ArrowDirection arrowScript = arrow.GetComponent<ArrowDirection>();
if (arrowScript != null)
{
    arrowScript.SetTarget(car.transform);
    arrowScript.SetJoystick(horizontalJoystick); // Assicurati che il joystick sia impostato correttamente
}*/

        // Assicurati che il RPC faccia ciò che desideri con car.name
        photonView.RPC("RegisterCarInstance", RpcTarget.All, car.name);
    }
    else
    {
        Debug.LogError("Indice di spawn non valido: " + spawnIndex);
    }

    // Istanziare i muri di mattoni
    foreach (Transform spawnPoint in brickWallSpawnPoints)
    {
        PhotonNetwork.Instantiate(brickWallPrefab.name, spawnPoint.position, spawnPoint.rotation);
    }
        int controlSetup = PlayerPrefs.GetInt("ControlSetup", 2);
        if (controlSetup == 1)
        {
            // Sposta i pulsanti verso destra, fuori dal canvas
            MoveButtonOutOfView(button1RectTransform); 
            MoveButtonOutOfView(button2RectTransform);
        }
/*int fakePlayerCount = GlobalGameManager.Instance.FakePlayerCount;
int count= fakePlayerCount;

Debug.Log($"Fake player count before coroutine: {fakePlayerCount}");
StartCoroutine(SpawnFakePlayers(count));*/

    StartCoroutine(PreRaceCountdown());
    resultsPanel.SetActive(false);
    
}

   /* private IEnumerator SpawnFakePlayers(int count)
    {
        Debug.Log($"[GameManager] Starting to spawn {count} fake players.");
        yield return new WaitForSeconds(1); // Un lieve ritardo per sicurezza.
        
        List<string> fakeNames = GlobalGameManager.Instance.GetFakePlayerNames();

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"[GameManager] Attempting to spawn fake player {i + 1} of {count}.");
            int spawnIndex = instantiatedCars.Count;
            if (spawnIndex < spawnPoints.Length && i < fakeNames.Count)
            {
                // Seleziona casualmente un prefab per l'auto AI
                GameObject aiCarPrefab = aiCarPrefabs[Random.Range(0, aiCarPrefabs.Length)];
                
                // Istanzia l'auto AI con la rotazione dello spawn point
                GameObject fakeCar = PhotonNetwork.Instantiate(aiCarPrefab.name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
                instantiatedCars.Add(fakeCar);

                // Assegna il nome
                var playerNameText = fakeCar.transform.Find("PlayerName").GetComponent<Text>();
                if (playerNameText != null)
                {
                    playerNameText.text = fakeNames[i];
                }
                else
                {
                    Debug.LogError("Text component for player name not found");
                }

                Debug.Log($"[GameManager] Fake player {i + 1} spawned with name {fakeNames[i]}.");
            }
            else
            {
                Debug.LogWarning($"[GameManager] Insufficient spawn points for fake player {i + 1} or missing name.");
            }
        }

        // Reset dei dati dopo lo spawn
        GlobalGameManager.Instance.ResetFakePlayerCountAndNames();
    }

*/



    private void MoveButtonOutOfView(RectTransform buttonRectTransform)
    {
        // Questo è solo un esempio, dovrai adattare i valori in base alle dimensioni del tuo canvas e alla posizione desiderata
        Vector2 newPosition = new Vector2(2000, buttonRectTransform.anchoredPosition.y); // Sposta di 2000 unità a destra
        buttonRectTransform.anchoredPosition = newPosition;
    }
    [PunRPC]
    void RegisterCarInstance(string carName)
    {
        GameObject carObject = GameObject.Find(carName);
        if (carObject != null)
        {
            instantiatedCars.Add(carObject);
            Debug.Log("Auto registrata: " + carName);
        }
        else
        {
            Debug.LogError("Non è stato possibile trovare l'auto con nome: " + carName);
        }
    }
   IEnumerator PreRaceCountdown()
    {
                        string language = PlayerPrefs.GetString("Language", "en"); // Ottieni la lingua corrente

        if (language == "it")
        {
                countdownText.text = "Attendendo altri giocatori...";
        }
                if (language == "en")
        {
                countdownText.text = "Waiting for other players...";
        }
                if (language == "es")
        {
                countdownText.text = "Esperando a otros jugadores...";
        }
                if (language == "fr")
        {
                countdownText.text = "En attente d'autres joueurs...";
        }
         // Attendi che tutte le auto siano istanziate.
        while (PhotonNetwork.CurrentRoom != null && instantiatedCars != null && PhotonNetwork.CurrentRoom.PlayerCount != instantiatedCars.Count)
        {

            yield return null; // Aspetta un frame prima di controllare nuovamente.
        }
countdownText.gameObject.SetActive(false);
        if (PhotonNetwork.CurrentRoom == null)
        {
            countdownText.text = ""; // Pulisci il testo se la stanza non esiste più.
            yield break; // Fermati se la stanza non esiste più.
        }

        preRaceCountdownText.text = "";
        float preRaceTimer = this.preRaceTimer;
        while (preRaceTimer > 0)
        {
            preRaceCountdownText.text = Mathf.RoundToInt(preRaceTimer).ToString();
            preRaceTimer -= Time.deltaTime;
            yield return null;
        }
                preRaceCountdownText.gameObject.SetActive(false); // Opzionale: nascondi il testo del pre-race timer

        yield return new WaitForSeconds(1f); // Breve pausa prima di iniziare il countdown della gara


        startRaceCountdownText.gameObject.SetActive(true);

        // Se l'AudioSource non è stato trovato, stampa un messaggio di avviso
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource non trovato sull'oggetto " + gameObject.name);
        }

// Avvia la riproduzione dell'AudioSource
            float startTimer = startRaceTimer;
            StartCoroutine(TimerSound(2)); // Ritardo di 2 secondi
         while (startTimer > 0)
        {
            startRaceCountdownText.text = Mathf.RoundToInt(startTimer).ToString();
            startTimer -= Time.deltaTime;





    
            yield return null;
        }

         startRaceCountdownText.text = ""; // Nascondi il testo del countdown di inizio gara
         photonView.RPC("StartRaceSync", RpcTarget.All);
                    // Trova il GameObject chiamato "BallSpawnerManager"
        GameObject ballSpawnerManager = GameObject.Find("BallSpawnerManager");

        // Verifica se il GameObject è stato trovato
        if (ballSpawnerManager != null)
        {
            // Ottieni il componente BallSpawner dal GameObject
            BallSpawner ballSpawner = ballSpawnerManager.GetComponent<BallSpawner>();

            // Verifica se il componente è stato trovato
            if (ballSpawner != null)
            {
                // Chiamare il metodo StartSpawnProcess su BallSpawner
                ballSpawner.StartSpawnProcess();
                                Debug.Log("gamemanager chiama ballspawner.");

            }
            else
            {
                Debug.LogWarning("Componente BallSpawner non trovato su BallSpawnerManager.");
            }
        }
        else
        {
            Debug.LogWarning("GameObject BallSpawnerManager non trovato nella scena.");
        }
    }
    public void SetCarPrefab(string carName)
    {
        if (carPrefabs.TryGetValue(carName, out GameObject prefab))
        {
            selectedCarPrefab = prefab;
        }
        else
        {
            Debug.LogError("Prefab non trovato per: " + carName);
        }
    }

    void StartRace()
    {
        raceStartTime = Time.time;
        Debug.Log("Gara iniziata. Tempo di inizio della gara: " + raceStartTime);

        if (instantiatedCars.Count == 1 && playerPositions.Count == 0)
        {
            GameObject singlePlayer = instantiatedCars.FirstOrDefault();
            if (singlePlayer != null)
            {
                PhotonView playerView = singlePlayer.GetComponent<PhotonView>();
                if (playerView != null)
                {
                    string playerId = playerView.Owner.UserId;
                    playerPositions[playerId] = 1;
                    SavePlayerPositions();
                    photonView.RPC("AnnounceWinner", RpcTarget.All, playerView.ViewID);
                }
            }
        }
        else
        {
            foreach (GameObject car in instantiatedCars)
            {
                var carController = car.GetComponent<PrometeoCarController>();
                if (carController != null)
                {
                    carController.EnableControls();
                }
            }
            
 
        }
 
    

    }


    public void PlayerEliminated(GameObject player)
    {
          
    
        if (!eliminatedPlayers.Contains(player))
        {
            Debug.Log("PlayerEliminated chiamato per: " + player.name);

            eliminatedPlayers.Add(player);

            // Imposta il tempo di eliminazione
            carFinishTimes[player] = Time.time - raceStartTime;

  // Gestione per i giocatori AI
        if (player.tag == "Enemy")
        {
            Debug.Log(player.name + " AI eliminato.");
            // Assicurati che questo non influisca sul MasterClient in maniera negativa
            // Qui potresti decidere di non fare nulla di specifico per il MasterClient
            // dato che l'AI è controllato dal MasterClient ma non lo rappresenta direttamente
          if (playerEliminatedPrefab != null && finishPrefabspawnPoint != null)
{
    Instantiate(playerEliminatedPrefab, finishPrefabspawnPoint.position, finishPrefabspawnPoint.rotation);
}
        }
        if (player.tag == "Player")
        {
                ExperienceManager.Instance.AddExperience(experienceEliminatedGain);

            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
       // Se l'oggetto ha un PhotonView e l'owner è il giocatore locale, procedi con l'eliminazione
        if (playerPhotonView != null && playerPhotonView.IsMine)
        {
                string playerName = playerPhotonView.Owner.NickName;
                Debug.Log(playerName + " è stato eliminato. Tempo: " + carFinishTimes[player]);
            if (playerEliminatedPrefab != null && finishPrefabspawnPoint != null)
{
    Instantiate(playerEliminatedPrefab, finishPrefabspawnPoint.position, finishPrefabspawnPoint.rotation);
}


    // Disattiva i controlli del giocatore eliminato e gestisci la telecamera
  // Nel contesto del giocatore eliminato
    var playerController = player.GetComponent<PrometeoCarController>();
    if (playerController != null) {
        playerController.DisableControls();
        playerController.DestroyCameraInstance();
    }


    // Gestisci la telecamera principale e il pannello dei risultati
    if (player.GetComponent<PhotonView>().IsMine)
    {
        mainCamera.gameObject.SetActive(true); // Attiva la telecamera principale
        resultsPanel.SetActive(true); // Mostra il pannello dei risultati

RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
if (hbRectTransform != null)
{
    hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
RectTransform tbRectTransform = throttleButton.GetComponent<RectTransform>();
if (tbRectTransform != null)
{
    tbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
RectTransform rbRectTransform = reverseButton.GetComponent<RectTransform>();
if (rbRectTransform != null)
{
    rbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}

    RectTransform joystickRectTransform = horizontalJoystick.GetComponent<RectTransform>();
    
    if (joystickRectTransform != null)
    {
        // Sposta il joystick fuori dallo schermo aggiungendo un valore grande abbastanza all'anchoredPosition
        joystickRectTransform.anchoredPosition += new Vector2(2000, 0); // Modifica questo valore in base alle tue necessità
    }

ShowEliminatedText();

    }
        }
         
                     int totalPlayers = instantiatedCars.Count;
            int position = instantiatedCars.Count - eliminatedPlayers.Count + 1;
               int coinsEarned = AssignCoinsToPlayer(totalPlayers, position);

            // Aggiorna il testo per mostrare le monete guadagnate
            if (coinsAwardText != null) {
                coinsAwardText.text = $"+{coinsEarned}";
            }
            ShowEliminationResults(player);
}

           
            CheckForWinner();

            }
        

        
    }


private int AssignCoinsToPlayer(int totalPlayers, int position)
{
    RewardTier tier = rewardTiers.FirstOrDefault(t => t.playerCount == totalPlayers);
    if (tier != null && position - 1 < tier.rewards.Length)
    {
        int coins = tier.rewards[position - 1];
        CurrencyManager.Instance.ModifyCoins(coins);
        return coins; // Restituisce il numero di monete guadagnate
    }
    return 0; // Restituisce 0 se non ci sono monete da assegnare
}

    void ShowEliminationResults(GameObject eliminatedPlayer)
    {
        PhotonView eliminatedPlayerView = eliminatedPlayer.GetComponent<PhotonView>();

        if (eliminatedPlayerView != null)
        {
            int place = instantiatedCars.Count - eliminatedPlayers.Count + 1;

            // Ottieni l'ID del giocatore da PhotonView
            string playerId = eliminatedPlayerView.Owner.UserId;

            // Registra la posizione del giocatore nel dizionario
            playerPositions[playerId] = place;

            // Mostra i risultati solo al giocatore eliminato
            if (eliminatedPlayerView.IsMine)
            {
                string playerName = eliminatedPlayerView.Owner.NickName;
                string results = $" {place}  .  {playerName}--{carFinishTimes[eliminatedPlayer]:F2}";
                resultsText.text = results;
                resultsPanel.SetActive(true);
                RectTransform abilityRectTransform = abilityButtons.GetComponent<RectTransform>();
if (abilityRectTransform != null)
{
    abilityRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
                returnToLobbyButton.gameObject.SetActive(true);
RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
if (hbRectTransform != null)
{
    hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
RectTransform tbRectTransform = throttleButton.GetComponent<RectTransform>();
if (tbRectTransform != null)
{
    tbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
RectTransform rbRectTransform = reverseButton.GetComponent<RectTransform>();
if (rbRectTransform != null)
{
    rbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
            }
        }
    }

   void CheckForWinner()
{
                    Debug.Log("checkforwinnerchiamato");

    // Conta quanti giocatori, inclusi AI, sono rimasti nel gioco.
    int remainingPlayers = instantiatedCars.Count(car => !eliminatedPlayers.Contains(car));

    if (remainingPlayers == 1)
    {
        // Trova l'ultimo giocatore o AI rimasto.
        GameObject lastStanding = instantiatedCars.Except(eliminatedPlayers).FirstOrDefault();

        // Se ce un ultimo rimasto, annuncia che ha vinto.
        if (lastStanding != null )
        {
            PhotonView pv = lastStanding.GetComponent<PhotonView>();
            if (pv != null && pv.IsMine)
            {
                // Assicurati che questo annuncio sia visto solo dal giocatore reale e non dagli altri.
                photonView.RPC("AnnounceWinner", RpcTarget.All, pv.ViewID);
                mainCamera.gameObject.SetActive(true);
                finishPlane.SetActive(true);
            }
                           if (playerWinPrefab != null && finishPrefabspawnPoint != null)
{
    Instantiate(playerWinPrefab, finishPrefabspawnPoint.position, finishPrefabspawnPoint.rotation);
}

        }
    }

}


    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Il giocatore " + otherPlayer.NickName + " ha lasciato la stanza.");

        GameObject playerCar = instantiatedCars.FirstOrDefault(car => car.GetComponent<PhotonView>().Owner == otherPlayer);
        if (playerCar != null && !eliminatedPlayers.Contains(playerCar))
        {
            eliminatedPlayers.Add(playerCar);
            carFinishTimes[playerCar] = Time.time - raceStartTime; // Opzionale
        }

        CheckForWinner();
    }

    [PunRPC]
    void AnnounceWinner(int winnerViewID)
    {
        GameObject winner = PhotonView.Find(winnerViewID).gameObject;
                string language = PlayerPrefs.GetString("Language", "en"); // Ottieni la lingua corrente

        if (winner != null)
        {
            // Mostra il messaggio del vincitore solo al giocatore vincente
            if (PhotonNetwork.LocalPlayer == winner.GetComponent<PhotonView>().Owner)
            {
 
            // Assegna le monete al giocatore solo se non è un bot
            if (!winner.CompareTag("Enemy"))
            {
        if (language == "it")
        {
                   resultsText.text = "Hai vinto!";
        }
           if (language == "en")
        {
                   resultsText.text = "You win!";
        }
           if (language == "es")
        {
                   resultsText.text = "¡Has ganado!";
        }
           if (language == "fr")
        {
                   resultsText.text = "Tu as gagné!";
        }
                        ExperienceManager.Instance.AddExperience(experienceWinnerGain);

                int totalPlayers = instantiatedCars.Count; // Presumibilmente tutti quelli che hanno partecipato alla gara
                int position = 1; // La posizione del vincitore
                int coinsEarned = AssignCoinsToPlayer(totalPlayers, position);

                // Aggiorna il testo sul pannello dei risultati per mostrare le monete guadagnate
                if (coinsAwardText != null)
                {
                    coinsAwardText.text = $"+{coinsEarned}";
                }
                ShowWinText();
            }
                           if (winner.CompareTag("Enemy"))
            {
                Text playerNameText = winner.transform.Find("PlayerName").GetComponent<Text>();
                       if (language == "it")
        {
                resultsText.text = playerNameText.text + " ha vinto";
        }
                           if (language == "en")
        {
                resultsText.text = playerNameText.text + " won";
        }
                           if (language == "es")
        {
                resultsText.text = playerNameText.text + " ganó";
        }
                           if (language == "fr")
        {
                resultsText.text = playerNameText.text + " a gagné";
        }
            }
                    resultsPanel.SetActive(true);
                   RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
            if (hbRectTransform != null)
            {
                      hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
            }
                RectTransform tbRectTransform = throttleButton.GetComponent<RectTransform>();
              if (tbRectTransform != null)
        {
             tbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
            }
                RectTransform rbRectTransform = reverseButton.GetComponent<RectTransform>();
                    if (rbRectTransform != null)
                    {
                          rbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
                            }
                        

                         }
            else
            {
                          if (language == "it")
        {
                resultsText.text = winner.GetComponent<PhotonView>().Owner.NickName + " ha vinto!";
        }
                           if (language == "en")
        {
                resultsText.text = winner.GetComponent<PhotonView>().Owner.NickName + " won!";
        }
                           if (language == "es")
        {
                resultsText.text = winner.GetComponent<PhotonView>().Owner.NickName + "ganò";
        }
                           if (language == "fr")
        {
                resultsText.text = winner.GetComponent<PhotonView>().Owner.NickName + "a gagnè";
        }
                resultsPanel.SetActive(true);
RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
if (hbRectTransform != null)
{
    hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
RectTransform tbRectTransform = throttleButton.GetComponent<RectTransform>();
if (tbRectTransform != null)
{
    tbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
RectTransform rbRectTransform = reverseButton.GetComponent<RectTransform>();
if (rbRectTransform != null)
{
    rbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}


            }

            // Inizia il processo di ritorno alla lobby dopo un breve ritardo
            StartCoroutine(ReturnToLobbyAfterDelay(10)); // Ritardo di 5 secondi
        }
    }
private void ShowEliminatedText()
    {
        if (eliminatedTextPrefab != null && canvas != null)
        {
            // Istanziare il prefab come figlio del Canvas
            GameObject instance = Instantiate(eliminatedTextPrefab, canvas.transform, false);

            // Imposta la posizione usando RectTransform
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-200, 20); // Centrato e leggermente a sinistra

            // Distruggere il prefab dopo un certo numero di secondi
            Destroy(instance, 5); // Regolabile
        }
        else
        {
            Debug.LogError("Prefab o Canvas non assegnati correttamente.");
        }
        RectTransform abilityRectTransform = abilityButtons.GetComponent<RectTransform>();
if (abilityRectTransform != null)
{
    abilityRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
    }

    private void ShowWinText()
    {
        if (winTextPrefab != null && canvas != null)
        {
            // Istanziare il prefab come figlio del Canvas
            GameObject instance = Instantiate(winTextPrefab, canvas.transform, false);

            // Imposta la posizione usando RectTransform
            RectTransform rectTransform = instance.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(-200, 20); // Centrato e leggermente a sinistra

            // Distruggere il prefab dopo un certo numero di secondi
            Destroy(instance, 5); // Regolabile
        }
        else
        {
            Debug.LogError("Prefab o Canvas non assegnati correttamente.");
        }
        RectTransform abilityRectTransform = abilityButtons.GetComponent<RectTransform>();
if (abilityRectTransform != null)
{
    abilityRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}
    }

    IEnumerator TimerSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        audioSource.Play();
    }

    IEnumerator ReturnToLobbyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToLobby();
    }

    public void ReturnToLobby()
    {
        SavePlayerPositions();
        StartCoroutine(LeaveRoomAndLoadLobby());
    }
    private void SavePlayerPositions()
    {
        // Converte il dizionario in una stringa JSON e lo salva
        string positionsJson = JsonUtility.ToJson(playerPositions);
        PlayerPrefs.SetString("PlayerPositions", positionsJson);
    }
    private IEnumerator LeaveRoomAndLoadLobby()
    {
        PhotonNetwork.LeaveRoom();

        while (PhotonNetwork.InRoom)
        {
            yield return null; // Aspetta che il giocatore lasci la stanza
        }

        PhotonNetwork.Disconnect();

        while (PhotonNetwork.IsConnected)
        {
            yield return null; // Aspetta che il giocatore sia completamente disconnesso
        }
    transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadLobbyScene", 1f);
    }
void LoadLobbyScene()
{
SceneManager.LoadScene("DemoAsteroids-LobbyScene");}
[PunRPC]
public void DestroyPlayer(int viewID)
{
    PhotonView pv = PhotonView.Find(viewID);
    if (pv != null && pv.gameObject != null)
    {
        PhotonNetwork.Destroy(pv.gameObject);
    }
}
[PunRPC]
void SyncStartPosition()
{
    for (int i = 0; i < instantiatedCars.Count; i++)
    {
        // Trova l'ActorNumber del proprietario dell'auto.
        int actorNumber = instantiatedCars[i].GetComponent<PhotonView>().Owner.ActorNumber;
        
        // Calcola l'indice dello spawn point basandosi sull'ActorNumber.
        int spawnIndex = (actorNumber - 1) % startLineSpawnPoints.Length;
        
        // Posiziona l'auto al suo spawn point designato.
        instantiatedCars[i].transform.position = startLineSpawnPoints[spawnIndex].position;
        instantiatedCars[i].transform.rotation = startLineSpawnPoints[spawnIndex].rotation;
        
        Debug.Log($"Auto {instantiatedCars[i].name} posizionata allo spawn point {spawnIndex}.");
    }
}



    [PunRPC]
    void StartRaceSync()
    {
        raceStartTime = Time.time;
        Debug.Log("Gara iniziata. Tempo di inizio della gara: " + raceStartTime);

        foreach (GameObject car in instantiatedCars)
        {
            var carController = car.GetComponent<PrometeoCarController>();
            if (carController != null)
            {
                carController.EnableControls();
                Debug.Log("Controlli abilitati per: " + car.name);
            }
        }
       /*         // Trova tutti i GameObject con il tag "Enemy" nella scena
    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

    // Per ogni "Enemy", cerca il componente FakePlayerCarController e attiva la variabile
    foreach (GameObject enemy in enemies)
    {
        FakePlayerCarController controller = enemy.GetComponent<FakePlayerCarController>();
        if (controller != null)
        {
            controller.raceStarted = true; 
        }
    }*/
    }


}
