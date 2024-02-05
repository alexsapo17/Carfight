using UnityEngine;
using UnityEngine.UI; // Necessario per interagire con gli elementi UI
using Photon.Pun; // Assicurati di includere questo spazio dei nomi per usare PhotonView

public class PlayerJump : MonoBehaviourPun
{
   public Button jumpButton; // Assegna questo nell'Inspector
    public float jumpForce = 10f; // Potenza del salto, regolabile dall'Inspector

    void Start()
    {
        jumpButton.onClick.AddListener(ActivateJump);
    }

    void ActivateJump()
    {
        // Cerca tra tutti i PhotonView presenti nella scena
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            // Controlla se il PhotonView appartiene al giocatore locale e ha un componente PlayerEffects
            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                // Ottieni il componente PlayerEffects
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
                // Chiama il metodo per attivare il salto, assicurandosi che esista nel componente PlayerEffects
                playerEffects.StartJump(jumpForce);
                break; // Interrompe il ciclo una volta trovato e attivato il salto
            }
        }
    }
}