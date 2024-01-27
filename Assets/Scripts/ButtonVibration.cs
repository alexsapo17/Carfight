using UnityEngine;
using UnityEngine.UI;
using UnityCoreHaptics;

[RequireComponent(typeof(Button))]
public class ButtonVibration : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Vibrate);
    }

    private void Vibrate()
    {
        if (UnityCoreHapticsProxy.SupportsCoreHaptics())
        {
            // Riproduci una vibrazione leggera e breve
            UnityCoreHapticsProxy.PlayTransientHaptics(1f, 1f); // Intensità e nitidezza massime
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(Vibrate);
        }
    }
}
