using UnityEngine;
using Photon.Pun;

public class RagdollController : MonoBehaviourPun
{
    private Rigidbody[] ragdollRigidbodies;
    private bool isRagdollActive = false;

    void Awake()
    {
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>(true);

        foreach (var rb in ragdollRigidbodies)
        {
            rb.useGravity = false;
        }
    }

    [PunRPC]
    public void ActivateRagdoll()
    {
        if (isRagdollActive) return;

        foreach (var rb in ragdollRigidbodies)
        {
            rb.useGravity = true;
        }
        isRagdollActive = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Attiva il ragdoll alla collisione se non è già attivo
        if (!isRagdollActive && photonView.IsMine)
        {
                Debug.Log("Collision detected with: " + collision.gameObject.name);

            photonView.RPC("ActivateRagdoll", RpcTarget.All);
        }
    }
    public void CollisionDetected(RagdollPart part, Collision collision)
{
    if (!isRagdollActive && photonView.IsMine)
    {
        photonView.RPC("ActivateRagdoll", RpcTarget.All);
    }
}

}
