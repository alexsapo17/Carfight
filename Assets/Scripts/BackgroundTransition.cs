using UnityEngine;
using UnityEngine.UI;

public class BackgroundTransition : MonoBehaviour
{
    public Image backgroundImage; // Assegna l'immagine di sfondo qui
    public Color transitionColor = Color.white; // Colore della transizione selezionabile dall'Inspector
    public float transitionDuration = 2.0f; // Durata della transizione
    public float returnDuration = 2.0f; // Durata della transizione di ritorno al colore originale

    private bool isTransitioning = false;
    private float transitionProgress = 0.0f;
    private Color originalColor; // Per salvare il colore originale dell'immagine di sfondo

    void Start()
    {
        originalColor = backgroundImage.color; // Salva il colore originale all'avvio
    }

    // Metodo da chiamare per iniziare la transizione
    public void StartTransition()
    {
        if (!isTransitioning)
        {
            isTransitioning = true;
            transitionProgress = 0.0f;
            originalColor = backgroundImage.color; // Assicurati di salvare il colore originale prima di iniziare la transizione
        }
    }

    void Update()
    {
        if (isTransitioning)
        {
            if (transitionProgress < 1.0f)
            {
                // Transizione verso il nuovo colore
                transitionProgress += Time.deltaTime / transitionDuration;
                backgroundImage.color = Color.Lerp(originalColor, transitionColor, transitionProgress);
            }
            else
            {
                // Inizia la transizione di ritorno al colore originale
                transitionProgress += Time.deltaTime / returnDuration;
                backgroundImage.color = Color.Lerp(transitionColor, originalColor, transitionProgress - 1.0f);

                // Quando anche la transizione di ritorno è completata
                if (transitionProgress >= 2.0f)
                {
                    isTransitioning = false;
                }
            }
        }
    }
}
