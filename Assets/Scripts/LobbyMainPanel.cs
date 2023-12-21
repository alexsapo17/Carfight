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



        private Dictionary<string, RoomInfo> cachedRoomList;
        private Dictionary<string, GameObject> roomListEntries;
        private Dictionary<int, GameObject> playerListEntries;
        private FirebaseAuth auth;
        private DatabaseReference databaseReference;

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

    // Trova il CurrencyManager e aggiorna l'UI delle monete
    UpdateCurrencyUI();
}
public void Start()
{
    UpdateCurrencyUI();
}

private void UpdateCurrencyUI()
{
    CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
    if (currencyManager != null)
    {
        // Trova il componente TextMeshProUGUI
        TextMeshProUGUI coinTextComponent = GameObject.Find("CoinsText").GetComponent<TextMeshProUGUI>();

        // Assicurati che il componente sia stato trovato
        if(coinTextComponent != null) 
        {
            currencyManager.coinsText = coinTextComponent;
            currencyManager.UpdateCoinsUI();
        }
        else 
        {
            Debug.LogError("Componente TextMeshProUGUI per le monete non trovato.");
        }
    }
    else
    {
    Debug.LogError("CurrencyManager non trovato nella scena.");
    }
}

        #endregion

        #region PUN CALLBACKS

        public override void OnConnectedToMaster()
        {
            this.SetActivePanel(SelectionPanel.name);
            UpdateCurrencyUI();
        }

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

            RoomOptions options = new RoomOptions { MaxPlayers = 8 };

            PhotonNetwork.CreateRoom(roomName, options, null);
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
                    // È la prima volta, mostra l'input field
                    SetActivePanel(LoginPanel.name);
                    PlayerNameInput.gameObject.SetActive(true);
                    PlayerNameInput.text = "";
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
    UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
}
public void OnLogoutButtonClicked()
{
    if (auth.CurrentUser != null)
    {
        auth.SignOut();
        Debug.Log("User signed out successfully.");

        // Opzionalmente, aggiorna l'interfaccia utente o reindirizza l'utente a un altro pannello, ad esempio il pannello di login.
        SetActivePanel(LoginPanel.name);
    }
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
                    Debug.Log("Nickname saved successfully.");
                    PlayerNameInput.gameObject.SetActive(false);




                }
            });
        }
        public void OnLoginButtonClicked()
        {
            string playerName = PlayerNameInput.text;

            if (!string.IsNullOrEmpty(playerName))
            {
                // Controllo se l'utente è autenticato con Firebase
                if (auth.CurrentUser != null)
                {
                    // L'utente è già autenticato, salva il nickname e continua
                    SaveNickname(playerName);
                }
                else
                {
                    // L'utente non è autenticato, esegui il processo di registrazione o login
                    // Questo potrebbe essere un nuovo metodo che gestisce la registrazione o il login
                    RegisterOrLoginUser(playerName);
                }
            }
            else
            {
                Debug.LogError("Player Name is invalid.");
            }
            ConnectToPhoton();
            
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
                        Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
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
                entry.transform.SetParent(InsideRoomPanel.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(AsteroidsGame.PLAYER_READY, out isPlayerReady))
                {
                    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                }

                playerListEntries.Add(p.ActorNumber, entry);
            }

            StartGameButton.gameObject.SetActive(CheckPlayersReady());

            Hashtable props = new Hashtable
            {
                {AsteroidsGame.PLAYER_LOADED_LEVEL, false}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

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
            entry.transform.SetParent(InsideRoomPanel.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
            playerListEntries.Remove(otherPlayer.ActorNumber);

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
            UpdateCurrencyUI();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartGameButton.gameObject.SetActive(CheckPlayersReady());
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

            StartGameButton.gameObject.SetActive(CheckPlayersReady());
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
            string roomName = RoomNameInputField.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

            byte maxPlayers;
            byte.TryParse(MaxPlayersInputField.text, out maxPlayers);
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers, PlayerTtl = 10000 };

            PhotonNetwork.CreateRoom(roomName, options, null);
        }

        public void OnJoinRandomRoomButtonClicked()
        {
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

            int entryCost = 100; // Costo per unirsi alla stanza
            if (currencyManager.HasEnoughCoins(entryCost))
            {
                SetActivePanel(JoinRandomRoomPanel.name);
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                Debug.LogError("Non hai abbastanza monete per unirti alla stanza.");
            }
        }

        public void OnStartGameButtonClicked()
        {
            CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
            if (currencyManager == null)
            {
                Debug.LogError("CurrencyManager non trovato.");
                return;
            }

            int entryCost = 100; // Costo per avviare il gioco

            if (currencyManager.TrySpendCoins(entryCost))
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                PhotonNetwork.CurrentRoom.IsVisible = false;
                PhotonNetwork.LoadLevel("GameScene");
            }
            else
            {
                Debug.LogError("Non hai abbastanza monete per avviare il gioco.");
                // Gestisci il caso in cui il giocatore non ha abbastanza monete
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
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }

        private void SetActivePanel(string activePanel)
        {
            LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
            SelectionPanel.SetActive(activePanel.Equals(SelectionPanel.name));
            CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
            JoinRandomRoomPanel.SetActive(activePanel.Equals(JoinRandomRoomPanel.name));
            RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));    // UI should call OnRoomListButtonClicked() to activate this
            InsideRoomPanel.SetActive(activePanel.Equals(InsideRoomPanel.name));
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
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, (byte)info.MaxPlayers);

                roomListEntries.Add(info.Name, entry);
            }
        }
    }
}