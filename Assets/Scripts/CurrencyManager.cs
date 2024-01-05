using System;
using UnityEngine;
using TMPro;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class CurrencyManager : MonoBehaviour
{
    private int playerCoins;
public Text coinsText;       
    private DatabaseReference databaseReference;
    public static CurrencyManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Distruggi l'oggetto se esiste già un'istanza
        }
        else
        {
            Instance = this; // Assegna questa istanza a Instance
            DontDestroyOnLoad(gameObject); // Impedisce la distruzione quando si cambia scena
        }
    }

    private void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        
       
    }

public void LoadCoins()
{
    FirebaseAuth auth = FirebaseAuth.DefaultInstance;
    FirebaseUser user = auth.CurrentUser;
    
            if (user != null && databaseReference != null)
        {
            string userId = user.UserId;
        databaseReference.Child("users").Child(userId).Child("coins").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error loading coins: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Value != null)
                {
                    playerCoins = int.Parse(snapshot.Value.ToString());
                    UpdateCoinsUI();
                }
                else
                {
                    // Se non ci sono monete salvate, assegna 1000 monete
                    playerCoins = 1000;
                    SaveCoins();
                    
                }
            }
        });
    }
 
}

    private void SaveCoins()
    {
        string userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        databaseReference.Child("users").Child(userId).Child("coins").SetValueAsync(playerCoins);
        UpdateCoinsUI();
    }

    public void ModifyCoins(int amount)
    {
        playerCoins += amount;
        SaveCoins();
        UpdateCoinsUI();
    }

public void RefundCoins(int amount, Action onComplete)
{
    playerCoins += amount;
    SaveCoins();
    onComplete?.Invoke(); // Aggiorna l'UI dopo il rimborso
}


    public void UpdateCoinsUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + playerCoins.ToString();
        }
    }
   public bool HasEnoughCoins(int amount)
    {
        return playerCoins >= amount;
    }

    public bool TrySpendCoins(int amount)
    {
        if (HasEnoughCoins(amount))
        {
            ModifyCoins(-amount);
            return true;
        }
        return false;
    }

}
