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
        if (other.CompareTag("Player"))
        {
            PlayerEffects playerEffects = other.gameObject.GetComponent<PlayerEffects>();

            if (playerEffects != null)
            {
                playerEffects.StartInvisibilityTimer(effectDuration);
                RequestDestroyPickup();
            }
        }
    }

    void RequestDestroyPickup()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            DestroyPickup();
        }
        else
        {
            photonView.RPC("DestroyPickupRPC", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void DestroyPickupRPC()
    {
        DestroyPickup();
    }

    void DestroyPickup()
    {
        if (spawnPointIndex != -1 && pickupsManager != null)
        {
            pickupsManager.FreeSpawnPoint(spawnPointIndex);
        }
        PhotonNetwork
.Destroy(gameObject);
}
}

