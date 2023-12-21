using UnityEngine;
using Photon.Pun;

public class BrickDestruction : MonoBehaviourPun
{
    public float destructionDelay = 5f;
    private bool hasImpacted = false;

    void OnCollisionEnter(Collision collision)
    {
        if (!hasImpacted)
        {
            hasImpacted = true;
            Invoke(nameof(DestroyBrick), destructionDelay);
        }
    }

    void DestroyBrick()
    {
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
