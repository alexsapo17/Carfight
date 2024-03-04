using UnityEngine;
using System.Collections.Generic; // Necessario per usare le liste
using System.Collections;
public class SurvivalPickupsManager : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject gemPrefab;
    public int coinsToSpawn = 10;
    public int gemsToSpawn = 5;
    public Terrain terrain;
    public UnityEngine.UI.Text coinsToRewardText;
    public UnityEngine.UI.Text gemsToRewardText;
    public float spawnInterval = 5f; // Intervallo di tempo tra gli spawn in secondi


    private List<GameObject> spawnedPickups = new List<GameObject>(); // Lista per tenere traccia dei pickup spawnati
    private float safeMargin = 10f; // Margine di sicurezza per evitare lo spawn vicino ai bordi

    private int totalCoins = 0;
    private int totalGems = 0;

    void Start()
    {
        if (!terrain) terrain = Terrain.activeTerrain;

    }
    public void StartRace()
      {
          StartCoroutine(SpawnPickupsPeriodically());
}

    IEnumerator SpawnPickupsPeriodically()
    {
        while (true) // Loop infinito per continuare a spawnare pickup
        {
            SpawnPickups(coinPrefab, coinsToSpawn);
            SpawnPickups(gemPrefab, gemsToSpawn);
            yield return new WaitForSeconds(spawnInterval); // Aspetta per l'intervallo di tempo prima del prossimo spawn
        }
    }
   void SpawnPickups(GameObject prefab, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrainWithMargin();
            GameObject spawned = Instantiate(prefab, spawnPosition, Quaternion.identity);
            spawnedPickups.Add(spawned); // Aggiungi il pickup spawnato alla lista
        }
    }

  Vector3 GetRandomPositionOnTerrainWithMargin()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        float randomX = Random.Range(safeMargin, terrainSize.x - safeMargin) + terrainPos.x;
        float randomZ = Random.Range(safeMargin, terrainSize.z - safeMargin) + terrainPos.z;
        float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPos.y;

        return new Vector3(randomX, y, randomZ);
    }

      // Metodo per gestire la raccolta degli item
    public void CollectItem(string itemType, int value)
    {
        if (itemType == "coin")
        {
            totalCoins += value;
        }
        else if (itemType == "gem")
        {
            totalGems += value;
        }
        // Qui puoi aggiungere effetti o logiche specifiche per la raccolta
    }

     // Metodo da chiamare alla fine della partita per aggiungere i valori raccolti
    public void AssignCollectedItems()
    {
        if (totalCoins > 0)
        {
            CurrencyManager.Instance.ModifyCoins(totalCoins);
        coinsToRewardText.text = "+" + totalCoins ;

            totalCoins = 0; // Resetta il conteggio
        }

        if (totalGems > 0)
        {
            CurrencyManager.Instance.ModifyGems(totalGems); 
        gemsToRewardText.text = "+" + totalGems ;

            totalGems = 0; // Resetta il conteggio
        }
        

        }
            public void EndRace()
    {
        StopAllCoroutines(); // Ferma la coroutine dello spawn
        foreach (var pickup in spawnedPickups)
        {
            Destroy(pickup); // Distruggi tutti i pickup spawnati
        }
        spawnedPickups.Clear(); // Pulisci la lista
    }
}
