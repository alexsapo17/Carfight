using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshRenderer))]
public class MeshRendererAnimation : MonoBehaviour
{
    public enum AnimationMode
    {
        Loop,
        PingPong,
        OnceForward,
        OnceBackward
    }

    public enum ColorAnimationMode 
    {
        None, // Nessuna animazione del colore
        OnlyColor, // Solo animazione del colore
        WithOtherAnimations // Animazione del colore insieme alle altre animazioni
    }

    private MeshRenderer meshRendererComponent;
    public Color baseColor = Color.white;
    public Color radiusAnimationColor = Color.white; // Colore dell'animazione del radius selezionabile dall'Inspector


    public Color transitionColor = Color.red; // Colore di transizione selezionabile dall'Inspector
    public float colorTransitionDuration = 2.0f; // Durata della transizione del colore
    public float radius = 0.5f;
    public float animationSpeed = 1.0f;
    public AnimationMode animationMode = AnimationMode.Loop;
    public ColorAnimationMode colorAnimationMode = ColorAnimationMode.None; // Modalità di animazione del colore

    private bool isAnimating = false;
    private float targetRadius = 0.6f;
    private bool animationForward = true;
    private int pingPongCount = 0;
    private Color originalColor;
    private bool isColorTransitioning = false;
    private float colorTransitionProgress = 0.0f;

    void Start()
    {
        meshRendererComponent = GetComponent<MeshRenderer>();
        originalColor = baseColor; // Salva il colore originale all'avvio
        ApplyInitialSettings();
    }


    void Update()
    {
        if (isAnimating)
        {
            AnimateRadius();
        }

        if (isColorTransitioning)
        {
            AnimateColor();
        }
    }

    public void ToggleAnimation()
    {
        if (colorAnimationMode != ColorAnimationMode.OnlyColor)
        {
            isAnimating = !isAnimating;
        }

        if (isAnimating || colorAnimationMode == ColorAnimationMode.OnlyColor)
        {
            pingPongCount = 0;
            if (colorAnimationMode != ColorAnimationMode.None)
            {
                StartColorTransition(); // Avvia la transizione del colore se necessario
            }

            if (animationMode == AnimationMode.OnceForward || animationMode == AnimationMode.OnceBackward)
            {
                radius = (animationMode == AnimationMode.OnceForward) ? -0.1f : 1.0f;
                animationForward = (animationMode == AnimationMode.OnceForward);
            }
        }
    }

     private void AnimateRadius()
    {
        float direction = animationForward ? 1.0f : -1.0f;
        radius += direction * animationSpeed * Time.deltaTime;

        // Applica il colore dell'animazione del radius qui
        meshRendererComponent.material.SetColor("_Color", radiusAnimationColor); // Assicurati che "_Color" sia il nome corretto usato nel tuo shader

        if ((animationForward && radius >= targetRadius) || (!animationForward && radius <= -0.1f))
        {
            switch (animationMode)
            {
                case AnimationMode.Loop:
                    radius = animationForward ? -0.1f : 1.0f;
                    break;
                case AnimationMode.PingPong:
                    animationForward = !animationForward; // Cambia direzione
                    pingPongCount++; // Incrementa il contatore ogni volta che cambia direzione
                    if (pingPongCount == 2) // Termina dopo che il cerchio è tornato indietro
                    {
                        isAnimating = false;
                    }
                    break;
                case AnimationMode.OnceForward:
                case AnimationMode.OnceBackward:
                    isAnimating = false; // Ferma l'animazione
                    break;
            }
        }

        radius = Mathf.Clamp(radius, -0.1f, 1.0f);
        meshRendererComponent.material.SetFloat("_Radius", radius);
    }

  public void StartColorTransition()
    {
        isColorTransitioning = true;
        colorTransitionProgress = 0.0f;
    }

    private void AnimateColor()
    { 
        colorTransitionProgress += Time.deltaTime / colorTransitionDuration;
        if (colorTransitionProgress < 1.0f)
        {
            // Transizione dal colore originale al colore di transizione
            baseColor = Color.Lerp(originalColor, transitionColor, colorTransitionProgress);
        }
        else if (colorTransitionProgress < 2.0f)
        {
            // Transizione dal colore di transizione al colore originale
            baseColor = Color.Lerp(transitionColor, originalColor, colorTransitionProgress - 1.0f);
        }
        else
        {
            isColorTransitioning = false; // Ferma la transizione del colore
        }

        meshRendererComponent.material.SetColor("_BaseColor", baseColor);
    }

    private void ApplyInitialSettings()
    {
        meshRendererComponent.material.SetColor("_BaseColor", baseColor);
        meshRendererComponent.material.SetFloat("_Radius", radius);
    }

    // Aggiungi qui ulteriori metodi per cambiare animazione, colore, ecc...
}
