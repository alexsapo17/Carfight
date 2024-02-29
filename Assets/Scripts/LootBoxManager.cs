using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using Firebase.Extensions;
using System.Linq;

public enum CurrencyType
{
    Coins,
    Gems,
    Both
}

public class LootBoxManager : MonoBehaviour
{
    private DatabaseReference databaseReference;
    private string userId;
public Canvas lootBoxCanvas; 

public Transform spawnPoint; // Punto dove spawnare la macchina
public Vector3 spawnScale = Vector3.one; // Scala del prefab al momento dello spawn
public Vector3 rotationSpeed = new Vector3(0, 100, 0); // Velocità di rotazione (gradi al secondo)
public float moveUpSpeed = 1f; // Velocità di movimento verso l'alto
public float destructionDelay = 2f; // Tempo dopo il quale il prefab verrà distrutto
public LootBox[] lootBoxes;
public GameObject tutorial3Panel;
public GameObject tutorial4Panel;

[SerializeField]
private List<GameObject> objectsToToggle = new List<GameObject>();

[System.Serializable]
public class LootBox
{
    public int boxCostCoins; // Costo in monete per aprire questa cassa
    public int boxCostGems; // Costo in gemme per aprire questa cassa
    public CurrencyType currencyType; // Tipo di valuta richiesta
    public Animator lootBoxAnimator;
    public CarProbability[] carProbabilities; 
    public int experienceGain;
    public ParticleSystem particleSystemToStop;
}



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


    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
    }

public void OpenLootBox(int lootBoxIndex)
{
                if (PlayerPrefs.GetInt("ShowTutorial2Panel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorial3Panel.SetActive(false);


    }
    LootBox selectedBox = lootBoxes[lootBoxIndex];
    
    bool canOpen = false;
    switch (selectedBox.currencyType)
    {
        case CurrencyType.Coins:
            if (CurrencyManager.Instance.HasEnoughCoins(selectedBox.boxCostCoins))
            {
                CurrencyManager.Instance.TrySpendCoins(selectedBox.boxCostCoins);
                canOpen = true;
            }
            break;
        case CurrencyType.Gems:
            if (CurrencyManager.Instance.HasEnoughGems(selectedBox.boxCostGems))
            {
                CurrencyManager.Instance.TrySpendGems(selectedBox.boxCostGems);
                canOpen = true;
            }
            break;
        case CurrencyType.Both:
            if (CurrencyManager.Instance.HasEnoughCoins(selectedBox.boxCostCoins) && CurrencyManager.Instance.HasEnoughGems(selectedBox.boxCostGems))
            {
                CurrencyManager.Instance.TrySpendCoins(selectedBox.boxCostCoins);
                CurrencyManager.Instance.TrySpendGems(selectedBox.boxCostGems);
                canOpen = true;
            }
            break;
    }
    
    if (!canOpen)
    {
        Debug.Log("Non hai abbastanza risorse per aprire questa cassa.");
        return;
    }
        // Disattiva i GameObject specificati
    foreach (GameObject obj in objectsToToggle)
    {
        obj.SetActive(false);
    }
    selectedBox.lootBoxAnimator.SetTrigger("OpenChest");
    string selectedCar = SelectRandomCar(selectedBox.carProbabilities);
    SaveCar(selectedCar);
    CurrencyManager.Instance.UpdateCoinsUI(); // Assicurati che questa funzione aggiorni anche l'UI delle gemme se necessario
    // Avvia la coroutine per gestire il flusso
StartCoroutine(WaitAndReactivateCanvas(selectedBox.lootBoxAnimator, selectedBox));
    // Avvia la coroutine per spawnare la macchina dopo l'animazione
    StartCoroutine(SpawnCarAfterAnimation(selectedCar, selectedBox.carProbabilities));
    ExperienceManager.Instance.AddExperience(selectedBox.experienceGain);
}




private IEnumerator WaitAndReactivateCanvas(Animator animator, LootBox lootBox) 
{
    // Attendi la fine dell'animazione 
    yield return new WaitForSeconds(4.5f);
    
    // Riattiva gli oggetti specificati
    foreach (GameObject obj in objectsToToggle)
    {
        obj.SetActive(true);
    }
    
    // Controlla e ferma il sistema di particelle per la cassa specificata
    if (lootBox.particleSystemToStop != null)
    {
        lootBox.particleSystemToStop.Stop();
    }

    // Resetta l'animazione della cassa
    animator.SetTrigger("ResetChest");
}



    private string SelectRandomCar(CarProbability[] carProbabilities)
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
private IEnumerator SpawnCarAfterAnimation(string carName, CarProbability[] carProbabilities)
{
    // Attendi la fine dell'animazione della cassa
    yield return new WaitForSeconds(3f); 

    // Spawn del prefab della macchina
    SpawnCarPrefab(carName, carProbabilities);
}

private void SpawnCarPrefab(string carName, CarProbability[] carProbabilities)
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
                if (PlayerPrefs.GetInt("ShowTutorial2Panel", 0) == 1)
    {
        // Mostra il pannello speciale
        tutorial4Panel.SetActive(true);
PlayerPrefs.SetInt("ShowTutorial2Panel", 0);
PlayerPrefs.SetInt("ShowTutorialCustomPanel", 1);
    }
}

private IEnumerator MoveAndRotate(GameObject carInstance, Vector3 rotationSpeed)
{
    Vector3 startPosition = carInstance.transform.position;
    Vector3 endPosition = startPosition + Vector3.up * moveUpSpeed * destructionDelay;
    Quaternion startRotation = carInstance.transform.rotation;
    Quaternion endRotation = startRotation * Quaternion.Euler(rotationSpeed * destructionDelay);

    float elapsedTime = 0f;

    while (elapsedTime < destructionDelay)
    {
        float fracComplete = elapsedTime / destructionDelay;
        carInstance.transform.position = Vector3.Lerp(startPosition, endPosition, fracComplete);
        carInstance.transform.rotation = Quaternion.Slerp(startRotation, endRotation, fracComplete);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
}



    // Altre funzioni...
}