using UnityEngine;
using UnityEngine.UI;

public class ActivateInvisibilityButton : MonoBehaviour
{
    public InvisibilityEffect invisibilityEffectPrefab; // Imposta questo prefab tramite l'Inspector
    public Button invisibilityButton; // Riferimento al tuo pulsante UI

    private void Start()
    {
        invisibilityButton.onClick.AddListener(ActivateInvisibility);
    }

    void ActivateInvisibility()
    {
        // Ottieni il riferimento a PlayerEffects
        PlayerEffects playerEffects = FindObjectOfType<PlayerEffects>(); // Assicurati che ci sia solo un PlayerEffects nel giocatore locale

        if (playerEffects != null)
        {
            // Crea una nuova istanza del power-up di invisibilità e attivalo
            InvisibilityEffect instance = Instantiate(invisibilityEffectPrefab);
            instance.ActivateInvisibility(playerEffects);
        }
    }
}
