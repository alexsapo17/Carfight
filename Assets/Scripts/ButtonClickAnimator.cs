using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;

public class ButtonClickAnimator : MonoBehaviour
{
    public float animationDuration = 0.5f;
    public Vector3 enlargedScale = new Vector3(1.2f, 1.2f, 1.2f);
    public UnityEvent onClick;

    private Vector3 originalScale;
    private CanvasGroup canvasGroup;
    private Animator animator; // Variabile per memorizzare il riferimento all'Animator

    void Start()
    {
        originalScale = transform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>(); // Ottieni il riferimento all'Animator

        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnButtonClick()
    {
        StartCoroutine(AnimateButton());
    }

    IEnumerator AnimateButton()
    {
        // Disattiva l'Animator se è presente
        if (animator != null)
        {
            animator.enabled = false;
        }

        float time = 0;
        Time.timeScale = 1;
        while (time < animationDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, enlargedScale, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }

        // Ripristina lo stato originale e attiva l'evento onClick
        transform.localScale = originalScale;
        canvasGroup.alpha = 1;
        
        if (onClick != null) 
        {
            onClick.Invoke();
        }
        else
        {
            Debug.Log("Attenzione: onClick è null.");
        }

        // Riattiva l'Animator dopo l'animazione, se necessario
        if (animator != null)
        {
            animator.enabled = true;
        }
    }
}
