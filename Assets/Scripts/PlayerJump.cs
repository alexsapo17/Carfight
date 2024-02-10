using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PlayerJump : MonoBehaviourPun
{
    public Button jumpButton; // Assegna questo nell'Inspector
    public float jumpForce = 10f; // Potenza del salto, regolabile dall'Inspector
    public float cooldownDuration = 10f; // Durata del cooldown prima che il pulsante possa essere riattivato

    private bool isCooldown = false; // Flag per controllare lo stato del cooldown

    void Start()
    {
        jumpButton.onClick.AddListener(ActivateJump);
    }

    void ActivateJump()
    {
        if (isCooldown) return;

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
                StartCooldown(); // Inizia il cooldown dopo aver attivato il salto
                StartCoroutine(FadeButton(0f, 1f)); // Inizia l'animazione di fading
                break; // Interrompe il ciclo una volta trovato e attivato il salto
            }
        }
    }

    void StartCooldown()
    {
        isCooldown = true; // Attiva il flag di cooldown
        jumpButton.interactable = false; // Disabilita il pulsante
        Invoke("ResetCooldown", cooldownDuration); // Imposta un timer per resettare il cooldown
    }

    void ResetCooldown()
    {
        isCooldown = false; // Resetta il flag di cooldown
        jumpButton.interactable = true; // Riabilita il pulsante
    }

    IEnumerator FadeButton(float startAlpha, float targetAlpha)
    {
        Graphic graphic = jumpButton.GetComponent<Graphic>();
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
