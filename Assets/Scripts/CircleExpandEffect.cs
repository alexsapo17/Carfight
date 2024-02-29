using UnityEngine;
using UnityEngine.UI;

public class CircleExpandEffect : MonoBehaviour
{
    public Material circleMaterial; // Assegna il materiale dello shader dallo Inspector
    public Image targetImage; // Assegna l'Image dallo Inspector
    public Color baseColor = Color.blue; // Il colore base che vuoi utilizzare
    public float animationSpeed = 1.0f; // Velocità dell'animazione

    private bool isAnimating = false;
    private float radius = 0.0f;

    void Update()
    {
        if (isAnimating)
        {
            // Aumenta il raggio per creare l'effetto di espansione
            radius += Time.deltaTime * animationSpeed;
            if (radius > 1.0f) // Quando raggiunge il limite, resetta l'animazione
            {
                radius = 0.0f;
                isAnimating = false;
            }
            // Aggiorna il raggio nel materiale dello shader
            circleMaterial.SetFloat("_Radius", radius);
        }
    }

    public void StartCircleAnimation()
    {
        radius = 0.0f; // Resetta il raggio per riavviare l'animazione
        isAnimating = true;
        // Imposta il colore base dello shader
        circleMaterial.SetColor("_Color", baseColor);
        // Assicurati che l'Image utilizzi il materiale dello shader
        if (targetImage != null)
        {
            targetImage.material = circleMaterial; 
        }
    }
}
