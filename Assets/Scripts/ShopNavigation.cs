using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine.UI;

public class ShopNavigation : MonoBehaviour
{
    public GameObject shopPanel; // Pannello principale dello shop
    public GameObject realShopPanel;
    public GameObject carPanel;   // Pannello per le macchine
    public GameObject skillPanel; // Pannello per le abilità
    public GameObject customPanel; // Pannello personalizzazione

     public GameObject raceCarPanel;
     public GameObject prometheusPanel;
     public GameObject monstertruckPanel;
     public GameObject MicraRiccioPanel;
     public GameObject AmbulancePanel;
     public GameObject SportRacingCarPanel;
     public GameObject PoliceMonsterTruckPanel;
     public GameObject CicePanel;
     public GameObject BusPanel;
     public GameObject RubyPanel;
     public GameObject JeepPanel;
     public GameObject tutorial2Panel;
     public GameObject tutorial3Panel;
     public GameObject tutorialCustomPanel;
     public GameObject tutorialCustom2Panel;
     public GameObject tutorialCustom3Panel;
     public GameObject tutorialSingleplayerPanel;
     public GameObject FiretruckPanel;
     public GameObject sportCarPanel;
    public GameObject[] carButtons;
public Text coinsText;
public Text gemsText;
public Animator transitionAnimator; 
    void Start()
    {
                // Controlla se dobbiamo mostrare il pannello speciale
    if (PlayerPrefs.GetInt("ShowTutorial2Panel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorial2Panel.SetActive(true);


    }

        UpdateCoinsUI();
        UpdateGemsUI();
    }

   private void UpdateCoinsUI()
    {
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null && coinsText != null)
        {
            currencyManager.coinsText = coinsText;
            currencyManager.UpdateCoinsUI();
        }
    }
       private void UpdateGemsUI()
    {
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null && gemsText != null)
        {
            currencyManager.gemsText = gemsText;
            currencyManager.UpdateGemsUI();
        }
    }
    // Metodo per mostrare il pannello principale dello shop
    public void ShowShopPanel()
    {
                        if (PlayerPrefs.GetInt("ShowTutorialCustomPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorialCustomPanel.SetActive(true);

    }
         if (PlayerPrefs.GetInt("ShowTutorialSingleplayerPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorialSingleplayerPanel.SetActive(true);

    }
        shopPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
                realShopPanel.SetActive(false);

    }
        public void ShowRealShopPanel()
    {
        shopPanel.SetActive(false);
        realShopPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);

    }

    // Metodo per mostrare il pannello dei bauli
    public void ShowCarPanel()
    {
            if (PlayerPrefs.GetInt("ShowTutorial2Panel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorial3Panel.SetActive(true);
        tutorial2Panel.SetActive(false);


    }
                        realShopPanel.SetActive(false);

        shopPanel.SetActive(false);
        carPanel.SetActive(true);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);

    }

    // Metodo per mostrare il pannello delle abilità
    public void ShowSkillPanel()
    {
                        realShopPanel.SetActive(false);

        shopPanel.SetActive(false);
        carPanel.SetActive(false);
        skillPanel.SetActive(true);
        customPanel.SetActive(false);

    }
        public void ShowCustomPanel()
    {
                        if (PlayerPrefs.GetInt("ShowTutorialCustomPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorialCustom2Panel.SetActive(true);
       tutorialCustomPanel.SetActive(false);
              

    }
    tutorialCustom3Panel.SetActive(false);
        raceCarPanel.SetActive(false);
        prometheusPanel.SetActive(false);
        monstertruckPanel.SetActive(false);
        sportCarPanel.SetActive(false);
        AmbulancePanel.SetActive(false);
        SportRacingCarPanel.SetActive(false);
        PoliceMonsterTruckPanel.SetActive(false);
        CicePanel.SetActive(false);
        BusPanel.SetActive(false);
        RubyPanel.SetActive(false);
        JeepPanel.SetActive(false);
                        realShopPanel.SetActive(false);

        FiretruckPanel.SetActive(false);
        MicraRiccioPanel.SetActive(false);
        shopPanel.SetActive(false);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(true);

           foreach (var button in carButtons)
    {
        button.SetActive(false);
    }

    LoadOwnedCars();
    UpdateCarButtons();
}
public void ShowraceCarPanel()
    {
        raceCarPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
    public void ShowprometheusPanel()
    {
        prometheusPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowmonstertruckPanel()
    {
        monstertruckPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowMicraRiccioPanel()
    {
        MicraRiccioPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowsportCarPanel()
    {
        sportCarPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowAmbulancePanel()
    {
        AmbulancePanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowSportRacingCarPanel()
    {
        SportRacingCarPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowPoliceMonsterTruckPanel()
    {
        PoliceMonsterTruckPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowCicePanel()
    {
        CicePanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
       public void ShowBusPanel()
    {
        BusPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
           public void ShowRubyPanel()
    {
        RubyPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
           public void ShowJeepPanel()
    {
        JeepPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }
           public void ShowFiretruckPanel()
    {
        FiretruckPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }

public void OnSinglePlayerButtonClicked()
{
    transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadSinglePlayerScene", 1f);
    }
    // Metodo separato per caricare la scena, chiamato dopo il ritardo
void LoadSinglePlayerScene()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("SinglePlayerScene");
}
public void OnOnlineButtonClicked()
{
   transitionAnimator.SetTrigger("Start"); // Avvia l'animazione di transizione
    Invoke("LoadOnlineScene", 1f);
    }
    // Metodo separato per caricare la scena, chiamato dopo il ritardo
void LoadOnlineScene()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("DemoAsteroids-LobbyScene");
}

   private void LoadOwnedCars()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DatabaseReference userCarsRef = FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId).Child("ownedCars");

        userCarsRef.GetValueAsync().ContinueWithOnMainThread(task => 
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading owned cars: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot car in snapshot.Children)
                {
                    string carName = car.Key;
                    bool isOwned = (bool)car.Value; 
                    if (isOwned) 
                    {
                        EnableCarButton(carName);
                    }
                }
            }
        });
    }

private void EnableCarButton(string carName)
{
    foreach (var button in carButtons)
    {
        if (button.name.ToLower() == carName.ToLower())
        {
            button.SetActive(true);
        }
    }
}

public void SelectCar(string carName)
{
    // Salva il nome della macchina selezionata nelle PlayerPrefs
    PlayerPrefs.SetString("SelectedCar", carName);

    // Imposta l'abilità "Invisibility" come l'abilità selezionata nelle PlayerPrefs
    PlayerPrefs.SetString("SelectedAbility", "Invisibility");

    // Salva le modifiche nelle PlayerPrefs
    PlayerPrefs.Save();

    // Aggiorna i pulsanti delle macchine per riflettere la selezione corrente
    UpdateCarButtons();

     if (PlayerPrefs.GetInt("ShowTutorialCustomPanel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorialCustom3Panel.SetActive(true);
       tutorialCustom2Panel.SetActive(false);
       PlayerPrefs.SetInt("ShowTutorialCustomPanel", 0);
PlayerPrefs.SetInt("ShowTutorialSingleplayerPanel", 1);
    }

    // Trova l'istanza di AbilitySelection e chiama UpdateButtonColors
    AbilitySelection abilitySelection = FindObjectOfType<AbilitySelection>();
    if (abilitySelection != null)
    {
        abilitySelection.StartDelayedUpdateButtonColors();
    }
}

private void UpdateCarButtons()
{
    string selectedCar = PlayerPrefs.GetString("SelectedCar", "");

    foreach (var carButton in carButtons) // Assumi che carButtons sia una lista di tutti i pulsanti
    {
        string carName = carButton.name; // O come ottieni il nome dell'auto dal pulsante
        Transform textTransform = carButton.transform.Find("SelectText"); // Assumi che il testo sia un figlio chiamato "SelectText"
        
        if (textTransform != null)
        {
            Text carText = textTransform.GetComponent<Text>();
            if (carName == selectedCar)
            {
                 string language = PlayerPrefs.GetString("Language", "en"); // Ottieni la lingua corrente
 if (language == "it")
        {
                carText.text = "SELEZIONATA";
        }
         if (language == "en")
        {
                carText.text = "SELECTED";
        }
         if (language == "es")
        {
                carText.text = "SELECCIONADA";
        }
         if (language == "fr")
        {
                carText.text = "SÉLECTIONNÉE";
        }                carText.color = Color.green; // Cambia con il colore desiderato
                // Aggiungi qui ulteriori modifiche visive se necessario
            }
            else
            {
                 string language = PlayerPrefs.GetString("Language", "en"); // Ottieni la lingua corrente
 if (language == "it")
        {
                carText.text = "SELEZIONA";
        }
         if (language == "en")
        {
                carText.text = "SELECT";
        }
         if (language == "es")
        {
                carText.text = "SELECCIONA";
        }
         if (language == "fr")
        {
                carText.text = "SÉLECTIONNE";
        }
                carText.color = Color.white;
                // Resetta altre modifiche visive se necessario
            }
        }
    }
}
    // Metodo per tornare alla lobby
    public void ReturnToLobby()
    {
        SceneManager.LoadScene("DemoAsteroids-LobbyScene"); 
    }
}
