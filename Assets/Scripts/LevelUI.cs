using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelUI : MonoBehaviour
{
    public int LevelId { get; private set; }
    private LevelProgressManager progressManager;
    public Text bestTimeText;
    public Text starsText;
      public Text levelText;
      public Button levelButton;
      public GameObject levelLockedPanel;

   public void Initialize(int levelId, LevelProgressManager manager)
{
    LevelId = levelId;
    progressManager = manager;

    if (levelText != null)
    {
        levelText.text = "Livello " + levelId; // Imposta il testo del livello
    }

    UpdateUI(0, 0); // Inizializza l'UI con valori di default
}
    public void SetButtonListener(UnityEngine.Events.UnityAction action)
    {
        levelButton.onClick.RemoveAllListeners();
        levelButton.onClick.AddListener(action);
    }

   public void ShowLockedLevelPanel()
{
    levelLockedPanel.SetActive(true);
    StartCoroutine(DisablePanelAfterDelay(levelLockedPanel, 3f));
}

private IEnumerator DisablePanelAfterDelay(GameObject panel, float delay)
{
    yield return new WaitForSeconds(delay);
    panel.SetActive(false);
}
    public void UpdateUI(float bestTime, int stars)
    {

        bestTimeText.text = "Miglior Tempo: " + bestTime.ToString("F2");
        starsText.text = "Stelle: " + stars;
    }

    // Aggiungi qui ulteriori metodi se necessario
}
