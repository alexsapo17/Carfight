using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Il target che la camera deve seguire
    public Vector3 offset = new Vector3(0, 5, -10); // Offset della camera rispetto al target
    public float smoothTime = 0.3f; // Tempo per il movimento fluido della camera

    private Vector3 initialPosition; // Posizione iniziale della camera
    private Vector3 velocity = Vector3.zero; // Velocità iniziale, utilizzata dalla funzione SmoothDamp
    private bool startFollow = false; // Quando true, inizia a seguire il target

    // Metodo per impostare il target della camera.
    public void SetTarget(GameObject target)
    {
        if (target != null)
        {
            this.target = target.transform;
            initialPosition = transform.position; // Imposta la posizione iniziale della camera
            startFollow = true; // Attiva il seguito
        }
    }

    void LateUpdate()
    {
        if (target != null && startFollow)
        {
            // Calcola la posizione desiderata della camera basandoti sulla rotazione del target
            Vector3 desiredPosition = target.position - target.forward * offset.z + target.up * offset.y;

            // Interpola la posizione della camera dalla posizione iniziale alla posizione desiderata
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            // Imposta la rotazione della camera per guardare il target
            transform.LookAt(target.position);
        }
    }

    // Puoi chiamare questo metodo per resettare la posizione iniziale della camera se necessario
    public void ResetInitialPosition(Vector3 newPosition)
    {
        initialPosition = newPosition;
    }
}
