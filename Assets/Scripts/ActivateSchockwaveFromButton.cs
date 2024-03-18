using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;
public class ActivateShockwaveFromButton : MonoBehaviour
{
    public Button shockwaveButton; // Assegna questo nell'Inspector
    public GameObject visualEffectPrefab; // Assegna il prefab dell'effetto visivo nell'Inspector
    private bool isCooldown = false;
    public float cooldownDuration = 10f; // Durata del cooldown prima che il pulsante possa essere riattivato

    void Start()
    {
        shockwaveButton.onClick.AddListener(ActivateShockwave);
    }

    public void ActivateShockwave()
{
    if (isCooldown) return;
    // Cerca tra tutti i PhotonView presenti nella scena
    foreach (var pv in FindObjectsOfType<PhotonView>())
    {
        // Controlla se il PhotonView appartiene al giocatore locale, ha un componente PlayerEffects e il tag "Player"
        if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>() && pv.gameObject.tag == "Player")
        {
            // Ottieni il componente PlayerEffects e attiva l'effetto
            var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
            playerEffects.StartShockwaveEffect();

            // Istanziare l'effetto visivo nel punto della macchina
            Instantiate(visualEffectPrefab, pv.transform.position, Quaternion.identity);
            StartCooldown();
            break; // Interrompe il ciclo una volta trovato e attivato l'effetto
        }
    }
}


    void StartCooldown()
    {
        isCooldown = true; // Attiva il flag di cooldown
        shockwaveButton.interactable = false; // Disabilita il pulsante
        StartCoroutine(FadeButton(0f, 1f)); // Inizia l'animazione di fading
        Invoke("ResetCooldown", cooldownDuration); // Imposta un timer per resettare il cooldown
    }

    void ResetCooldown()
    {
        isCooldown = false; // Resetta il flag di cooldown
        shockwaveButton.interactable = true; // Riabilita il pulsante
    }

IEnumerator FadeButton(float startAlpha, float targetAlpha)
{
    Graphic graphic = shockwaveButton.GetComponent<Graphic>();
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
