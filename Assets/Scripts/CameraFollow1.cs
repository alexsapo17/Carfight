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
private Vector3 originalOffset;
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
 private WheelCollider[] targetWheelColliders;

    void Start()
    {
        initialOffset = offset;
    }

    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            this.target = target.transform;
            targetWheelColliders = target.GetComponentsInChildren<WheelCollider>();
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
        for (int i = 0; i < Input.touchCount; ++i)
        {
            Touch touch = Input.GetTouch(i);
            Vector2 touchPosition = touch.position;

            if (!IsPointerOverUIObject(touchPosition))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (!isDragging)
                    {
                        // Inizia a trascinare solo se non si sta già trascinando con un altro dito
                        isDragging = true;
                        dragFingerId = touch.fingerId;
                        lastTouchPosition = touch.position;
                    }
                }
                else if (touch.fingerId == dragFingerId)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        Vector2 deltaTouch = touch.position - lastTouchPosition;
                        Vector3 touchMovement = new Vector3(
                            -deltaTouch.x * touchSensitivity,
                            -Mathf.Abs(deltaTouch.x) * yTouchSensitivity,
                            -Mathf.Abs(deltaTouch.x) * zTouchSensitivity);
                        
                        Vector3 potentialOffset = offset + touchMovement * Time.deltaTime;
                        offset = new Vector3(
                            Mathf.Clamp(potentialOffset.x, initialOffset.x - maxXOffset, initialOffset.x + maxXOffset),
                            Mathf.Clamp(potentialOffset.y, initialOffset.y - maxYOffset, initialOffset.y + maxYOffset),
                            Mathf.Clamp(potentialOffset.z, initialOffset.z - maxZOffset, initialOffset.z + maxZOffset));
                        
                        lastTouchPosition = touch.position;
                    }
                    else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        isDragging = false;
                        dragFingerId = -1;
                    }
                }
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
        // Verifica se almeno un WheelCollider è a contatto con qualcosa
        bool isAnyWheelTouching = false;
        foreach (WheelCollider wheelCollider in targetWheelColliders)
        {
            if (wheelCollider.isGrounded)
            {
                isAnyWheelTouching = true;
                break;
            }
        }

           if (!isAnyWheelTouching)
        {

            // Calcola la posizione desiderata della camera mantenendo l'offset
            Vector3 desiredPositionNoGround = target.position + originalOffset;

            // Applica la posizione interpolata alla camera senza modificare la rotazione
            transform.position = Vector3.Lerp(transform.position, desiredPositionNoGround, smoothSpeed * Time.deltaTime);

            // Mantieni la camera rivolta verso il target senza modificare la rotazione
            transform.LookAt(target);

            // Applica l'inclinazione sull'asse X aggiuntiva, se desiderato
            Vector3 currentRotationNoGround = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(currentRotationNoGround.x + tiltAngleX, currentRotationNoGround.y, currentRotationNoGround.z);

            return;
        }
    originalOffset = transform.position - target.position;

        // Resto del codice per aggiornare la posizione e la rotazione della camera
        Vector3 offsetRotated = target.TransformDirection(offset);
        Vector3 desiredPosition = target.position + offsetRotated;
        
        // Calcola la posizione interpolata per X e Z usando smoothSpeed
        float smoothedPositionX = Mathf.Lerp(transform.position.x, desiredPosition.x, smoothSpeed * Time.deltaTime);
        float smoothedPositionZ = Mathf.Lerp(transform.position.z, desiredPosition.z, smoothSpeed * Time.deltaTime);

        // Calcola la posizione interpolata per Y usando smoothSpeedY, ma assicurati che non vada sotto fixedYPosition
        float targetYPosition = Mathf.Max(desiredPosition.y, target.position.y + fixedYPosition);
        float smoothedPositionY = Mathf.Lerp(transform.position.y, targetYPosition, smoothSpeedY * Time.deltaTime);

        // Applica la posizione interpolata alla camera
        transform.position = new Vector3(smoothedPositionX, smoothedPositionY, smoothedPositionZ);

        // Mantieni la camera rivolta verso il target
        transform.LookAt(target);

        // Applica l'inclinazione sull'asse X aggiuntiva, se desiderato
        Vector3 currentRotation = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(currentRotation.x + tiltAngleX, currentRotation.y, currentRotation.z);
    }
}



private bool IsPointerOverUIObject(Vector2 touchPos)
{
    // Logica per controllare se il tocco è su un oggetto UI
    PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
    eventDataCurrentPosition.position = touchPos;
    List<RaycastResult> results = new List<RaycastResult>();
    EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
    
    foreach (var result in results)
    {
        Debug.Log("UI Object Hit: " + result.gameObject.name);
    }

    return results.Count > 0;
}

}
