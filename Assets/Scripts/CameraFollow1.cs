using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Il target che la camera deve seguire
    public Vector3 offset = new Vector3(0, 5, -10); // Offset della camera rispetto al target
    public float fixedYPosition = 10.0f; // Imposta questo valore all'altezza Y desiderata
    public float smoothSpeed = 5.0f; // Regola la velocità di smorzamento
    public float tiltAngleX = 20.0f; // Angolo di inclinazione lungo l'asse X

    // Metodo per impostare il target della camera.
    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            this.target = target.transform;
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 offsetRotated = target.TransformDirection(offset);
            Vector3 desiredPosition = target.position + offsetRotated;

            Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(desiredPosition.x, fixedYPosition, desiredPosition.z), smoothSpeed * Time.deltaTime);

            // Imposta la posizione della camera
            transform.position = smoothedPosition;

            // Imposta la rotazione della camera per guardare il target con l'inclinazione specificata
            transform.LookAt(target);
            
            // Aggiunge un'inclinazione lungo l'asse X alla camera
            Vector3 currentRotation = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(currentRotation.x + tiltAngleX, currentRotation.y, currentRotation.z);
        }
    }
}
