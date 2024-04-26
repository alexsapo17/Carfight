using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine.SceneManagement;

namespace Photon.Pun.Demo.Asteroids
{
    public class LobbyMainPanel : MonoBehaviourPunCallbacks
    {
        [Header("Login Panel")]
        public GameObject LoginPanel;

        public InputField PlayerNameInput;

        [Header("Selection Panel")]
        public GameObject SelectionPanel;

        [Header("Create Room Panel")]
        public GameObject CreateRoomPanel;
        [Header("Connecting Panel")]
public GameObject ConnectingPanel;
        public InputField RoomNameInputField;
        public InputField MaxPlayersInputField;

        [Header("Join Random Room Panel")]
        public GameObject JoinRandomRoomPanel;

        [Header("Room List Panel")]
        public GameObject RoomListPanel;

        public GameObject RoomListContent;
        public GameObject RoomListEntryPrefab;

        [Header("Inside Room Panel")]
        public GameObject InsideRoomPanel;

        public Button StartGameButton;
        public GameObject PlayerListEntryPrefab;
   public Button ShopButton;
    public Button SinglePlayerButton;
    public Button LoginButton;
    public GameObject TutorialPanel;
public GameObject Interactable1Panel;
public GameObject Interactable2Panel;

public GameObject Interactable3Panel;

public Animator transitionAnimator;

public GameObject NoCoinsPanel;
public GameObject NoCarSelectedPanel;
public GameObject tutorial2Panel;
public GameObject creditsPanel;
public GameObject eliminateAccountPanel;
public GameObject eliminateAccountPanel2;

 public GameObject[] objectsToDisable;
        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;
        private FirebaseAuth auth;
        private DatabaseReference databaseReference;
        private bool isRandomRoom = false;
        private bool isDuelRoom = false;
    private Coroutine startGameCoroutine;

        #region UNITY

public void Awake()
{
    PhotonNetwork.AutomaticallySyncScene = true;
    InitializeFirebase();
    cachedRoomList = new Dictionary<string, RoomInfo>();
    roomListEntries = new Dictionary<string, GameObject>();

    if (PlayerPrefs.HasKey("PlayerID"))
    {
        string playerId = PlayerPrefs.GetString("PlayerID");
        PhotonNetwork.NickName = playerId;
    }
    else
    {
        SetActivePanel(LoginPanel.name);
        PlayerNameInput.text = "Player " + Random.Range(1000, 10000);
    }
           if (!PlayerPrefs.HasKey("FirstTimeLoginCompleted"))
            {
            // Disattiva tutti gli oggetti nell'array
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }
                }
    // Trova il CurrencyManager e aggiorna l'UI delle monete
    UpdateCurrencyUI();
}
public void Start()
{

        // Controlla se dobbiamo mostrare il pannello speciale
    if (PlayerPrefs.GetInt("ShowTutorial2Panel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorial2Panel.SetActive(true);
     // Disattiva tutti gli oggetti nell'array
            foreach (GameObject obj in objectsToDisable)
            {
                obj.SetActive(false);
            }

    }
             

    UpdateCurrencyUI();
}
     public void SetLanguage(string language)
    {
        PlayerPrefs.SetString("Language", language);
        PlayerPrefs.Save();

        // Trova tutti i componenti LocalizedText nella scena, inclusi quelli disattivati
        LocalizedText[] allLocalizedTextComponents = FindObjectsOfType<LocalizedText>(true);

        // Itera attraverso tutti i componenti LocalizedText e chiama UpdateTextLanguage su ognuno di essi
        foreach (LocalizedText localizedText in allLocalizedTextComponents)
        {
            localizedText.UpdateTextLanguage();
        }
    }
 private void UpdateCurrencyUI()
        {
            CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
            if (currencyManager != null)
            {
                // Aggiorna UI delle monete
                UpdateCoinsUI(currencyManager);

                // Aggiorna UI delle gemme
                UpdateGemsUI(currencyManager);
            }
            else
            {
                Debug.LogError("CurrencyManager non trovato nella scena.");
            }
        }

        private void UpdateCoinsUI(CurrencyManager currencyManager)
        {
            Text coinTextComponent = GameObject.Find("CoinsText").GetComponent<Text>();
            if (coinTextComponent != null)
            {
                currencyManager.coinsText = coinTextComponent;
                currencyManager.UpdateCoinsUI(); 
            }
            else
            {
                Debug.LogError("Componente Text UI per le monete non trovato.");
            }
        }

        private void UpdateGemsUI(CurrencyManager currencyManager)
        {
            Text gemsTextComponent = GameObject.Find("GemsText").GetComponent<Text>();
            if (gemsTextComponent != null)
            {
                currencyManager.gemsText = gemsTextComponent;
                currencyManager.UpdateGemsUI();
            }
            else
            {
                Debug.LogError("Componente Text UI per le gemme non trovato.");
            }
        }
        #endregion

        #region PUN CALLBACKS




        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateCachedRoomList(roomList);
            UpdateRoomListView();
        }

