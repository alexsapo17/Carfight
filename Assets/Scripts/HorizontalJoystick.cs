using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform handle;
    private float handleRange = 100f; // Range di movimento del joystick
    private float horizontalValue;
    private float verticalValue; // Aggiunto per tracciare il valore verticale

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = eventData.position - (Vector2)transform.position;
        newPosition = Vector2.ClampMagnitude(newPosition, handleRange);
        handle.anchoredPosition = new Vector2(newPosition.x, newPosition.y); // Modificato per tracciare anche il movimento verticale

        // Calcola i valori orizzontali e verticali basati sulla posizione del joystick
        horizontalValue = newPosition.x / handleRange;
        verticalValue = newPosition.y / handleRange; // Aggiunto per calcolare il valore verticale
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Riposiziona il joystick al centro quando rilasciato
        handle.anchoredPosition = Vector2.zero;
        horizontalValue = 0;
        verticalValue = 0; // Aggiunto per resettare il valore verticale
    }

    // Metodo per ottenere il valore orizzontale
    public float GetHorizontal()
    {
        return horizontalValue;
    }

    // Metodo per ottenere il valore verticale - Aggiunto
    public float GetVertical()
    {
        return verticalValue;
    }
}
