using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnDelay = 5f;
    public float spawnInterval = 3f;
    public BoxCollider spawnArea; // Assegna questo dall'Editor di Unity
    public float minSpawnDistanceFromPlayer = 30f; // Distanza minima dal giocatore per lo spawn
    public int maxSpawnAttempts = 10; // Numero massimo di tentativi per trovare una posizione valida

    void Start()
    {
        if(spawnArea == null)
        {
            Debug.LogError("Spawn Area BoxCollider non assegnato su EnemyManager."); 
            return;
        }
        InvokeRepeating("BeginEnemySpawn", spawnDelay, spawnInterval);
    }

    public void BeginEnemySpawn()
    {
        GameObject playerCar = GameObject.FindGameObjectWithTag("Player");
        if (playerCar == null) return;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector3 potentialSpawnPosition = GetRandomPositionInSpawnArea();

            if (Vector3.Distance(playerCar.transform.position, potentialSpawnPosition) >= minSpawnDistanceFromPlayer)
            {
                Instantiate(enemyPrefab, potentialSpawnPosition, Quaternion.identity);
                break; // Uscire dal ciclo una volta trovata una posizione valida
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
        // Assicurati che la posizione di spawn sia all'interno del collider (al livello del suolo, se necessario)
        spawnPosition.y = spawnArea.transform.position.y; // Ad esempio, regola questa linea se i nemici devono spawnare a un certo livello del suolo

        return spawnPosition;
    }
}
