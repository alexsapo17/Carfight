using System;
using UnityEngine;

[Serializable]
public class SpawnableItem
{
    public GameObject prefab;
    public float minIncline;
    public float maxIncline;
}

public class DynamicTreeSpawner : MonoBehaviour
{
    public SpawnableItem[] itemsToSpawn;
    public int numberOfItems = 10; // Numero totale di oggetti da spawnare
    public Terrain terrain;
    public float margin = 10f;

    void Start()
    {
        foreach (var item in itemsToSpawn)
        {
            SpawnItem(item);
        }
    }

    void SpawnItem(SpawnableItem item)
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrainWithMargin(item.minIncline, item.maxIncline);
            if (spawnPosition != Vector3.zero) // Verifica che una posizione valida sia stata trovata
            {
                Instantiate(item.prefab, spawnPosition, Quaternion.identity, transform);
            }
        }
    }

    Vector3 GetRandomPositionOnTerrainWithMargin(float minIncline, float maxIncline)
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        for (int attempts = 0; attempts < 100; attempts++) // Tentativi limitati per evitare loop infiniti
        {
 float randomX = UnityEngine.Random.Range(margin, terrainSize.x - margin) + terrainPos.x;
float randomZ = UnityEngine.Random.Range(margin, terrainSize.z - margin) + terrainPos.z;

            float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPos.y;
            Vector3 position = new Vector3(randomX, y, randomZ);
            Vector3 normal = terrain.terrainData.GetInterpolatedNormal((randomX - terrainPos.x) / terrainSize.x, (randomZ - terrainPos.z) / terrainSize.z);
            float angle = Vector3.Angle(normal, Vector3.up);

            if (angle >= minIncline && angle <= maxIncline)
            {
                return position; // La posizione soddisfa i criteri di inclinazione
            }
        }

        return Vector3.zero; // Indica che non è stata trovata una posizione valida
    }
}
