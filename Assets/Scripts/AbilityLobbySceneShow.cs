using UnityEngine;
using UnityEngine.UI;

public class AbilityLobbySceneShow : MonoBehaviour
{
    public GameObject invisibilityButton;
    public GameObject punchButton;
    public GameObject shockwaveButton;
    public GameObject jumpButton;
    public GameObject flashbangButton;

    private void Start()
    {
        // Controlla se è stata selezionata un'abilità dalle PlayerPrefs
        string selectedAbility = PlayerPrefs.GetString("SelectedAbility", "");

        // Attiva il pulsante corrispondente all'abilità selezionata e disattiva gli altri
        switch (selectedAbility)
        {
        case "Invisibility":
                invisibilityButton.SetActive(true);
                punchButton.SetActive(false);
                shockwaveButton.SetActive(false);
                jumpButton.SetActive(false);
                flashbangButton.SetActive(false);
                break;
            case "Punch":
                invisibilityButton.SetActive(false);
                punchButton.SetActive(true);
                shockwaveButton.SetActive(false);
                jumpButton.SetActive(false);
                flashbangButton.SetActive(false);
                break;
            case "Shockwave":
                invisibilityButton.SetActive(false);
                punchButton.SetActive(false);
                shockwaveButton.SetActive(true);
                jumpButton.SetActive(false);
                flashbangButton.SetActive(false);
                break;
            case "Jump":
                invisibilityButton.SetActive(false);
                punchButton.SetActive(false);
                shockwaveButton.SetActive(false);
                jumpButton.SetActive(true);
                flashbangButton.SetActive(false);
                break;
            case "Flashbang":
                invisibilityButton.SetActive(false);
                punchButton.SetActive(false);
                shockwaveButton.SetActive(false);
                jumpButton.SetActive(false);
                flashbangButton.SetActive(true);
                break;
            default:
                // Nessuna abilità selezionata, disattiva tutti i pulsanti
                invisibilityButton.SetActive(false);
                punchButton.SetActive(false);
                shockwaveButton.SetActive(false);
                jumpButton.SetActive(false);
                flashbangButton.SetActive(false);
                break;
        }
    }
}
