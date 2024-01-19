using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Linq;


public class LootBoxManager : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private string userId;
public Animator lootBoxAnimator;
public Canvas lootBoxCanvas; 

public Transform spawnPoint; // Punto dove spawnare la macchina
public Vector3 spawnScale = Vector3.one; // Scala del prefab al momento dello spawn
public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Velocità di rotazione (gradi al secondo)
public float moveUpSpeed = 1f; // Velocità di movimento verso l'alto
public float destructionDelay = 2f; // Tempo dopo il quale il prefab verrà distrutto

[System.Serializable]
public class CarProbability
{
    public string carName;
    public GameObject carPrefab; 
    public float probability;
    public Vector3 customSpawnScale = Vector3.one; // Scala personalizzata
    public Vector3 customRotationSpeed = new Vector3(0, 100, 0); // Velocità di rotazione personalizzata
        public Vector3 initialRotation = Vector3.zero; 
}

    public CarProbability[] carProbabilities; // Array di tutte le macchine e le loro probabilità

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

    public void OpenLootBox(int boxCost)
    {
        if (!CurrencyManager.Instance.HasEnoughCoins(boxCost))
        {
            Debug.Log("Non hai abbastanza monete.");
            return;
        }
        if (lootBoxAnimator == null)
{
    Debug.LogError("Animator non assegnato!");
    return;
}

    // Disattiva il canvas e avvia l'animazione
    lootBoxCanvas.enabled = false;
    lootBoxAnimator.SetTrigger("OpenChest");
    
         string selectedCar = SelectRandomCar();
        SaveCar(selectedCar);
        CurrencyManager.Instance.TrySpendCoins(boxCost);
        CurrencyManager.Instance.UpdateCoinsUI();
         // Avvia la coroutine per gestire il flusso
    StartCoroutine(WaitAndReactivateCanvas());  
        // Avvia la coroutine per spawnare la macchina dopo l'animazione
    StartCoroutine(SpawnCarAfterAnimation(selectedCar));
    }

private IEnumerator WaitAndReactivateCanvas()
{
    // Attendi la fine dell'animazione 
    yield return new WaitForSeconds(4f);
    
    // Riattiva il canvas
    lootBoxCanvas.enabled = true;
    
    // Resetta l'animazione della cassa qui se necessario
    lootBoxAnimator.SetTrigger("ResetChest");
}
    private string SelectRandomCar()
    {
        float totalProbability = 0;
        foreach (var car in carProbabilities)
        {
            totalProbability += car.probability;
        }

        float randomPoint = Random.value * totalProbability;
        foreach (var car in carProbabilities)
        {
            if (randomPoint < car.probability)
                return car.carName;
            randomPoint -= car.probability;
        }

        return ""; // Default return, non dovrebbe mai accadere
    }

    private void SaveCar(string carName)
    {
        DatabaseReference carRef = databaseReference.Child("users").Child(userId).Child("ownedCars").Child(carName);
        carRef.SetValueAsync(true).ContinueWithOnMainThread(task => 
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Errore nel salvare la macchina.");
            }
            else if (task.IsCompleted)
            {
                Debug.Log(carName + " sbloccata con successo.");
            }
        });
          
    }
    private IEnumerator SpawnCarAfterAnimation(string carName)
{
    // Attendi la fine dell'animazione della cassa
    yield return new WaitForSeconds(3f); 

    // Spawn del prefab della macchina
    SpawnCarPrefab(carName);
}
private void SpawnCarPrefab(string carName)
{
    var carData = carProbabilities.FirstOrDefault(car => car.carName == carName);
    if(carData == null || carData.carPrefab == null)
    {
        Debug.LogError("Car prefab not found: " + carName);
        return;
    }

    GameObject carInstance = Instantiate(carData.carPrefab, spawnPoint.position, Quaternion.Euler(carData.initialRotation));
    carInstance.transform.localScale = carData.customSpawnScale;
    carInstance.transform.SetParent(spawnPoint);

    StartCoroutine(MoveAndRotate(carInstance, carData.customRotationSpeed));
    Destroy(carInstance, destructionDelay);
}


private IEnumerator MoveAndRotate(GameObject carInstance, Vector3 rotationSpeed)
{
    float startTime = Time.time;

    while(Time.time - startTime < destructionDelay)
    {
        carInstance.transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
        carInstance.transform.Rotate(rotationSpeed * Time.deltaTime);

        yield return null;
    }
}


    // Altre funzioni...
}