        public override void OnJoinedLobby()
        {
            // whenever this joins a new lobby, clear any previous room lists
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        // note: when a client joins / creates a room, OnLeftLobby does not get called, even if the client was in a lobby before
        public override void OnLeftLobby()
        {
            cachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            SetActivePanel(SelectionPanel.name);
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            string roomName = "Room " + Random.Range(1000, 10000);
if (isDuelRoom == true)
{
    RoomOptions options = new RoomOptions { MaxPlayers = 2 };
    options.CustomRoomPropertiesForLobby = new string[] { "IsDuelRoom" };
    options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "IsDuelRoom", true } };
    PhotonNetwork.CreateRoom(roomName, options, null);
} 
else
{
    RoomOptions options = new RoomOptions { MaxPlayers = 6 };
    options.CustomRoomPropertiesForLobby = new string[] { "IsDuelRoom" };
    options.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "IsDuelRoom", false } };
    PhotonNetwork.CreateRoom(roomName, options, null);
}


        }
        private void InitializeFirebase()
        {
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
        }

        private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {

      if (auth.CurrentUser != null)
    {
        // L'utente è autenticato, recupera il nickname e carica le monete
        RetrieveNickname();
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null)
        {
            currencyManager.LoadCoins();
            currencyManager.UpdateCoinsUI();
                        currencyManager.LoadGems();
            currencyManager.UpdateGemsUI();
        }
        else
        {
            Debug.LogError("CurrencyManager non trovato nella scena.");
        }
    }
            else
            {
                // L'utente non è autenticato, verifica se è la prima volta che accede al gioco
                     if (!PlayerPrefs.HasKey("PlayerID"))
        {
            // È la prima volta, mostra l'input field e modifica i pulsanti
            SetActivePanel(LoginPanel.name);
            PlayerNameInput.gameObject.SetActive(true);
            PlayerNameInput.text = "";
    TutorialPanel.SetActive(true);
    Interactable1Panel.SetActive(true);
        Interactable2Panel.SetActive(true);

    Interactable3Panel.SetActive(true);

            // Nascondi i pulsanti Shop e SinglePlayer
            ShopButton.gameObject.SetActive(false);
            SinglePlayerButton.gameObject.SetActive(false);
            string language = PlayerPrefs.GetString("Language", "en"); // Ottieni la lingua corrente

            // Cambia il testo del pulsante Login in "Procedi"  
        if (language == "it")
        {
            LoginButton.GetComponentInChildren<Text>().text = "Procedi";
        }
                if (language == "en")
        {
            LoginButton.GetComponentInChildren<Text>().text = "Proceed";
        }
                if (language == "es")
        {
            LoginButton.GetComponentInChildren<Text>().text = "Proceder";
        }
                if (language == "fr")
        {
            LoginButton.GetComponentInChildren<Text>().text = "Procèder";
        }
        
                }
                else
                {
                    // Non è la prima volta, ma l'utente non è autenticato su Firebase
                    // Potresti voler gestire questo caso (ad esempio, un utente disconnesso)
                    SetActivePanel(LoginPanel.name);
                    PlayerNameInput.gameObject.SetActive(false);
                }
            }
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

