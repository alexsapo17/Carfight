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
        // Scegli un indice casuale per lo spawn point
        int randomIndex = Random.Range(0, spawnPoints.Length);
        // Verifica se lo spawn point è libero
        if (!isSpawnPointOccupied[randomIndex])
        {
            GameObject pickupToSpawn = pickupPrefabs[Random.Range(0, pickupPrefabs.Length)];
            Transform spawnPoint = spawnPoints[randomIndex];

            // Istanzia il power-up nella posizione dello spawn point
            GameObject pickupInstance = PhotonNetwork.Instantiate(pickupToSpawn.name, spawnPoint.position, spawnPoint.rotation);
            // Imposta il manager e l'indice dello spawn point nel PickupEffect
            PickupEffect pickupEffect = pickupInstance.GetComponent<PickupEffect>();
            if (pickupEffect != null)
            {
                pickupEffect.Setup(randomIndex, this);
            }
            // Segna lo spawn point come occupato
            isSpawnPointOccupied[randomIndex] = true;
        }
    }

    public void FreeSpawnPoint(int index)
    {
        // Libera lo spawn point
        isSpawnPointOccupied[index] = false;
    }
}
