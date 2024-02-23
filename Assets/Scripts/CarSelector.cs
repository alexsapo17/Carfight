using Firebase.Database;
using UnityEngine;
using System.Linq; // Assicurati di includere questo per i metodi LINQ
using Firebase.Auth;
using Firebase.Extensions;
[System.Serializable]
public class CarPrefabMapping
{
    public string carName;
    public GameObject carPrefab;
}
public class CarSelector : MonoBehaviour
{
    public CarPrefabMapping[] carMappings;
    private GameObject currentCarInstance;
private float angularVelocity; // Velocità di rotazione
private float rotationDecay = 0.95f;
    void Start()
    {
        string selectedCarName = PlayerPrefs.GetString("SelectedCar", "");

        if (string.IsNullOrEmpty(selectedCarName))
        {
            FetchRandomOwnedCar();
        }
        else
        {
            SelectCar(selectedCarName);
        }
    }

    void FetchRandomOwnedCar()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DatabaseReference userCarsRef = FirebaseDatabase.DefaultInstance.GetReference("users").Child(userId).Child("ownedCars");

        userCarsRef.GetValueAsync().ContinueWithOnMainThread(task => 
        {
            if (task.IsFaulted)
            {
                // Gestisci l'errore...
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                var ownedCars = snapshot.Children.ToList();
                int count = ownedCars.Count();
                
                if (count > 0)
                {
                    int randomIndex = Random.Range(0, count);
                    string selectedCarName = ownedCars.ElementAt(randomIndex).Key;
                    
                    PlayerPrefs.SetString("SelectedCar", selectedCarName);
                    PlayerPrefs.Save();
                    SelectCar(selectedCarName);
                }
            }
        });
    }

    void SelectCar(string carName)
    {
        foreach (CarPrefabMapping mapping in carMappings)
        {
            if (mapping.carPrefab != null)
            {
                bool isSelected = mapping.carName == carName;
                mapping.carPrefab.SetActive(isSelected);
                if (isSelected)
                {
                    currentCarInstance = mapping.carPrefab;
                }
            }
        }
    }

void Update()
{
    if (currentCarInstance != null)
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Considera di utilizzare Screen.dpi per adattare la velocità di rotazione alla densità di pixel del dispositivo
            float dpiFactor = (Screen.dpi > 0) ? Screen.dpi / 160f : 1f; // 160 è il dpi base per un dispositivo Android
            float rotationSensitivity = 0.3f; // Regola questo valore per aumentare o diminuire la sensibilità

            if (touch.phase == TouchPhase.Moved)
            {
                // Usa dpiFactor per normalizzare la velocità di rotazione
                float touchDeltaX = touch.deltaPosition.x / dpiFactor * rotationSensitivity;
                angularVelocity = -touchDeltaX;
            }
        }
        else if (angularVelocity != 0)
        {
            // Rallenta gradualmente la velocità di rotazione
            angularVelocity *= rotationDecay;
            // Se la velocità di rotazione è molto piccola, fermati per evitare un'inerzia infinita
            if (Mathf.Abs(angularVelocity) < 0.01f)
            {
                angularVelocity = 0;
            }
        }

        // Applica la rotazione
        if (angularVelocity != 0)
        {
            currentCarInstance.transform.Rotate(Vector3.up, angularVelocity, Space.World);
        }
    }
}
}