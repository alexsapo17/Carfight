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

    void Start()
    {
        originalScale = transform.localScale;
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnButtonClick()
    {
        StartCoroutine(AnimateButton());
    }

    IEnumerator AnimateButton()
    {
        // Ingrandisci
        float time = 0;
        while (time < animationDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, enlargedScale, time / animationDuration);
            time += Time.deltaTime;
            yield return null;
        }



        // Ripristina lo stato originale e attiva l'evento onclick
        transform.localScale = originalScale;
        canvasGroup.alpha = 1;
        onClick?.Invoke();
    }
}
