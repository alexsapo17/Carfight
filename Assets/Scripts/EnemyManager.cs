using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject[] enemyPrefabs; // Lista dei prefabs dei nemici normali
    public GameObject[] specialEnemyPrefabs; // Lista dei prefabs dei nemici speciali
    public float spawnDelay = 5f;
    public float spawnInterval = 3f;
    public float specialEnemySpawnChance = 0.2f; // Probabilità di spawn per il nemico speciale
    public BoxCollider spawnArea;
    public float minSpawnDistanceFromPlayer = 30f;
    public int maxSpawnAttempts = 10;

    private bool isSpawning = false;

    void Start()
    {
        if (spawnArea == null)
        {
            Debug.LogError("Spawn Area BoxCollider non assegnato su EnemyManager.");
            return;
        }
    }

    public void StartSpawn()
    {
        isSpawning = true;
        InvokeRepeating("BeginEnemySpawn", spawnDelay, spawnInterval);
    }

    public void StopSpawn()
    {
        isSpawning = false;
        CancelInvoke("BeginEnemySpawn");
    }

    public void BeginEnemySpawn()
    {
        if (!isSpawning)
            return;

        GameObject playerCar = GameObject.FindGameObjectWithTag("Player");
        if (playerCar == null) return;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 potentialSpawnPosition = GetRandomPositionInSpawnArea();

            if (Vector3.Distance(playerCar.transform.position, potentialSpawnPosition) >= minSpawnDistanceFromPlayer)
            {
                // Genera un numero casuale tra 0 e 1 per determinare se spawnare il nemico normale o il nemico speciale
                float spawnRoll = Random.value;
                GameObject[] prefabsToSpawn = (spawnRoll < specialEnemySpawnChance) ? specialEnemyPrefabs : enemyPrefabs;

                // Scegli casualmente uno dei prefabs nella lista
                GameObject prefabToSpawn = prefabsToSpawn[Random.Range(0, prefabsToSpawn.Length)];

                Instantiate(prefabToSpawn, potentialSpawnPosition, Quaternion.identity);
                break;
            }
        }
    }

    Vector3 GetRandomPositionInSpawnArea()
    {
        Vector3 basePosition = spawnArea.bounds.center;
        Vector3 size = spawnArea.bounds.size;

        float spawnPosX = basePosition.x + Random.Range(-size.x / 2, size.x / 2);
        float spawnPosY = basePosition.y + Random.Range(-size.y / 2, size.y / 2);
        float spawnPosZ = basePosition.z + Random.Range(-size.z / 2, size.z / 2);

        Vector3 spawnPosition = new Vector3(spawnPosX, spawnPosY, spawnPosZ);
        spawnPosition.y = spawnArea.transform.position.y;

        return spawnPosition;
    }
}
