using UnityEngine;
using System.Collections;

public class CameraAnimationController : MonoBehaviour
{
    public Animator cameraAnimator;
    public float moveSpeed = 1f; // Velocità di spostamento della camera
    public Vector3 moveOffset = new Vector3(1f, 0f, 0f); // Quanto spostarsi verso destra

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position; // Memorizza la posizione iniziale
        cameraAnimator.Play("CameraShopAnimation", 0, 0f);
        cameraAnimator.speed = 0;
    }

    public void PlayOrContinueAnimation()
    {
        cameraAnimator.enabled = true;
        cameraAnimator.speed = 1;
    }

    public void StopAnimation()
    {
        cameraAnimator.speed = 0;
    }

 IEnumerator MoveCameraRightCoroutine()
{
    Vector3 targetPosition = transform.position + moveOffset;
    while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        yield return null;
    }
}

public void MoveCameraRight()
{
    cameraAnimator.enabled = false; // Disabilita l'Animator
    StartCoroutine(MoveCameraRightCoroutine());
}

IEnumerator MoveCameraBackCoroutine()
{
    while (Vector3.Distance(transform.position, originalPosition) > 0.01f)
    {
        transform.position = Vector3.MoveTowards(transform.position, originalPosition, moveSpeed * Time.deltaTime);
        yield return null;
    }
}

public void MoveCameraBack()
{
    cameraAnimator.enabled = false; // Disabilita l'Animator
    StartCoroutine(MoveCameraBackCoroutine());
}

    // Ripristina la camera alla posizione originale
public void ResetCameraPosition()
{
    MoveCameraBack(); // Usa la coroutine per spostare la camera indietro
}

}
