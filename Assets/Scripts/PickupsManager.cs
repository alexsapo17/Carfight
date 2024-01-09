using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PickupsManager : MonoBehaviourPun
{
    public GameObject[] pickupPrefabs;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;

    private float timer;
    private bool[] isSpawnPointOccupied;

    private void Start()
    {
        // Inizializza l'array in base al numero di spawn points
        isSpawnPointOccupied = new bool[spawnPoints.Length];
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        timer += Time.deltaTime;

        if (timer > spawnInterval)
        {
            SpawnPickup();
            timer = 0;
        }
    }

void SpawnPickup()
{
    int attempts = spawnPoints.Length; // Numero massimo di tentativi per trovare uno spawn point libero

    while (attempts > 0)
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        if (!isSpawnPointOccupied[randomIndex])
        {
            GameObject pickupToSpawn = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
            Transform spawnPoint = spawnPoints[randomIndex];

            GameObject pickupInstance = PhotonNetwork.Instantiate(pickupToSpawn.name, spawnPoint.position, spawnPoint.rotation);

            PickupEffect pickupEffect = pickupInstance.GetComponent<PickupEffect>();
            if (pickupEffect != null)
            {
                pickupEffect.Setup(randomIndex, this);
            }

            isSpawnPointOccupied[randomIndex] = true;
            break; // Uscita dal ciclo dopo lo spawn di un pickup
        }

        attempts--; // Decrementa il numero di tentativi rimasti
    }
}


    public void FreeSpawnPoint(int index)
    {
        // Libera lo spawn point
        isSpawnPointOccupied[index] = false;
    }
}
