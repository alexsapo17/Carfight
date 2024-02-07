using UnityEngine;
using UnityEngine.UI; // Importa il namespace per UI

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public Button setup1Button; // Riferimento al pulsante per il setup 1
    public Button setup2Button; // Riferimento al pulsante per il setup 2
    private bool settingsPanelActive;

    void Start()
    {
        settingsPanel.SetActive(false);
        settingsPanelActive = false;
        UpdateButtonColors(); // Aggiorna i colori dei pulsanti all'avvio
    }

    void UpdateButtonColors()
    {
        // Predefinisci i colori per i pulsanti selezionati e non selezionati
        Color selectedColor = new Color(0.5f, 1.0f, 0.5f, 1); // Verde leggero per il pulsante selezionato
        Color unselectedColor = Color.white; // Bianco per il pulsante non selezionato

        // Ottieni l'attuale setup selezionato
        int selectedSetup = PlayerPrefs.GetInt("ControlSetup", 1);

        // Imposta i colori dei pulsanti in base alla selezione
        setup1Button.image.color = (selectedSetup == 1) ? selectedColor : unselectedColor;
        setup2Button.image.color = (selectedSetup == 2) ? selectedColor : unselectedColor;
    }

    public void ToggleSettingsPanel()
    {
        settingsPanelActive = !settingsPanelActive;
        settingsPanel.SetActive(settingsPanelActive);
        if (settingsPanelActive)
        {
            UpdateButtonColors(); // Aggiorna i colori ogni volta che il pannello viene aperto
        }
    }

    public void ChooseSetup1()
    {
        PlayerPrefs.SetInt("ControlSetup", 1);
        UpdateButtonColors(); // Aggiorna i colori subito dopo la selezione
    }

    public void ChooseSetup2()
    {
        PlayerPrefs.SetInt("ControlSetup", 2);
        UpdateButtonColors(); // Aggiorna i colori subito dopo la selezione
    }
}