public void OnSinglePlayerButtonClicked()
{
    transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadSinglePlayerScene", 1f); }
public void LoadSinglePlayerScene()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("SinglePlayerScene");
}

public void OnOnlineButtonClicked()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
}
public void OnLogoutButtonClicked()
{
    if (auth.CurrentUser != null)
    {
        auth.SignOut();

        // Opzionalmente, aggiorna l'interfaccia utente o reindirizza l'utente a un altro pannello, ad esempio il pannello di login.
        SetActivePanel(LoginPanel.name);
    }
}

public void OnCreditsButtonClicked()
{
creditsPanel.SetActive(true);
}
public void OnBackCreditsButtonClicked()
{
creditsPanel.SetActive(false);
}

public void OnEliminateAccountButtonClicked()
{
eliminateAccountPanel.SetActive(true);
}
public void OnBackEliminateAccountButtonClicked()
{
eliminateAccountPanel.SetActive(false);
}
public void OnEliminateAccountButtonClicked2()
{
eliminateAccountPanel2.SetActive(true);
}

   
        private void RetrieveNickname()
        {
            string userId = auth.CurrentUser.UserId;
            databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("RetrieveNickname encountered an error: " + task.Exception);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot.Value != null)
                    {
                        string nickname = snapshot.Value.ToString();
                        PlayerNameInput.text = nickname;
                        PlayerNameInput.gameObject.SetActive(false);

                        // Aggiorna il nickname in Photon con quello recuperato da Firebase
                        PhotonNetwork.NickName = nickname;


                    }
                    else
                    {
                        Debug.LogError("Nickname not found in the database.");
                    }
                }
            });
        }

