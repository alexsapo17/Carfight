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
          public Image star1; // Aggiungi queste
    public Image star2; // reference alle
    public Image star3; // immagini delle stelle

    public Sprite emptyStar; // Sprite per stella vuota
    public Sprite filledStar; // Sprite per stella piena

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

        // Aggiorna le stelle
        UpdateStars(stars);
    }

    private void UpdateStars(int stars)
    {
        star1.sprite = stars >= 1 ? filledStar : emptyStar;
        star2.sprite = stars >= 2 ? filledStar : emptyStar;
        star3.sprite = stars >= 3 ? filledStar : emptyStar;
    }

    // Aggiungi qui ulteriori metodi se necessario
}
