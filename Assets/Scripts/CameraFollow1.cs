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
    private Vector3 preFlyOriginalOffset;

    public float yTouchSensitivity = 0.1f; // Regola la sensibilità del movimento sull'asse Y
    public float zTouchSensitivity = 0.1f; // Regola la sensibilità del movimento sull'asse Z
    public float maxXOffset = 5f; // Limite massimo di spostamento sull'asse X
    public float maxYOffset = 5f; // Limite massimo di spostamento sull'asse Y
    public float maxZOffset = 5f; // Limite massimo di spostamento sull'asse Z
    private float rotationSpeed = 1f; 
        private float touchSpeed = 1f; 
private Vector2 smoothedDeltaTouch = Vector2.zero; // Movimento del dito smussato
private float smoothSpeedFly = 10f;
    private float rotationMultiplier = 0.1f;
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


    Vector2 rawDeltaTouch = touch.deltaPosition;

    // Applica un filtro per ridurre il rumore
    smoothedDeltaTouch = Vector2.Lerp(smoothedDeltaTouch, rawDeltaTouch, smoothSpeedFly * Time.deltaTime);
                Vector2 deltaTouch = touch.position - lastTouchPosition;

    if (!isAnyWheelTouching)
    {
        // Calcola la velocità del movimento del dito
        float touchSpeed = smoothedDeltaTouch.magnitude / touch.deltaTime;

        // Calcola la rotazione desiderata della telecamera basata sulla velocità del dito
        float rotationAmountX = smoothedDeltaTouch.x * rotationSpeed * rotationMultiplier * touchSpeed * Time.deltaTime;
        float rotationAmountY = -smoothedDeltaTouch.y * rotationSpeed * rotationMultiplier * touchSpeed * Time.deltaTime; // Aggiungi la rotazione anche sull'asse Y

        // Ruota attorno al player su entrambi gli assi
        transform.RotateAround(target.position, Vector3.up, rotationAmountX);

        // Calcola la nuova posizione della telecamera se la differenza sull'asse Y rientra nel range desiderato
        float newYPosition = transform.position.y + rotationAmountY;
        float clampedYPosition = Mathf.Clamp(newYPosition, target.position.y - 20f, target.position.y + 30f);

        // Applica la nuova posizione solo se rientra nel range
        if (clampedYPosition >= target.position.y - 20f && clampedYPosition <= target.position.y + 30f)
        {
            transform.position = new Vector3(transform.position.x, clampedYPosition, transform.position.z);
                    // Controllo della distanza tra la telecamera e il target
        Vector3 directionToTarget = transform.position - target.position;
        if (directionToTarget.magnitude > 20f)
        {
            // Limita la distanza tra la telecamera e il target a 40f
            transform.position = target.position + directionToTarget.normalized * 40f;
        }
        }

        // Aggiorna l'offset originale della telecamera
        originalOffset = transform.position - target.position;
    }

                if (isAnyWheelTouching)
                        {
                            // Calcola lo spostamento della posizione della telecamera
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
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {

                                isDragging = false;
                                dragFingerId = -1;
                                originalOffset= preFlyOriginalOffset;
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
        if (target != null)
    {
        
    

           if (!isAnyWheelTouching )
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
preFlyOriginalOffset = originalOffset;
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

 
void LateUpdate()
{

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
    }

    return results.Count > 0;
}

}