private void SaveNickname(string nickname)
{
    string userId = auth.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("nickname").SetValueAsync(nickname).ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Error saving nickname: " + task.Exception);
        }
        else if (task.IsCompleted)
        {
            // Nascondi l'input field del nickname
            PlayerNameInput.gameObject.SetActive(false);
            // Controlla se è la prima volta che l'utente accede al gioco
            if (!PlayerPrefs.HasKey("FirstTimeLoginCompleted"))
            {
                // Imposta il flag per indicare che il primo login è stato completato
                PlayerPrefs.SetInt("FirstTimeLoginCompleted", 1);
                // Imposta il flag per mostrare il pannello speciale dopo il riavvio
                PlayerPrefs.SetInt("ShowTutorial2Panel", 1);

                // Ricarica la scena
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
            else
            {
                // Se non è la prima volta, connetti a Photon
                ConnectToPhoton();
            }
        }
    });
}

   public override void OnConnectedToMaster()
{
 
        this.SetActivePanel(SelectionPanel.name);
        UpdateCurrencyUI();
    
}
public void OnLoginButtonClicked()
{
    string playerName = PlayerNameInput.text;
    

    if (!string.IsNullOrEmpty(playerName))
    {
        if (auth.CurrentUser != null)
        {
            // Salva il nickname
            SaveNickname(playerName);


        }
        else
        {
            RegisterOrLoginUser(playerName);
            // Non connettere a Photon qui, sarà gestito dopo il login/registrazione
        }
    }
    else
    {
        Debug.LogError("Player Name is invalid.");
        return;
    }
    this.SetActivePanel(ConnectingPanel.name);
}

        private void RegisterOrLoginUser(string playerName)
        {
            if (auth.CurrentUser == null)
            {
                // L'utente non è autenticato, effettua l'accesso anonimo
                auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError("SignInAnonymouslyAsync was canceled or encountered an error: " + task.Exception);
                        // Qui puoi gestire l'errore di login
                    }
                    else if (task.IsCompleted)
                    {
                        // L'autenticazione è riuscita, l'utente ha un ID univoco
                        Firebase.Auth.FirebaseUser newUser = task.Result.User; // Cambiato qui
                        // Associare il nickname all'ID univoco dell'utente
                        SaveNickname(playerName);
                        UpdateCurrencyUI();
                    }
                });
            }
            else
            {
                // L'utente è già autenticato, procedi come necessario
                SaveNickname(playerName);
UpdateCurrencyUI();
            }
        }
        public void OnResetButtonClicked()
        {
            // Cancella tutti i PlayerPrefs (attenzione, questo rimuove tutti i dati salvati)
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();

            // Disconnetti l'utente da Firebase
            if (auth.CurrentUser != null)
            {
                // Opzionale: Rimuovi i dati dell'utente da Firebase Database, se necessario
                var userId = auth.CurrentUser.UserId;
                databaseReference.Child("users").Child(userId).RemoveValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsFaulted || task.IsCanceled)
                    {
                        Debug.LogError("Error removing user data: " + task.Exception);
                    }
                    else
                    {
                        Debug.Log("User data successfully removed.");
                    }
                });

                // Disconnetti l'utente
                auth.SignOut();
            }


        }

        #endregion

        #region UI CALLBACKS

        public void OnBackButtonClicked()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SetActivePanel(SelectionPanel.name);
        }
    
              public void OnBackButtonSelectionPanelClicked()
        {
   
PhotonNetwork.Disconnect();
            SetActivePanel(LoginPanel.name);
        }


        public void OnCreateRoomButtonClicked()
        {
            isRandomRoom = false;
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 6);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

      public void OnJoinRandomRoomButtonClicked()
{
    // Verifica se c'è una macchina selezionata nelle PlayerPrefs
    if (!PlayerPrefs.HasKey("SelectedCar"))
    {
        // Mostra un pannello o gestisci il caso in cui non ci sia una macchina selezionata
NoCarSelectedPanel.SetActive(true);
       return;
    }
isDuelRoom= false;
    // Aggiorna il nickname di Photon prima di unirsi alla stanza
    if (auth.CurrentUser != null)
    {
        string userId = auth.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string nickname = task.Result.Value.ToString();
                PhotonNetwork.NickName = nickname;
            }
        });
    }

    // Trova il CurrencyManager nella scena
    CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
    if (currencyManager == null)
    {
        Debug.LogError("CurrencyManager non trovato.");
        return;
    }

    int entryCost = 2000; // Costo per unirsi alla stanza
    if (currencyManager.HasEnoughCoins(entryCost))
    {
        SetActivePanel(JoinRandomRoomPanel.name);
        PhotonNetwork.JoinRandomRoom(new Hashtable() { { "IsDuelRoom", false } }, 0);
        isRandomRoom = true;
    }
    else
    {
NoCoinsPanel.SetActive(true);
            // Disattiva il pannello delle monete dopo 3 secondi
        Invoke("DisableNoCoinsPanel", 3f);
        
    }
}

   public void RandomRoomButtonClickedDuel()
{ 
    // Verifica se c'è una macchina selezionata nelle PlayerPrefs
    if (!PlayerPrefs.HasKey("SelectedCar"))
    {
        // Mostra un pannello o gestisci il caso in cui non ci sia una macchina selezionata
NoCarSelectedPanel.SetActive(true);
       return;
    }
isDuelRoom= true;
    // Aggiorna il nickname di Photon prima di unirsi alla stanza
    if (auth.CurrentUser != null)
    {
        string userId = auth.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("nickname").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                string nickname = task.Result.Value.ToString();
                PhotonNetwork.NickName = nickname;
            }
        });
    }

    // Trova il CurrencyManager nella scena
    CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
    if (currencyManager == null)
    {
        Debug.LogError("CurrencyManager non trovato.");
        return;
    }

    int entryCost = 1000; // Costo per unirsi alla stanza
    if (currencyManager.HasEnoughCoins(entryCost))
    {
        SetActivePanel(JoinRandomRoomPanel.name);
        PhotonNetwork.JoinRandomRoom(new Hashtable() { { "IsDuelRoom", true } }, 0);
        isRandomRoom = true;
    }
    else
    {
NoCoinsPanel.SetActive(true);
            // Disattiva il pannello delle monete dopo 3 secondi
        Invoke("DisableNoCoinsPanel", 3f);
        
    }
}
private void DisableNoCoinsPanel()
{
    NoCoinsPanel.SetActive(false);
}




        public void OnStartGameButtonClicked()
        {

                transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("StartGame", 1f); // Sostituisce la coroutine con Invoke

        }
