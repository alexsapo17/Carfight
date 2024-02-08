using UnityEngine;

public class AbilityButtonActivator : MonoBehaviour
{
    public GameObject invisibilityButton;
    public GameObject jumpButton;
    public GameObject shockwaveButton;

    private void Start()
    {
        string selectedAbility = PlayerPrefs.GetString("SelectedAbility");

        switch (selectedAbility)
        {
            case "Invisibility":
                invisibilityButton.SetActive(true);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(false);
                break;
            case "Jump":
                invisibilityButton.SetActive(false);
                jumpButton.SetActive(true);
                shockwaveButton.SetActive(false);
                break;
            case "ShockWave":
                invisibilityButton.SetActive(false);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(true);
                break;
            default:
                invisibilityButton.SetActive(true);
                jumpButton.SetActive(false);
                shockwaveButton.SetActive(false);
                break;
        }
    }
}
