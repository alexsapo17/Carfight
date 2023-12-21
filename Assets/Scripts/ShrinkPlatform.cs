using UnityEngine;
using Photon.Pun;

public class ShrinkPlatform : MonoBehaviourPunCallbacks, IPunObservable
{
    public float shrinkRate = 0.1f; 
    private Vector3 originalScale;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            originalScale = transform.localScale;
        }
    }

  void Update()
{
    if (PhotonNetwork.IsMasterClient && transform.localScale.x > 0.1f && transform.localScale.z > 0.1f)
    {
        // Assicurati che la piattaforma si rimpicciolisca uniformemente da centro
        Vector3 scaleChange = new Vector3(shrinkRate, 0, shrinkRate) * Time.deltaTime;
        transform.localScale -= scaleChange;

        // Aggiusta la posizione per mantenere la piattaforma centrata
        transform.position += new Vector3(scaleChange.x / 2, 0, scaleChange.z / 2);
    }
}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Trasmettiamo la scala corrente a tutti gli altri client
            stream.SendNext(transform.localScale);
        }
        else
        {
            // Riceviamo la scala aggiornata e la applichiamo
            transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
}
