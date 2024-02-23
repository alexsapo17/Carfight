using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public float referenceAspectRatio = 16f /9f; // Inserisci qui il rapporto di aspetto di riferimento (es: 16:9)
    public float referenceDistance = 10f; // Distanza della camera per il rapporto di aspetto di riferimento
    public float maxDistance = 15f; // Massima distanza consentita per la camera
    public float minDistance = 5f; // Minima distanza consentita per la camera

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        AdjustCameraPosition();
    }

    void AdjustCameraPosition()
    {
        // Calcola il rapporto di aspetto corrente
        float currentAspectRatio = (float)Screen.width / Screen.height;

        // Calcola un fattore di scala basato sul confronto dei rapporti di aspetto
        float distanceFactor = referenceAspectRatio / currentAspectRatio;

        // Imposta la distanza della camera usando il fattore di scala, mantenendola tra minDistance e maxDistance
        float distance = Mathf.Clamp(referenceDistance * distanceFactor, minDistance, maxDistance);

        // Aggiorna la posizione della camera mantenendo la stessa direzione
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, -distance);
    }
}
