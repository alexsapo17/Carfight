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
        if (Input.GetMouseButton(0) && currentCarInstance != null)
        {
            float rotationSpeed = 500.0f;
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentCarInstance.transform.Rotate(Vector3.up, -mouseX);
        }
    }
}
