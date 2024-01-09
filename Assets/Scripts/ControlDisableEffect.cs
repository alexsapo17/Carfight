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
        if (other.CompareTag("Player"))
        {
            PhotonView playerPhotonView = other.GetComponent<PhotonView>();

            // Invia l'effetto a tutti gli altri giocatori
            if (playerPhotonView != null)
            {
                photonView.RPC("ApplyControlDisableEffect", RpcTarget.Others, effectDuration, playerPhotonView.ViewID);
                photonView.RPC("DestroyPickup", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    void ApplyControlDisableEffect(float duration, int playerViewID)
    {
        // Evita di applicare l'effetto al giocatore che ha raccolto il pickup
        if (PhotonNetwork.LocalPlayer.ActorNumber != playerViewID)
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
