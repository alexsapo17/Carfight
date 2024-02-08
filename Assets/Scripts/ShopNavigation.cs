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
     public GameObject FiretruckPanel;
     public GameObject sportCarPanel;
    public GameObject[] carButtons;
public Text coinsText;
public Text gemsText;
public Animator transitionAnimator; 
    void Start()
    {
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
        shopPanel.SetActive(true);
        carPanel.SetActive(false);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }

    // Metodo per mostrare il pannello delle macchine
    public void ShowCarPanel()
    {
        shopPanel.SetActive(false);
        carPanel.SetActive(true);
        skillPanel.SetActive(false);
        customPanel.SetActive(false);
    }

    // Metodo per mostrare il pannello delle abilità
    public void ShowSkillPanel()
    {
        shopPanel.SetActive(false);
        carPanel.SetActive(false);
        skillPanel.SetActive(true);
        customPanel.SetActive(false);
    }
        public void ShowCustomPanel()
    {
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

        public void OnShopButtonClicked()
{
    UnityEngine.SceneManagement.SceneManager.LoadScene("ShopScene");
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
                carText.text = "SELEZIONATA";
                carText.color = Color.green; // Cambia con il colore desiderato
                // Aggiungi qui ulteriori modifiche visive se necessario
            }
            else
            {
                carText.text = "SELEZIONA";
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
