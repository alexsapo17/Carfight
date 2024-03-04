using UnityEngine;

public class DynamicTreeSpawner : MonoBehaviour
{
    public GameObject[] treePrefabs; // Usa un array per memorizzare più prefab
    public int numberOfTrees = 10; // Numero di alberi da spawnare
    public Terrain terrain; // Riferimento al Terrain
    public float margin = 10f; // Margine dai bordi del terreno

    void Start()
    {
        SpawnTrees();
    }

    void SpawnTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 spawnPosition = GetRandomPositionOnTerrainWithMargin();
            GameObject treePrefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
            Instantiate(treePrefab, spawnPosition, Quaternion.identity, transform);
        }
    }

    Vector3 GetRandomPositionOnTerrainWithMargin()
    {
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 terrainPos = terrain.transform.position;

        // Assicurati di usare lo stesso margine qui che usi per i pickup
        float randomX = Random.Range(margin, terrainSize.x - margin) + terrainPos.x;
        float randomZ = Random.Range(margin, terrainSize.z - margin) + terrainPos.z;
        float y = terrain.SampleHeight(new Vector3(randomX, 0, randomZ)) + terrainPos.y;

        return new Vector3(randomX, y, randomZ);
    }
}
