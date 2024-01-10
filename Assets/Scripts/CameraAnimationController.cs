using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
    public Animator cameraAnimator;

    private float halfWayPoint = 0.5f; // Normalized time per la metà dell'animazione

    // Assumi che l'animazione sia ferma all'inizio
    private bool isAtStart = true;

    private void Start()
    {
        // Imposta l'animazione al punto di partenza e fermala
        cameraAnimator.Play("CameraShopAnimation", 0, 0f);
        cameraAnimator.speed = 0;
    }

    // Metodo chiamato quando si preme il pulsante "Avanti"
    public void PlayForward()
    {
        if (isAtStart)
        {
            cameraAnimator.speed = 1; // Inizia l'animazione
            cameraAnimator.Play("CameraShopAnimation", 0, 0f);
            isAtStart = false;
        }
    }

    // Metodo chiamato quando si preme il pulsante "Indietro"
    public void PlayBackward()
    {
        if (!isAtStart)
        {
            cameraAnimator.speed = 1; // Inizia l'animazione
            cameraAnimator.Play("CameraShopAnimation", 0, halfWayPoint);
            isAtStart = true;
        }
    }

    // Questo metodo verrà chiamato dall'Animation Event
    public void StopAnimation()
    {
        cameraAnimator.speed = 0; // Ferma l'animazione
    }
}