// Metodo separato per caricare la scena, chiamato dopo il ritardo
void StartGame()
{

            CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
            if (currencyManager == null)
            {
                Debug.LogError("CurrencyManager non trovato.");
                return;
            }
if (isDuelRoom==false)
{
            int entryCost = 2000; // Costo per avviare il gioco
              if (currencyManager.TrySpendCoins(entryCost))
            {
                    // Assicurati che solo il Master Client avvii la partita per evitare avvii multipli
  if (PhotonNetwork.IsMasterClient)
    {

    if (PhotonNetwork.IsMasterClient)
    {
  
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameScene");
    }
            
        }
 
            }
            else
            {
                Debug.LogError("Non hai abbastanza monete per avviare il gioco.");
                // Gestisci il caso in cui il giocatore non ha abbastanza monete
            }
} 
else
{
            int entryCost = 1000; // Costo per avviare il gioco
              if (currencyManager.TrySpendCoins(entryCost))
            {
                    // Assicurati che solo il Master Client avvii la partita per evitare avvii multipli
  if (PhotonNetwork.IsMasterClient)
    {

    if (PhotonNetwork.IsMasterClient)
    {
  
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameScene");
    }
            
        }
 
            }
            else
            {
                Debug.LogError("Non hai abbastanza monete per avviare il gioco.");
                // Gestisci il caso in cui il giocatore non ha abbastanza monete
            }
}
          
            }

        public void OnLeaveGameButtonClicked()
        {
            PhotonNetwork.LeaveRoom();
        }


        private void ConnectToPhoton()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                // Opzionale: se sei già connesso e per esempio vuoi aggiornare l'interfaccia utente o fare altro
            }
        }

        public void OnRoomListButtonClicked()
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }

            SetActivePanel(RoomListPanel.name);
        }


