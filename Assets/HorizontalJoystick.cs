 using UnityEngine;
using UnityEngine.EventSystems;

public class HorizontalJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public RectTransform handle;
    private float handleRange = 100f; // Range di movimento del joystick
    private float horizontalValue;

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = eventData.position - (Vector2)transform.position;
        newPosition = Vector2.ClampMagnitude(newPosition, handleRange);
        handle.anchoredPosition = new Vector2(newPosition.x, 0);

        // Calcola il valore orizzontale basato sulla posizione del joystick
        horizontalValue = newPosition.x / handleRange;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Riposiziona il joystick al centro quando rilasciato
        handle.anchoredPosition = Vector2.zero;
        horizontalValue = 0;
    }

    // Metodo per ottenere il valore orizzontale
    public float GetHorizontal()
    {
        return horizontalValue;
    }
}