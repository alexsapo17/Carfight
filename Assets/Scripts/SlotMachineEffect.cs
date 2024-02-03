using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachineEffect : MonoBehaviour
{
    public float animationDuration = 1.0f; // Durata dell'animazione in secondi

    // Funzione per avviare l'animazione del testo
    public void AnimateText(int startValue, int endValue)
    {
        StartCoroutine(AnimateTextCoroutine(startValue, endValue));
    }

    IEnumerator AnimateTextCoroutine(int startValue, int endValue)
    {
        Text textComponent = GetComponent<Text>();
        float startTime = Time.time;
        while (Time.time - startTime < animationDuration)
        {
            // Calcola il valore corrente da mostrare
            float t = (Time.time - startTime) / animationDuration;
            int currentValue = (int)Mathf.Lerp(startValue, endValue, t);
            textComponent.text = currentValue.ToString();
            yield return null;
        }
        // Assicurati di impostare il valore finale
        textComponent.text = endValue.ToString();
    }
}