public override void OnJoinedRoom()
        {
            // joining (or entering) a room invalidates any cached lobby room list (even if LeaveLobby was not called due to just joining a room)
            cachedRoomList.Clear();
UpdateCurrencyUI();

            SetActivePanel(InsideRoomPanel.name);

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                GameObject entry = Instantiate(PlayerListEntryPrefab);
  entry.transform.SetParent(InsideRoomPanel.transform, false);
entry.transform.localPosition = Vector3.zero; // Imposta la posizione locale a zero per centrarlo nel genitore
entry.transform.localScale = Vector3.one; 
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

 // Controlla se la stanza è casuale e aggiorna di conseguenza
    if (isRandomRoom)
    {
        // Nascondi il pulsante "Start Game" nelle stanze casuali
        StartGameButton.gameObject.SetActive(false);
        
        // Avvia il timer solo se ci sono almeno due giocatori
        UpdateGameStartCondition();
    }
    else
    {
        // Mostra il pulsante "Start Game" nelle stanze private
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
    /*
      // Controlla se ci sono giocatori finti salvati nelle proprietà della stanza e aggiungili
    object fakePlayersObject;
    if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("fakePlayers", out fakePlayersObject) && PhotonNetwork.IsMasterClient)
    {
        string[] fakePlayers = (string[])fakePlayersObject;
        foreach (string fakePlayerName in fakePlayers)
        {
            AddFakePlayerToList(fakePlayerName);
        }
    }
            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            StartCoroutine(AddFakePlayersDynamically());*/
        }
        /*
private System.Collections.IEnumerator AddFakePlayersDynamically()
{
    if (PhotonNetwork.IsMasterClient)
    {
        int fakePlayersAdded = 0;
        List<string> fakePlayersNames = new List<string>();

        // Lista dei nomi unici e creativi forniti
        List<string> creativeNicknames = new List<string> {
            "Fire-Bred", "Titanic", "XXHurricaneXX", "AMan", "Iron-Cut3", "Tempest", "IronG4YMan", "Steel11111",
            "Pursuit99", "SteeFrix13", "Sick Rebellious Names", "Upsurge", "UpCupMud", "Overthrow", "Breaker",
            "SabotageJake", "Dissentery", "SubvAscular", "Rebell", "Insurgent", "AnAccidentalGenius", "4cidJim",
            "AdmiralotBBBBOy", "AgentHercules", "HubHobo", "AlleyMan", "Alpha17", "AlphaReturns", "IMYOURANGEL",
            "AngelsCreed", "ArsenicCoo", "Blastoid", "FreeDWG", "BabyBrownBoy", "BackBett",
            // Aggiungi tutti gli altri nomi qui
            // Nomi italiani aggiunti
            "P0veroDentro", "TuboNudoAHAH", "scartoumano", "FraTex10", "Sniubbo",
            "StupidaBibbi", "King-C-Cyclette", "Noob", "Stabby Mcstabface", "PewPewBB"
            // Continua ad aggiungere i nomi...
        };

        while (fakePlayersAdded < 5) // Se vuoi aggiungere 5 giocatori finti
        {
            yield return new WaitForSeconds(Random.Range(1, 10)); // Aspetta un periodo casuale tra secondi
            int randIndex = UnityEngine.Random.Range(0, creativeNicknames.Count); // Scegli un indice a caso
            string fakePlayerName = creativeNicknames[randIndex]; // Seleziona un nome casuale dalla lista

            // Assicurati che il nome scelto non sia già stato usato
            while (fakePlayersNames.Contains(fakePlayerName))
            {
                randIndex = UnityEngine.Random.Range(0, creativeNicknames.Count);
                fakePlayerName = creativeNicknames[randIndex];
            }

            GlobalGameManager.Instance.AddFakePlayerName(fakePlayerName); // Aggiungi il nome scelto alla lista dei nomi usati
            fakePlayersNames.Add(fakePlayerName); // Aggiungi il nome alla lista dei giocatori finti

            Hashtable props = new Hashtable { { "fakePlayers", fakePlayersNames.ToArray() } };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props); // Aggiorna le proprietà della stanza con i nuovi giocatori finti

            photonView.RPC("AddFakePlayerRPC", RpcTarget.All, fakePlayerName, fakePlayersAdded + 1); // Invoca un RPC per aggiungere il giocatore finto a tutti i client
            fakePlayersAdded++;
        }
    }
}





[PunRPC]
private void AddFakePlayerRPC(string fakePlayerName, int fakePlayerIndex)
{
    int fakeActorNumber = -100 - playerListEntries.Count;

    GameObject entry = Instantiate(PlayerListEntryPrefab);
    entry.transform.SetParent(InsideRoomPanel.transform, false);
    entry.transform.localPosition = Vector3.zero;
    entry.transform.localScale = Vector3.one;

    entry.GetComponent<PlayerListEntry>().Initialize(fakeActorNumber, fakePlayerName);
    entry.GetComponent<PlayerListEntry>().SetPlayerReady(true);

    playerListEntries.Add(fakeActorNumber, entry);
        // Aggiorna il conteggio dei giocatori finti

    UpdateGameStartCondition();
}

private void AddFakePlayerToList(string fakePlayerName)
{
    int fakeActorNumber = -100 - playerListEntries.Count;

    GameObject entry = Instantiate(PlayerListEntryPrefab);
    entry.transform.SetParent(InsideRoomPanel.transform, false);
    entry.transform.localPosition = Vector3.zero;
    entry.transform.localScale = Vector3.one;
    entry.GetComponent<PlayerListEntry>().Initialize(fakeActorNumber, fakePlayerName);
    entry.GetComponent<PlayerListEntry>().SetPlayerReady(true);

    playerListEntries.Add(fakeActorNumber, entry);


}
*/




        public override void OnLeftRoom()
        {
            SetActivePanel(SelectionPanel.name);

            foreach (GameObject entry in playerListEntries.Values)
            {
                Destroy(entry.gameObject);
            }
UpdateCurrencyUI();
            playerListEntries.Clear();
            playerListEntries = null;
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InsideRoomPanel.transform, false);
            entry.transform.localPosition = Vector3.zero; // Imposta la posizione locale a zero per centrarlo nel genitore
            entry.transform.localScale = Vector3.one; 
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);
     if (isRandomRoom)
    {
        // Nascondi il pulsante "Start Game" nelle stanze casuali
        StartGameButton.gameObject.SetActive(false);
        
        // Avvia il timer solo se ci sono almeno due giocatori
        UpdateGameStartCondition();
    }
    else
    {
        // Mostra il pulsante "Start Game" nelle stanze private
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);
     if (isRandomRoom)
    {
        // Nascondi il pulsante "Start Game" nelle stanze casuali
        StartGameButton.gameObject.SetActive(false);
        
 
    }
    else
    {
        // Mostra il pulsante "Start Game" nelle stanze private
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
            UpdateCurrencyUI();
        }
