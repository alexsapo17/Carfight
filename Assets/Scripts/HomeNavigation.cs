using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class HomeNavigation : MonoBehaviour
{
    public GameObject firstPanel; // Riferimento al FirstPanel
    public GameObject levelsPanel; // Riferimento al LevelsPanel
    public GameObject missionsPanel; // Riferimento al MissionsPanel
    public GameObject gameControlsUI; // UI per il controllo del gioco
    public LevelManager levelManager; // Riferimento a LevelManager
public GameObject finishPanel; 
    void Start()
    {
        // Assicurati che solo il FirstPanel sia visibile all'avvio
        firstPanel.SetActive(true);
        levelsPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

    public void OpenLevelsPanel()
    {
        Time.timeScale = 0;
        // Mostra LevelsPanel e nascondi gli altri
        levelsPanel.SetActive(true);
        firstPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

    // Chiamato quando un pulsante del livello è premuto
public void OnLevelSelect(int levelIndex)
{
    levelsPanel.SetActive(false);
    gameControlsUI.SetActive(true); // Attiva l'UI di controllo della macchina
    levelManager.LoadLevel(levelIndex); // Carica il livello selezionato
    finishPanel.SetActive(false);
}
    public void OpenMissionsPanel()
    {
        // Mostra MissionsPanel e nascondi gli altri
        missionsPanel.SetActive(true);
        firstPanel.SetActive(false);
        levelsPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

    public void ReturnToFirstPanel()
    {
        // Mostra FirstPanel e nascondi gli altri
        Time.timeScale = 0;
        firstPanel.SetActive(true);
        levelsPanel.SetActive(false);
        missionsPanel.SetActive(false);
        finishPanel.SetActive(false);
    }

}
