using UnityEngine;

public class CameraAnimationLobby : MonoBehaviour
{
    public Animator cameraAnimator;

    private void Start()
    {
    GoToNextStopPoint();
    }

    // Metodo per far avanzare l'animazione al prossimo punto di arresto
    public void GoToNextStopPoint()
    {
        cameraAnimator.speed = 1; // Inizia o continua l'animazione
    }

    // Metodo chiamato dall'Animation Event per fermare l'animazione
    public void StopAnimationAtPoint()
    {
        cameraAnimator.speed = 0; // Ferma l'animazione
    }
}
