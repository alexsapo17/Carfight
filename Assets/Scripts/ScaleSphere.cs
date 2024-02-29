using UnityEngine;
using Photon.Pun; // Assicurati di avere Photon PUN nel tuo progetto

public class ScaleSphere : MonoBehaviourPunCallbacks
{
    public float scaleSpeed = 1f; // Velocità di scala regolabile dall'Inspector
    private float targetScaleX = 150f; // Scala target sull'asse X

    private void Update()
    {
        if (photonView.IsMine) // Assicurati che solo il giocatore che controlla questo oggetto esegua il codice
        {
            ScaleObject();
        }
    }

    void ScaleObject()
    {
        if (transform.localScale.x < targetScaleX)
        {
            // Calcola la nuova scala
            float newScaleX = Mathf.MoveTowards(transform.localScale.x, targetScaleX, scaleSpeed * Time.deltaTime);
            // Aggiorna la scala dell'oggetto
            transform.localScale = new Vector3(newScaleX, 150f, 150f);
        }
    }
}
