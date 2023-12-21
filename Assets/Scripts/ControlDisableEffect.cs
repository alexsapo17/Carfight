using UnityEngine;
using Photon.Pun;

public class ControlDisableEffect : MonoBehaviourPunCallbacks
{
    public float effectDuration = 5f; // Durata dell'effetto
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
        // Invia l'effetto a tutti gli altri giocatori
        photonView.RPC("ApplyControlDisableEffect", RpcTarget.Others, effectDuration);
        photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
    }
}

[PunRPC]
void ApplyControlDisableEffect(float duration)
{
    GameObject localPlayerGameObject = PhotonNetwork.LocalPlayer.TagObject as GameObject;
    if (localPlayerGameObject != null)
    {
        PlayerEffects playerEffects = localPlayerGameObject.GetComponent<PlayerEffects>();
        if (playerEffects != null)
        {
            playerEffects.StartControlDisableTimer(duration);
        }
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
