using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class ActivateFlashbangFromButton : MonoBehaviour
{
    public Button flashbangButton; // Assegna questo nell'Inspector
    public float cooldownDuration = 10f; // Durata del cooldown prima che il pulsante possa essere riattivato
    public float fadeDuration = 1f; // Durata dell'animazione di fading

    private bool isCooldown = false; // Flag per controllare lo stato del cooldown

    void Start()
    {
        flashbangButton.onClick.AddListener(ActivateFlashbang);
    }

    void ActivateFlashbang()
    {
        if (isCooldown) return;
        
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
                playerEffects.StartFlashbangEffect();
                StartCooldown(); // Inizia il cooldown dopo aver attivato l'effetto
                StartCoroutine(FadeButton(0f, 1f)); // Inizia l'animazione di fading
                break;
            }
        }
    }

    void StartCooldown()
    {
        isCooldown = true; // Attiva il flag di cooldown
        flashbangButton.interactable = false; // Disabilita il pulsante
        Invoke("ResetCooldown", cooldownDuration); // Imposta un timer per resettare il cooldown
    }

    void ResetCooldown()
    {
        isCooldown = false; // Resetta il flag di cooldown
        flashbangButton.interactable = true; // Riabilita il pulsante
    }

    IEnumerator FadeButton(float startAlpha, float targetAlpha)
    {
        Graphic graphic = flashbangButton.GetComponent<Graphic>();
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
