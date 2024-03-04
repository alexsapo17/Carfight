using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class HomeNavigation : MonoBehaviour
{
    public GameObject firstPanel; // Riferimento al FirstPanel
    public GameObject LeaderboardPanel;
    public GameObject levelsPanel; // Riferimento al LevelsPanel
    public GameObject missionsPanel; // Riferimento al MissionsPanel
    public GameObject gameControlsUI; // UI per il controllo del gioco
    public LevelManager levelManager; // Riferimento a LevelManager
        public GameObject TutorialSingleplayerPanel;
    public GameObject TutorialSingleplayer2Panel;
    public GameObject gameOverPanel;

public GameObject finishPanel; 
    void Start()
    {
        // Assicurati che solo il FirstPanel sia visibile all'avvio
        firstPanel.SetActive(true);
        LeaderboardPanel.SetActive(false);
        levelsPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
                 if (PlayerPrefs.GetInt("ShowTutorialSingleplayerPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        TutorialSingleplayerPanel.SetActive(true);
       

    }
    }

    public void OpenLevelsPanel()
    {
        // Mostra LevelsPanel e nascondi gli altri
        levelsPanel.SetActive(true);
        firstPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
        LeaderboardPanel.SetActive(false);
         if (PlayerPrefs.GetInt("ShowTutorialSingleplayerPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        TutorialSingleplayer2Panel.SetActive(true);
               TutorialSingleplayerPanel.SetActive(false);


    }
    }

    // Chiamato quando un pulsante del livello è premuto
public void OnLevelSelect(int levelIndex)
{
    levelsPanel.SetActive(false);
    gameControlsUI.SetActive(true); // Attiva l'UI di controllo della macchina
    levelManager.LoadLevel(levelIndex); // Carica il livello selezionato
    finishPanel.SetActive(false);
    LeaderboardPanel.SetActive(false);

}
    public void OpenMissionsPanel()
    {
        // Mostra MissionsPanel e nascondi gli altri
        missionsPanel.SetActive(true);
        firstPanel.SetActive(false);
        levelsPanel.SetActive(false);
        finishPanel.SetActive(false);
        LeaderboardPanel.SetActive(false);

    }

    public void ReturnToFirstPanel()
    {
        // Mostra FirstPanel e nascondi gli altri
        firstPanel.SetActive(true);
        levelsPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
     LeaderboardPanel.SetActive(false);
             gameOverPanel.SetActive(false);


    }
        public void OpenLeaderBoardPanel()
    {
        // Mostra FirstPanel e nascondi gli altri
      
        firstPanel.SetActive(false);
        levelsPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
     LeaderboardPanel.SetActive(true);

    }

}
