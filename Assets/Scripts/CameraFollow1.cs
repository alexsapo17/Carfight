using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Il target che la camera deve seguire
    public Vector3 offset = new Vector3(0, 5, -10); // Offset della camera rispetto al target

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

    // Sposta la camera direttamente alla posizione desiderata
    transform.position = desiredPosition;

    // Imposta la rotazione della camera per guardare il target
    transform.LookAt(target);
}
}
}