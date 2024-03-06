using UnityEngine;
using UnityEngine.EventSystems; // Necessario per interagire con EventTrigger
using System.Collections.Generic;
using System.Linq;
public class CameraFollow : MonoBehaviour
{
    public Vector3 offset = new Vector3(0, 5, -10);
    public float fixedYPosition = 10.0f;
    public float smoothSpeed = 5.0f;
        public float smoothSpeedY = 0.5f; // Velocità di interpolazione per l'asse Y, molto più lenta

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
    // Rimozione del codice relativo a InputPanel
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
        -deltaTouch.x * touchSensitivity,
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

void LateUpdate() {
    if (target != null) {
        // Utilizza il metodo originale per calcolare la posizione desiderata, inclusa la rotazione
        Vector3 offsetRotated = target.TransformDirection(offset);
        Vector3 desiredPosition = target.position + offsetRotated;

        // Ora, sovrascrivi solamente la componente Y della posizione desiderata con un calcolo che ignora la rotazione verticale del target
        desiredPosition.y = Mathf.Lerp(transform.position.y, target.position.y + offset.y, smoothSpeedY * Time.deltaTime);

        // Applica un Lerp alle componenti X e Z per un movimento liscio, mantenendo la nuova componente Y calcolata sopra
        float smoothedPositionX = Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeed * Time.deltaTime);
        float smoothedPositionZ = Mathf.Lerp(transform.position.z, desiredPosition.z, smoothSpeed * Time.deltaTime);

        // Applica la posizione interpolata alla camera
        transform.position = new Vector3(smoothedPositionX, desiredPosition.y, smoothedPositionZ);

        // Mantieni la camera rivolta verso il target, utilizzando la rotazione originale
        transform.LookAt(target.position + Vector3.up * offset.y);

        // Applica l'inclinazione sull'asse X aggiuntiva, se desiderata
        transform.rotation = Quaternion.Euler(tiltAngleX + transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
    }
}



 private bool IsPointerOverUIObject(Vector2 touchPos)
{
    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    eventDataCurrentPosition.position = touchPos;
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    

    
    return results.Count > 0;
}


}
