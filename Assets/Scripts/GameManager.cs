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
    public float preRaceTimer = 7.0f; // Tempo prima dell'inizio della gara
    public Transform[] spawnPoints; // Punti di spawn delle macchine
    public Transform[] startLineSpawnPoints;
    public float startRaceTimer = 5.0f; // Timer per l'inizio della gara
    public GameObject resultsPanel; // Pannello per i risultati
    public Text resultsText;
    public Button returnToLobbyButton; // Pulsante per tornare alla lobby
    public Text countdownText;
    public GameObject brickWallPrefab; // Prefab del muro di mattoni
    public Transform[] brickWallSpawnPoints; // Punti di spawn del muro di mattoni
public Camera mainCamera;
public Image handbrakeButton; 

    private List<GameObject> instantiatedCars = new List<GameObject>(); // Lista delle macchine istanziate
    private List<GameObject> finishedCars = new List<GameObject>(); // Macchine che hanno finito la gara
    private Dictionary<GameObject, float> carFinishTimes = new Dictionary<GameObject, float>(); // Tempi di arrivo delle macchine
    private float raceStartTime; // Tempo di inizio della gara
    private List<GameObject> eliminatedPlayers = new List<GameObject>();
    private GameObject lastPlayerStanding;
    private Dictionary<string, int> playerPositions = new Dictionary<string, int>();

  
    public Dictionary<string, GameObject> carPrefabs;



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

    // Verifica se il prefab è stato caricato correttamente
    if (carPrefab == null)
    {
        Debug.LogError("Impossibile caricare il prefab della macchina: " + selectedCarName);
        return;
    }

    // Verifica se Photon è connesso e pronto
    if (!PhotonNetwork.IsConnectedAndReady)
    {
        Debug.LogError("Photon non è connesso e pronto.");
        return;
    }

    // Procedi con l'istanziazione della macchina
    int spawnIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
    if (spawnIndex < spawnPoints.Length)
    {
        GameObject car = PhotonNetwork.Instantiate(carPrefab.name, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
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

    StartCoroutine(PreRaceCountdown());
    resultsPanel.SetActive(false);
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
        // Attendi che tutte le auto siano istanziate.
        countdownText.text = "Attendendo altri giocatori...";
        while (PhotonNetwork.CurrentRoom != null && instantiatedCars != null && PhotonNetwork.CurrentRoom.PlayerCount != instantiatedCars.Count)
        {
            yield return null; // Aspetta un frame prima di controllare nuovamente.
        }

        if (PhotonNetwork.CurrentRoom == null)
        {
            countdownText.text = ""; // Pulisci il testo se la stanza non esiste più.
            yield break; // Fermati se la stanza non esiste più.
        }

        countdownText.text = "Preparati!";

        // Countdown prima dell'inizio della gara.
        float timer = preRaceTimer;
        while (timer > 0)
        {
            countdownText.text = $"{timer:F2}";
            timer -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = "Via!";
        yield return new WaitForSeconds(1f); // Breve pausa prima di iniziare la gara.

        photonView.RPC("SyncStartPosition", RpcTarget.All);

        // Aggiungi il countdown per l'inizio della gara
        float startTimer = startRaceTimer;
        while (startTimer > 0)
        {
            countdownText.text = $"{startTimer:F2}";
            startTimer -= Time.deltaTime;
            yield return null;
        }

        countdownText.text = ""; // Nasconde il countdown una volta che la gara inizia.
        photonView.RPC("StartRaceSync", RpcTarget.All);
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

            PhotonView playerPhotonView = player.GetComponent<PhotonView>();

            if (playerPhotonView != null && playerPhotonView.Owner != null)
            {
                string playerName = playerPhotonView.Owner.NickName;
                Debug.Log(playerName + " è stato eliminato. Tempo: " + carFinishTimes[player]);
            }
            else
            {
                Debug.Log(player.name + " è stato eliminato. Tempo: " + carFinishTimes[player]);
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
    }
        

            int totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
            int position = instantiatedCars.Count - eliminatedPlayers.Count + 1;
            AssignCoinsToPlayer(totalPlayers, position);
            CheckForWinner();
            // Mostra i risultati per il giocatore eliminato
            ShowEliminationResults(player);
        }
    }
    private void AssignCoinsToPlayer(int totalPlayers, int position)
    {
        RewardTier tier = rewardTiers.FirstOrDefault(t => t.playerCount == totalPlayers);
        if (tier != null && position - 1 < tier.rewards.Length)
        {
            int coins = tier.rewards[position - 1];
            CurrencyManager.Instance.ModifyCoins(coins);
        }
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
                string results = $"Sei stato eliminato. Posizione: {place}. {playerName} - Tempo: {carFinishTimes[eliminatedPlayer]:F2} secondi";
                resultsText.text = results;
                resultsPanel.SetActive(true);
                returnToLobbyButton.gameObject.SetActive(true);
RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
if (hbRectTransform != null)
{
    hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}


            }
        }
    }

    void CheckForWinner()
    {
        // Verifica se c'è solo un giocatore rimasto
        if (eliminatedPlayers.Count == instantiatedCars.Count - 1)
        {
            lastPlayerStanding = instantiatedCars.Except(eliminatedPlayers).FirstOrDefault();

            // Mostra il messaggio del vincitore e inizia il processo di ritorno alla lobby
            if (lastPlayerStanding != null)
            {
                photonView.RPC("AnnounceWinner", RpcTarget.All, lastPlayerStanding.GetComponent<PhotonView>().ViewID);
            }
        }
        // Aggiungi qui il controllo per un singolo giocatore 
        else if (instantiatedCars.Count == 1 && playerPositions.Count == 0)
        {
            GameObject singlePlayer = instantiatedCars.FirstOrDefault();
            if (singlePlayer != null)
            {
                PhotonView playerView = singlePlayer.GetComponent<PhotonView>();
                if (playerView != null)
                {
                    string playerId = playerView.Owner.UserId;
                    playerPositions[playerId] = 1; // Assegna la prima posizione
                    SavePlayerPositions(); // Salva la posizione
                    photonView.RPC("AnnounceWinner", RpcTarget.All, playerView.ViewID);
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

        if (winner != null)
        {
            // Mostra il messaggio del vincitore solo al giocatore vincente
            if (PhotonNetwork.LocalPlayer == winner.GetComponent<PhotonView>().Owner)
            {
                resultsText.text = "Hai vinto!";
                resultsPanel.SetActive(true);
RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
if (hbRectTransform != null)
{
    hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}



            }
            else
            {
                resultsText.text = winner.GetComponent<PhotonView>().Owner.NickName + " ha vinto!";
                resultsPanel.SetActive(true);
RectTransform hbRectTransform = handbrakeButton.GetComponent<RectTransform>();
if (hbRectTransform != null)
{
    hbRectTransform.anchoredPosition += new Vector2(2000, 0); // Aggiungi un valore grande abbastanza per spostarlo fuori dallo schermo
}



            }

            // Inizia il processo di ritorno alla lobby dopo un breve ritardo
            StartCoroutine(ReturnToLobbyAfterDelay(5)); // Ritardo di 5 secondi
        }
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

        SceneManager.LoadScene("DemoAsteroids-LobbyScene"); // Nome della tua scena della lobby
    }

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
        Debug.Log("Sincronizzazione della posizione di partenza per " + instantiatedCars.Count + " auto.");

        for (int i = 0; i < instantiatedCars.Count; i++)
        {
            if (i < startLineSpawnPoints.Length)
            {
                Debug.Log("Posizionamento dell'auto: " + instantiatedCars[i].name + " allo spawn point della linea di partenza " + i);
                instantiatedCars[i].transform.position = startLineSpawnPoints[i].position;
                instantiatedCars[i].transform.rotation = startLineSpawnPoints[i].rotation;
            }
            else
            {
                Debug.LogError("Non ci sono abbastanza spawn points sulla linea di partenza per tutte le auto. Auto non posizionata: " + instantiatedCars[i].name);
            }
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
    }


}
