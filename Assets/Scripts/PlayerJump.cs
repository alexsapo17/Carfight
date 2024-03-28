using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PlayerJump : MonoBehaviourPun
{
    public Button jumpButton; // Assegna questo nell'Inspector
    public float jumpForce = 10f; // Potenza del salto, regolabile dall'Inspector
    public float cooldownDuration = 10f; // Durata del cooldown prima che il pulsante possa essere riattivato
    public GameObject jumpEffectPrefab; // Prefab dell'effetto di salto
    public Vector3 jumpEffectScale = Vector3.one; // Scala dell'effetto di salto
    public Vector3 jumpEffectRotation = new Vector3(90f, 0f, 0f); // Rotazione dell'effetto di salto

    private bool isCooldown = false; // Flag per controllare lo stato del cooldown

    void Start()
    {
        jumpButton.onClick.AddListener(ActivateJump);
    }

    void ActivateJump()
    {
        if (isCooldown) return;

        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
        // Verifica se il gameobject ha il tag "Player"
        if (pv.gameObject.CompareTag("Player"))
        {

            if (pv.IsMine && pv.gameObject.GetComponent<PlayerEffects>())
            {
                var playerEffects = pv.gameObject.GetComponent<PlayerEffects>();
                playerEffects.StartJump(jumpForce);

                // Passa la posizione del player al metodo TriggerJumpEffect
                TriggerJumpEffect(pv.transform.position);

                StartCooldown();
                StartCoroutine(FadeButton(0f, 1f));
                break;
            }
            }

        }
    }

    void TriggerJumpEffect(Vector3 position)
    {
        if (jumpEffectPrefab != null)
        {
            // Istanza l'effetto di salto con la rotazione e scala specificate
            GameObject effect = Instantiate(jumpEffectPrefab, position, Quaternion.Euler(jumpEffectRotation));
            effect.transform.localScale = jumpEffectScale;
        }
        else
        {
            Debug.LogError("Jump effect prefab is not assigned!");
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
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / cooldownDuration);
            graphic.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        graphic.color = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);
    }
}
