using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UpdateImageColor : MonoBehaviour
{
    public string sceneName;
    private Image buttonImage; // Variabile per il componente Image

    void Start()
    {
        // Cerca l'immagine con nome specifico tra i figli
        Image[] images = GetComponentsInChildren<Image>();
        buttonImage = images.FirstOrDefault(img => img.gameObject.name == "Icon");

        if (buttonImage != null) // Verifica che l'immagine sia stata trovata
        {
            UpdateColor();
        }
        else
        {
            Debug.LogWarning("Image 'Icon' not found!");
        }
    }

    void UpdateColor()
    {
        Button button = GetComponent<Button>(); // Ottiene il riferimento al componente Button

        if (SceneManager.GetActiveScene().name == sceneName)
        {
            buttonImage.color = Color.green; // Cambia il colore dell'immagine
            button.interactable = false; // Disabilita il pulsante
        }
        else
        {
            buttonImage.color = Color.white; // Ripristina il colore originale dell'immagine
            button.interactable = true; // Abilita il pulsante
        }
    }
}