private void UpdateGameStartCondition()
{
    // Conta tutti i giocatori nella stanza, inclusi i finti
    int totalPlayerCount = playerListEntries.Count;
if(isDuelRoom==true)
{
    if (totalPlayerCount == 2)
    {
        // Avvia la partita immediatamente senza aspettare ulteriori giocatori o usare un timer
        StartGame();
    }
}
    // avviare la partita a 6
    if (totalPlayerCount == 6)
    {
        // Avvia la partita immediatamente senza aspettare ulteriori giocatori o usare un timer
        StartGame();
    }
}





        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
    if (isRandomRoom)
    {
        // Nascondi il pulsante "Start Game" nelle stanze casuali
        StartGameButton.gameObject.SetActive(false);
        
        // Avvia il timer solo se ci sono almeno due giocatori
        UpdateGameStartCondition();
    }
    else
    {
        // Mostra il pulsante "Start Game" nelle stanze private
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
                }
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            GameObject entry;
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                if (changedProps.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }
            }

    if (isRandomRoom)
    {
        // Nascondi il pulsante "Start Game" nelle stanze casuali
        StartGameButton.gameObject.SetActive(false);
        
        // Avvia il timer solo se ci sono almeno due giocatori
        UpdateGameStartCondition();
    }
    else
    {
        // Mostra il pulsante "Start Game" nelle stanze private
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }
            }

        #endregion

        private bool CheckPlayersReady()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        private void ClearRoomListView()
        {
            foreach (GameObject entry in roomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            roomListEntries.Clear();
        }

        public void LocalPlayerPropertiesUpdated()
        {
    if (isRandomRoom)
    {
        // Nascondi il pulsante "Start Game" nelle stanze casuali
        StartGameButton.gameObject.SetActive(false);
        
        // Avvia il timer solo se ci sono almeno due giocatori
        UpdateGameStartCondition();
    }
    else
    {
        // Mostra il pulsante "Start Game" nelle stanze private
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }        }



        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
            ConnectingPanel.SetActive(activePanel.Equals(ConnectingPanel.name));
        }

      private void UpdateCachedRoomList(List<RoomInfo> roomList)
{
    foreach (RoomInfo info in roomList)
    {
        // Remove room from cached room list if it got closed, became invisible or was marked as removed
        if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
        {
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList.Remove(info.Name);
            }

            continue;
        }

        // Check if the room has the IsDuelRoom property
        if (info.CustomProperties.ContainsKey("IsDuelRoom"))
        {
            continue;
        }

        // Update cached room info
        if (cachedRoomList.ContainsKey(info.Name))
        {
            cachedRoomList[info.Name] = info;
        }
        // Add new room info to cache
        else
        {
            cachedRoomList.Add(info.Name, info);
        }
    }
}

private void UpdateRoomListView()
{
    foreach (RoomInfo info in cachedRoomList.Values)
    {
        GameObject entry = Instantiate(RoomListEntryPrefab);
        entry.transform.SetParent(RoomListContent.transform);
        entry.transform.localPosition = Vector3.zero; // Imposta la posizione a zero rispetto al parent
        entry.transform.localRotation = Quaternion.identity; // Imposta la rotazione a zero
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

        roomListEntries.Add(info.Name, entry);
    }
}


    }
}