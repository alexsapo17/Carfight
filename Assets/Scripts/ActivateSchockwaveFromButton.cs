using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ActivateShockwaveFromButton : MonoBehaviour
{
    public Button shockwaveButton; // Assegna questo nell'Inspector

    void Start()
    {
        shockwaveButton.onClick.AddListener(ActivateShockwave);
    }

    void ActivateShockwave()
    {
        // Cerca tra tutti i PhotonView presenti nella scena
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            // Controlla se il PhotonView appartiene al giocatore locale e ha un componente PlayerEffects
            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                // Ottieni il componente PlayerEffects e attiva l'effetto
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
                playerEffects.StartShockwaveEffect();
                break; // Interrompe il ciclo una volta trovato e attivato l'effetto
            }
        }
    }
}
