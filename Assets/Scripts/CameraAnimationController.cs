using UnityEngine;

public class CameraAnimationController : MonoBehaviour
{
    public Animator cameraAnimator;

    private void Start()
    {
        // Imposta l'animazione al punto di partenza e fermala
        cameraAnimator.Play("CameraShopAnimation", 0, 0f);
        cameraAnimator.speed = 0;
    }

    // Metodo chiamato quando si preme il pulsante per avviare o proseguire l'animazione
    public void PlayOrContinueAnimation()
    {
        cameraAnimator.speed = 1; // Inizia o continua l'animazione
    }

    // Questo metodo verrà chiamato dall'Animation Event per fermare l'animazione
    public void StopAnimation()
    {
        cameraAnimator.speed = 0; // Ferma l'animazione
    }
}
