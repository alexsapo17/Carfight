using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Linq; // Necessario per usare metodi come ToList()
using System.Threading.Tasks;

public class CarSelectionPanel : MonoBehaviour
{
    public GameObject carButtonPrefab; // Assegna il prefab del pulsante auto nell'Inspector
    public Transform contentPanel; // Assegna il GameObject Content dello ScrollView nell'Inspector
    public GameObject noCarSelectedPanel; // Pannello No Car Selected

    private DatabaseReference databaseReference;
    private List<GameObject> carButtons = new List<GameObject>(); // Lista per tenere traccia dei pulsanti generati

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        GenerateCarButtons();
    }


void GenerateCarButtons()
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    databaseReference.Child("users").Child(userId).Child("ownedCars").GetValueAsync().ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
        {
            Debug.LogError("Errore nel ricevere le ownedcars.");
            // Gestisci l'errore
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;
            foreach (DataSnapshot carSnapshot in snapshot.Children)
            {
                // Utilizza la chiave del nodo come nome dell'auto
                string carId = carSnapshot.Key;
                bool ownsCar = (bool)carSnapshot.Value;

                if (ownsCar) // Se l'utente possiede l'auto, crea un pulsante
                {

                    
                    GameObject buttonObj = Instantiate(carButtonPrefab, contentPanel);
                    buttonObj.name = carId; // Imposta il nome del GameObject per corrispondere all'ID dell'auto per riferimento facile
                    carButtons.Add(buttonObj); // Aggiungi il pulsante alla lista

             // Imposta il testo del nome dell'auto
            Text carNameText = buttonObj.transform.Find("CarNameText").GetComponent<Text>();
            carNameText.text = carId.ToUpper(); // Utilizza l'ID dell'auto come testo, convertito in maiuscolo
            carNameText.color = Color.white;

                    // Aggiungi un listener al pulsante
                    buttonObj.GetComponent<Button>().onClick.AddListener(() => SelectCar(carId));
                                // Carica l'immagine dal nome dell'auto
          // Carica l'immagine come Sprite da Resources
            Sprite carSprite = Resources.Load<Sprite>("Images/" + carId);
            if (carSprite != null)
            {
                Image buttonImage = buttonObj.GetComponentInChildren<Image>();
                buttonImage.sprite = carSprite;
            }
            else
            {
                Debug.LogError("Immagine non trovata per: " + carId);
            }

                }
            }

            UpdateCarButtons(); // Aggiorna i pulsanti subito dopo la creazione
        }
    });
}

// ... (il resto dello script rimane invariato)


    void SelectCar(string carId)
    {
        PlayerPrefs.SetString("SelectedCar", carId);
        PlayerPrefs.Save();
        UpdateCarButtons();
    }

 private void UpdateCarButtons()
{
    string selectedCar = PlayerPrefs.GetString("SelectedCar", "");

    foreach (var carButton in carButtons)
    {
        bool isSelected = carButton.name == selectedCar;
        // Cerca il Text per lo stato di selezione all'interno del pulsante
        Text statusText = carButton.transform.Find("StatusText").GetComponent<Text>();

        if (statusText != null)
        {
            statusText.text = isSelected ? "SELEZIONATA" : "SELEZIONA";
            statusText.color = isSelected ? Color.green : Color.white;
        }
    }
}

}
