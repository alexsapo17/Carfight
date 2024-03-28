using UnityEngine;
using Photon.Pun;

public class BallSpawner : MonoBehaviourPunCallbacks
{
    public GameObject[] ballPrefabs; // Array dei prefab delle palle da spawnare
    public Transform spawnPoint; // Punto di spawn delle palle
    public float spawnInterval = 5f; // Intervallo di tempo tra uno spawn e l'altro
    private bool isSpawning = false; // Flag per controllare se lo spawn è attivo

    public void StartSpawnProcess()
    {
        if (!isSpawning)
        {
            // Avvia lo spawn periodico delle palle
            InvokeRepeating("SpawnBall", 0f, spawnInterval);
            isSpawning = true;
        }
    }

    public void StopSpawnProcess()
    {
        if (isSpawning)
        {
            // Interrompe lo spawn periodico delle palle
            CancelInvoke("SpawnBall");
            isSpawning = false;
        }
    }

    void SpawnBall()
    {
        // Scegli un prefab casuale dall'array
        GameObject ballPrefab = ballPrefabs[UnityEngine.Random.Range(0, ballPrefabs.Length)]; // Usa UnityEngine.Random

        // Spawnare il prefab alla posizione dello spawn point
        PhotonNetwork.Instantiate(ballPrefab.name, spawnPoint.position, Quaternion.identity);
    }
}
