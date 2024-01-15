using UnityEngine;
using Photon.Pun;

public class ControlDisableEffect : MonoBehaviourPunCallbacks
{
    public float effectDuration = 5f; // Durata dell'effetto
    private int spawnPointIndex = -1;
    private PickupsManager pickupsManager;
    public GameObject explosionEffectPrefab;

    public void Setup(int index, PickupsManager manager)
    {
        spawnPointIndex = index;
        pickupsManager = manager;
    }

private void OnTriggerEnter(Collider other)
{
    Debug.Log("[ControlDisableEffect] Pickup interagito da: " + other.gameObject.name);
    if (other.CompareTag("Player"))
    {
        PhotonView playerPhotonView = other.GetComponent<PhotonView>();

        if (playerPhotonView != null)
        {
            photonView.RPC("ApplyControlDisableEffect", RpcTarget.All, effectDuration, playerPhotonView.ViewID);
            photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
            Debug.Log("[ControlDisableEffect] Inviato RPC per disabilitare i controlli degli altri giocatori.");
        }
    }
}


[PunRPC]
void ApplyControlDisableEffect(float duration, int playerViewID)
{
    Debug.Log($"[ControlDisableEffect] Tentativo di applicare l'effetto di disabilitazione per {duration} secondi. PlayerViewID: {playerViewID}");
    if (PhotonNetwork.LocalPlayer.ActorNumber != playerViewID)
    {
        GameObject localPlayerGameObject = PhotonNetwork.LocalPlayer.TagObject as GameObject; 
        if (localPlayerGameObject != null)
        {
            PlayerEffects playerEffects = localPlayerGameObject.GetComponent<PlayerEffects>();
            if (playerEffects != null)
            {
                playerEffects.StartControlDisableTimer(duration);
                Debug.Log("[ControlDisableEffect] Applicato l'effetto di disabilitazione.");
            }
        }
    }

}


    [PunRPC]
    void DestroyPickup()
    {
        if (photonView.IsMine)
        {
            // Crea l'effetto di esplosione prima di distruggere il pickup
            if (explosionEffectPrefab != null)
            {
                Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            }

            // Il resto del codice rimane invariato
            if (spawnPointIndex != -1 && pickupsManager != null)
            {
                pickupsManager.FreeSpawnPoint(spawnPointIndex);
            }
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
