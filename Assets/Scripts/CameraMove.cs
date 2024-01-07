using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform startPosition; // Assegna un oggetto vuoto nella scena con la posizione iniziale
    public Transform endPosition;   // Assegna la posizione finale della camera
    public float timeToMove = 5.0f; // Tempo in secondi che la camera impiegherà per arrivare alla posizione finale

    private float timeElapsed;

    void Start()
    {
        // Imposta la camera alla posizione iniziale all'avvio
        transform.position = startPosition.position;
        transform.rotation = startPosition.rotation;
    }

    void Update()
    {
        // Interpola la posizione e la rotazione della camera nel tempo
        if (timeElapsed < timeToMove)
        {
            transform.position = Vector3.Lerp(startPosition.position, endPosition.position, timeElapsed / timeToMove);
            transform.rotation = Quaternion.Lerp(startPosition.rotation, endPosition.rotation, timeElapsed / timeToMove);
            timeElapsed += Time.deltaTime;
        }
    }
}
