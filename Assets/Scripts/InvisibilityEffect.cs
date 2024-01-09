using UnityEngine;
using Photon.Pun;
public class InvisibilityEffect : MonoBehaviourPunCallbacks
{
    public float effectDuration = 5f;
    private int spawnPointIndex = -1;
    private PickupsManager pickupsManager;

    public void Setup(int index, PickupsManager manager)
    {
        spawnPointIndex = index;
        pickupsManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica se l'oggetto che ha colpito il power-up è un giocatore
        if (other.CompareTag("Player"))
        {
            PlayerEffects playerEffects = other.gameObject.GetComponent<PlayerEffects>();

            // Applica l'effetto al giocatore e distruggi il power-up
            if (playerEffects != null)
            {
                playerEffects.StartInvisibilityTimer(effectDuration);
                DestroyPickup();
            }
        }
    }

    void DestroyPickup()
    {
        // Libera il punto di spawn e distruggi il power-up
        if (spawnPointIndex != -1 && pickupsManager != null)
        {
            pickupsManager.FreeSpawnPoint(spawnPointIndex);
        }
        PhotonNetwork.Destroy(gameObject);
    }
}
