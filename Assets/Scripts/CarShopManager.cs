using System;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;


public class CarShopManager : MonoBehaviour
{
    public GameObject carPrefab1;
    public GameObject carPrefab2;
    public GameObject carPrefab3;
    public GameObject carPrefab4;
    private DatabaseReference databaseReference;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
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
                            Debug.Log(carName + " acquistata con successo.");
                            CurrencyManager.Instance.TrySpendCoins(carCost);
                            CurrencyManager.Instance.UpdateCoinsUI();
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





        private void UpdateCoinsUI()
    {
        CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
        if (currencyManager != null)
        {
            currencyManager.UpdateCoinsUI();
        }
    }

}
