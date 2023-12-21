using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PickupEffect : MonoBehaviourPunCallbacks
{
    public float effectDuration = 5f;
    public float sizeMultiplier = 1.5f;
    public float massMultiplier = 1.5f;
public float accelerationMultiplier = 1.5f;
    private int spawnPointIndex = -1;
    private PickupsManager pickupsManager;

    public void Setup(int index, PickupsManager manager)
    {
        spawnPointIndex = index;
        pickupsManager = manager;
    }

 private void OnTriggerEnter(Collider other)
{
    if (!photonView.IsMine || !other.CompareTag("Player"))
        return;

    PhotonView playerPhotonView = other.GetComponent<PhotonView>();

    if (playerPhotonView != null && playerPhotonView.IsMine)
    {
        PlayerEffects playerEffects = other.gameObject.GetComponent<PlayerEffects>();
        if (playerEffects != null)
        {
            // Chiama il metodo per iniziare l'effetto di aumento di grandezza/massa
            playerEffects.StartSizeMassIncreaseTimer(effectDuration, sizeMultiplier, massMultiplier);

            // Aggiungi questa chiamata per iniziare l'effetto di aumento dell'accelerazione e dell'attrito
            playerEffects.StartSpeedAndFrictionEffect(effectDuration, accelerationMultiplier);
        }
        photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
    }
}


    [PunRPC]
    void DestroyPickup()
    {
        if (photonView.IsMine)
        {
            if (spawnPointIndex != -1 && pickupsManager != null)
            {
                pickupsManager.FreeSpawnPoint(spawnPointIndex);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
