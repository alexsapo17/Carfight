using UnityEngine;
using Photon.Pun;
using UnityEngine.UI; // Aggiungi questo namespace per lavorare con l'UI

public class ActivateInvisibilityFromButton : MonoBehaviour
{
    public float invisibilityDuration = 5f; // Durata dell'effetto di invisibilità
    public float cooldownDuration = 10f; // Durata del cooldown prima che il pulsante possa essere riattivato
    public Button invisibilityButton; // Riferimento al pulsante di invisibilità nell'Inspector

    private bool isCooldown = false; // Flag per controllare lo stato del cooldown

    // Metodo da chiamare quando il pulsante UI viene premuto
    public void OnButtonClick()
    {
        if(isCooldown) return; // Se è in cooldown, non fare nulla

        // Cerca tutti i PhotonView nella scena
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();

        foreach (var view in photonViews)
        {
            if (view.IsMine)
            {
                PlayerEffects playerEffects = view.gameObject.GetComponent<PlayerEffects>();

                if (playerEffects != null)
                {
                    playerEffects.StartInvisibilityTimer(invisibilityDuration);
                    StartCooldown(); // Inizia il cooldown dopo aver attivato l'invisibilità
                    return;
                }
            }
        }

        Debug.LogError("ActivateInvisibilityFromButton: Impossibile trovare il componente PlayerEffects sul giocatore locale.");
    }

    void StartCooldown()
    {
        isCooldown = true; // Attiva il flag di cooldown
        invisibilityButton.interactable = false; // Disabilita il pulsante
        Invoke("ResetCooldown", cooldownDuration); // Imposta un timer per resettare il cooldown
    }

    void ResetCooldown()
    {
        isCooldown = false; // Resetta il flag di cooldown
        invisibilityButton.interactable = true; // Riabilita il pulsante
    }
}
