using UnityEngine;
using System.Collections;

public class TextMeshAnimation : MonoBehaviour
{
    public Color startColor = Color.white;
    public Color endColor = Color.yellow;
    public float scaleMultiplier = 1.2f;
    public float duration = 1f; 
    private TextMesh textMesh;

    void Start()
    {
        // Ottieni il componente TextMesh dall'oggetto corrente
        textMesh = GetComponent<TextMesh>();
        
        // Assicurati che il componente TextMesh sia stato trovato
        if (textMesh == null)
        {
            Debug.LogError("Il componente TextMesh non è stato trovato!");
            enabled = false; // Disabilita questo script
            return;
        }
        
        // Avvia l'animazione in un loop
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        while (true)
        {
            // Animazione da startColor a endColor
            yield return ChangeColor(startColor, endColor, duration);
            
            // Animazione da endColor a startColor
            yield return ChangeColor(endColor, startColor, duration);
        }
    }

    IEnumerator ChangeColor(Color fromColor, Color toColor, float animDuration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < animDuration)
        {
            // Calcola l'interpolazione del colore
            float t = elapsedTime / animDuration;
            Color lerpedColor = Color.Lerp(fromColor, toColor, t);
            
            // Applica il colore al TextMesh
            textMesh.color = lerpedColor;
            
            // Aggiorna il tempo trascorso
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        
        // Assicura che il colore sia esattamente quello finale
        textMesh.color = toColor;
    }
}
