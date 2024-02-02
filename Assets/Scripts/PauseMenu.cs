using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PauseMenu : MonoBehaviourPunCallbacks
{
    public GameObject pauseMenuUI;
public Animator transitionAnimator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        pauseMenuUI.SetActive(!pauseMenuUI.activeSelf);
        // Opzionale: Pausa il gioco se è un gioco single player o se hai l'autorità per mettere il gioco in pausa.
    }

    public void ContinueGame()
    {
        TogglePauseMenu();
        // Opzionale: Riprendi il gioco qui.
    }

    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom(); // Segnala l'intenzione di lasciare la stanza.
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect(); // Disconnetti dal server Photon dopo aver lasciato la stanza.
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
            transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadOnlineScene", 1f);
    }
    void LoadOnlineScene()
{
        UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
}
}
