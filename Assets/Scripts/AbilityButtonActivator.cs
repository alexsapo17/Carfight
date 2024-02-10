using UnityEngine;

public class AbilityButtonActivator : MonoBehaviour
{
    public GameObject invisibilityButton;
    public GameObject jumpButton;
    public GameObject shockwaveButton;
    public GameObject flashbangButton;
    public GameObject duplicateButton;
    private void Start()
    {
        string selectedAbility = PlayerPrefs.GetString("SelectedAbility");

        switch (selectedAbility)
        {
            case "Invisibility":
                invisibilityButton.SetActive(true);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(false);
                flashbangButton.SetActive(false);
                                duplicateButton.SetActive(false);

                break;
            case "Jump":
                invisibilityButton.SetActive(false);
                jumpButton.SetActive(true);
                shockwaveButton.SetActive(false);
                flashbangButton.SetActive(false);
                                duplicateButton.SetActive(false);

                break;
            case "ShockWave":
                invisibilityButton.SetActive(false);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(true);
                flashbangButton.SetActive(false);
                                duplicateButton.SetActive(false);

                break;
                            case "Flashbang":
                invisibilityButton.SetActive(false);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(false);
                flashbangButton.SetActive(true);
                                duplicateButton.SetActive(false);

                break;
                            case "Duplicate":
                invisibilityButton.SetActive(false);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(false);
                flashbangButton.SetActive(false);
                duplicateButton.SetActive(true);
                break;
            default:
                invisibilityButton.SetActive(true);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(false);
                flashbangButton.SetActive(false);
                duplicateButton.SetActive(false);
                break;
        }
    }
}
