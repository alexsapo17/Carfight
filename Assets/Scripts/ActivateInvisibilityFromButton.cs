using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

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
        if (view.IsMine && view.gameObject.CompareTag("Player"))
            {
                PlayerEffects playerEffects = view.gameObject.GetComponent<PlayerEffects>();

                if (playerEffects != null)
                {
                    playerEffects.StartInvisibilityTimer(invisibilityDuration);
                    StartCooldown(); // Inizia il cooldown dopo aver attivato l'invisibilità
                    StartCoroutine(FadeButton(0f, 1f)); // Inizia l'animazione di fading
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

    IEnumerator FadeButton(float startAlpha, float targetAlpha)
    {
        Graphic graphic = invisibilityButton.GetComponent<Graphic>();
        Color startColor = graphic.color;
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < cooldownDuration)
        {
            // Calcola l'opacità in base al tempo trascorso rispetto alla durata del cooldown
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / cooldownDuration);
            
            // Imposta il colore con l'opacità calcolata
            graphic.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            // Aggiorna il tempo trascorso
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Assicurati di impostare il valore finale
        graphic.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}
