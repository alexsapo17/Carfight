using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PlayerDuplicator : MonoBehaviour
{
    public Button cubeButton; // Assegna questo nell'Inspector
    public GameObject cubePrefab; // Assegna il prefab del cubo nell'Inspector
    public float cubeDuration = 5f; // Durata del cubo in secondi
    public Vector3 offsetDistance = new Vector3(2f, 2f, 2f); // Distanza di offset dal giocatore su ciascun asse
    public Vector3 cubeScale = Vector3.one; // Scala del cubo
    public float cooldownDuration = 10f; // Durata del cooldown prima che il pulsante possa essere riattivato

    private bool isCooldown = false; // Flag per controllare lo stato del cooldown

    void Start()
    {
        cubeButton.onClick.AddListener(ActivateCube);
    }

    public void ActivateCube()
    {
        if (isCooldown) return;
        
        // Cerca tra tutti i PhotonView presenti nella scena
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            // Controlla se il PhotonView appartiene al giocatore locale e ha un componente PlayerEffects
            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                // Ottieni il componente PlayerEffects e attiva l'effetto del cubo
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();

                // Attiva l'effetto del cubo passando i valori di durata, distanza di offset e scala
                playerEffects.ActivateCubeEffect(cubePrefab, cubeDuration, offsetDistance, cubeScale);
                
                StartCooldown(); // Inizia il cooldown dopo aver attivato l'effetto
                StartCoroutine(FadeButton(0f, 1f)); // Inizia l'animazione di fading
                break; // Interrompe il ciclo una volta trovato e attivato l'effetto
            }
        }
    }

    void StartCooldown()
    {
        isCooldown = true; // Attiva il flag di cooldown
        cubeButton.interactable = false; // Disabilita il pulsante
        Invoke("ResetCooldown", cooldownDuration); // Imposta un timer per resettare il cooldown
    }

    void ResetCooldown()
    {
        isCooldown = false; // Resetta il flag di cooldown
        cubeButton.interactable = true; // Riabilita il pulsante
    }

    IEnumerator FadeButton(float startAlpha, float targetAlpha)
    {
        Graphic graphic = cubeButton.GetComponent<Graphic>();
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
