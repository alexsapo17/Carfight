using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Importa il namespace per gli eventi
using UnityCoreHaptics;

[RequireComponent(typeof(Button))]
public class ButtonVibration : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    // Questo metodo viene chiamato quando il pulsante è premuto
    public void OnPointerDown(PointerEventData eventData)
    {
        PlayLightVibration();
    }

    // Questo metodo viene chiamato quando il pulsante è rilasciato
    public void OnPointerUp(PointerEventData eventData)
    {
        PlayIntenseVibration();
    }

    private void PlayLightVibration()
    {
        if (UnityCoreHapticsProxy.SupportsCoreHaptics())
        {
            // Riproduci una vibrazione leggera e breve
            UnityCoreHapticsProxy.PlayTransientHaptics(0.5f, 0.5f); // Intensità e nitidezza leggere
        }
    }

    private void PlayIntenseVibration()
    {
        if (UnityCoreHapticsProxy.SupportsCoreHaptics())
        {
            // Riproduci una vibrazione più intensa e breve
            UnityCoreHapticsProxy.PlayTransientHaptics(1f, 1f); // Intensità e nitidezza massime
        }
    }

    private void OnDestroy()
    {
        // Rimuovi i listener se necessario
    }
}
