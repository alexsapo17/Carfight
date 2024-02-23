using UnityEngine;

public class ActivateCarImages : MonoBehaviour
{
    public float aspectRatioThreshold = 1.77f; // Imposta la soglia per l'aspect ratio (es. 16:9 aspect ratio = 1.77)
    public GameObject[] buttons;

    void Start()
    {
        // Chiama la funzione di verifica quando lo script inizia
        CheckAndActivateImages();
    }

    void Update()
    {
        // Opzionale: se vuoi controllare continuamente l'aspect ratio dello schermo
        CheckAndActivateImages();
    }

    void CheckAndActivateImages()
    {
        // Calcola l'aspect ratio attuale dello schermo
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Controlla se l'aspect ratio è minore della soglia
        if (currentAspectRatio < aspectRatioThreshold)
        {
            ActivateImages();
        } else {
            foreach (GameObject button in buttons)
        {
            // Cerca un figlio chiamato "carImage"
            Transform carImage = button.transform.Find("carImage");
            
            // Se l'oggetto esiste e è  attivo, disattivalo
            if (carImage != null && carImage.gameObject.activeSelf)
            {
                carImage.gameObject.SetActive(false);
            }
        }
    }
    }

    void ActivateImages()
    {
        foreach (GameObject button in buttons)
        {
            // Cerca un figlio chiamato "carImage"
            Transform carImage = button.transform.Find("carImage");
            
            // Se l'oggetto esiste e non è già attivo, attivalo
            if (carImage != null && !carImage.gameObject.activeSelf)
            {
                carImage.gameObject.SetActive(true);
            }
        }
    }
}
