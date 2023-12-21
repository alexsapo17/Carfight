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
        if (!photonView.IsMine || !other.CompareTag("Player"))
            return;

        PhotonView playerPhotonView = other.GetComponent<PhotonView>();

        if (playerPhotonView != null && playerPhotonView.IsMine)
        {
            PlayerEffects playerEffects = other.gameObject.GetComponent<PlayerEffects>();
            if (playerEffects != null)
            {
                playerEffects.StartInvisibilityTimer(effectDuration);
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
