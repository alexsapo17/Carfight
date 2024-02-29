using UnityEngine;
using UnityEngine.EventSystems; // Necessario per interagire con EventTrigger
using System.Collections.Generic;
using System.Linq;
public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 5, -10);
    public float fixedYPosition = 10.0f;
    public float smoothSpeed = 5.0f;
    public float tiltAngleX = 20.0f;
public float touchSensitivity = 0.5f; // Regola questa variabile per controllare la sensibilità del trascinamento

    public float yTouchSensitivity = 0.1f; // Regola la sensibilità del movimento sull'asse Y
public float zTouchSensitivity = 0.1f; // Regola la sensibilità del movimento sull'asse Z
public float maxXOffset = 5f; // Limite massimo di spostamento sull'asse X
public float maxYOffset = 5f; // Limite massimo di spostamento sull'asse Y
public float maxZOffset = 5f; // Limite massimo di spostamento sull'asse Z

    private Vector3 initialOffset;
    private Transform target;
    private bool isDragging = false;
    private Vector2 lastTouchPosition; // Aggiunto per tenere traccia dell'ultima posizione del touch
    private int dragFingerId = -1; // Aggiungi questa variabile per tenere traccia dell'ID del tocco

    void Start()
    {
        initialOffset = offset;
        GameObject inputPanel = GameObject.Find("InputPanel");
        if (inputPanel != null)
        {
            EventTrigger trigger = inputPanel.GetComponent<EventTrigger>();
            if (trigger != null)
            {
                EventTrigger.Entry entryDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
                entryDown.callback.AddListener((data) => { OnPointerDown((PointerEventData)data); });
                trigger.triggers.Add(entryDown);

                EventTrigger.Entry entryUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
                entryUp.callback.AddListener((data) => { OnPointerUp((PointerEventData)data); });
                trigger.triggers.Add(entryUp);
            }
        }
    }

    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            this.target = target.transform;
        }
    }

    private void OnPointerDown(PointerEventData data)
{
    // Verifica se il tocco non è sopra un elemento UI utilizzando la posizione dell'evento
    if (!IsPointerOverUIObject(data.position))
    {
        isDragging = true;
        // Imposta l'ultima posizione del touch basandosi sulla posizione dell'evento
        lastTouchPosition = data.position;
    }
}


    private void OnPointerUp(PointerEventData data)
    {
        isDragging = false;
    }

void Update()
{
    if (Input.touchCount > 0)
    {
        if (!isDragging)
        {
            // Cerca un tocco che inizi al di fuori degli elementi UI per iniziare il trascinamento
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began && !IsPointerOverUIObject(touch.position))
                {
                    dragFingerId = touch.fingerId;
                    lastTouchPosition = touch.position;
                    isDragging = true;
                    break; // Interrompi il ciclo una volta trovato un tocco valido
                }
            }
        }
        else
        {
            // Gestisci il tocco che sta trascinando la camera
            Touch touch = Input.touches.FirstOrDefault(t => t.fingerId == dragFingerId);
if (touch.phase == TouchPhase.Moved)
{
    Vector2 deltaTouch = touch.position - lastTouchPosition;
    lastTouchPosition = touch.position;

    // Assicurati che il movimento sia proporzionale e coerente in entrambe le direzioni
    Vector3 touchMovement = new Vector3(
        deltaTouch.x * touchSensitivity,
        -Mathf.Abs(deltaTouch.x) * yTouchSensitivity, // Usa Mathf.Abs per mantenere la direzione
        -Mathf.Abs(deltaTouch.x) * zTouchSensitivity
    );

    // Calcola il nuovo offset considerando i limiti
    Vector3 potentialOffset = offset + touchMovement * Time.deltaTime;
    offset = new Vector3(
        Mathf.Clamp(potentialOffset.x, initialOffset.x - maxXOffset, initialOffset.x + maxXOffset),
        Mathf.Clamp(potentialOffset.y, initialOffset.y - maxYOffset, initialOffset.y + maxYOffset),
        Mathf.Clamp(potentialOffset.z, initialOffset.z - maxZOffset, initialOffset.z + maxZOffset)
    );
}



            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
                dragFingerId = -1;
            }
        }
    }
    else
    {
        isDragging = false;
        dragFingerId = -1;
    }

    if (!isDragging)
    {
        offset = Vector3.Lerp(offset, initialOffset, smoothSpeed * Time.deltaTime);
    }
}

 
    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 offsetRotated = target.TransformDirection(offset);
            Vector3 desiredPosition = target.position + offsetRotated;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, desiredPosition.y, desiredPosition.z), smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
            transform.LookAt(target);
            Vector3 currentRotation = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(currentRotation.x + tiltAngleX, currentRotation.y, currentRotation.z);
        }
    }
   private bool IsPointerOverUIObject(Vector2 touchPos)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(touchPos.x, touchPos.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}
