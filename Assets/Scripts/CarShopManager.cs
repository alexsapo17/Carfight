using System;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine.UI;


public class CarShopManager : MonoBehaviour
{
    public GameObject carPrefab1;
    public GameObject carPrefab2;
    public GameObject carPrefab3;
    public GameObject carPrefab4;
    
    public GameObject[] carButtons;
    private DatabaseReference databaseReference;
    private HashSet<string> ownedCars = new HashSet<string>();


    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadOwnedCars();
    }

public void BuyCar1()
    {
        int carCost = 100;
        TryBuyCar("prometheus", carCost);
    }

    public void BuyCar2()
    {
        int carCost = 120;
        TryBuyCar("sportCar", carCost);
    }

    public void BuyCar3()
    {
        int carCost = 130;
        TryBuyCar("raceCar", carCost);
    }
    public void BuyCar4()
{
    int carCost = 100; // Definisci il costo della Monstertruck
    TryBuyCar("monstertruck", carCost);
}


    private void TryBuyCar(string carName, int carCost)
    {
        if (!CurrencyManager.Instance.HasEnoughCoins(carCost))
        {
            Debug.Log("Non hai abbastanza monete.");
            return;
        }

        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DatabaseReference carRef = databaseReference.Child("users").Child(userId).Child("ownedCars").Child(carName);

        carRef.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Errore nel recupero dei dati.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value == null) // Se la macchina non è ancora posseduta
                {
                    carRef.SetValueAsync(true).ContinueWithOnMainThread(task => 
                    {
                        if(task.IsCompleted)
                        {
                            ownedCars.Add(carName);
                            Debug.Log(carName + " acquistata con successo.");
                            CurrencyManager.Instance.TrySpendCoins(carCost);
                            CurrencyManager.Instance.UpdateCoinsUI();
                            UpdateShopCarUI();
                        }
                    });
                }
                else
                {
                    Debug.Log("Macchina già posseduta.");
                }
            }
        });
    }

private bool CheckIfCarIsOwned(string carName)
{
    return ownedCars.Contains(carName);
}


public void UpdateShopCarUI()
{
    foreach (var carButton in carButtons) // Assumi che carButtons sia una lista di tutti i pulsanti delle auto nel negozio
    {
        string carName = carButton.name; // O come ottieni il nome dell'auto dal pulsante
        Transform textTransform = carButton.transform.Find("ownText"); // Assumi che il testo sia un figlio chiamato "ownText"

        if (textTransform != null)
        {
            Text carText = textTransform.GetComponent<Text>();
            if (CheckIfCarIsOwned(carName))
            {
                carText.text = "GIÀ POSSEDUTA";
                carText.color = Color.red; // Cambia con il colore desiderato

            }
            else
            {
                carText.text = "ACQUISTA";
                carText.color = Color.green;

            }
        }
    }
}
private void LoadOwnedCars()
{
    string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    DatabaseReference userCarsRef = databaseReference.Child("users").Child(userId).Child("ownedCars");

    userCarsRef.GetValueAsync().ContinueWithOnMainThread(task => {
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
                    ownedCars.Add(carName);
                    Debug.Log("Caricata auto posseduta: " + carName);
                }
            }
            UpdateShopCarUI(); // Sposta qui
        }
    });
}



        private void UpdateCoinsUI()
    {
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null)
        {
            currencyManager.UpdateCoinsUI();
        }
    }

}
