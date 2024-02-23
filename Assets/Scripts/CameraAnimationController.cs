using UnityEngine;
using System.Collections;

public class CameraAnimationController : MonoBehaviour
{
    public Animator cameraAnimator;
    public float moveSpeed = 1f; // Velocità di spostamento della camera
    public Vector3 moveOffset = new Vector3(1f, 0f, 0f); // Quanto spostarsi verso destra
    public float aspectRatioThreshold = 1.5f; // Soglia del rapporto di aspetto (ad esempio 1.5 per 3:2)
    public float additionalBackOffset = 5.0f; // Quanto spostarsi indietro sull'asse Z se la soglia è superata

    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.position; // Memorizza la posizione iniziale

        cameraAnimator.speed = 0;
    }

     public void PlayCameraShopAnimation()
    {
        cameraAnimator.enabled = true;
        cameraAnimator.Play("CameraShopAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void PlayCarShopAnimationBack()
    {
        cameraAnimator.Play("CameraShopAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void PlayRaceCarAnimation()
    {
        cameraAnimator.Play("RaceCarAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void PlayRaceCarBackAnimation()
    {
        cameraAnimator.Play("RaceCarBackAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }
public void PlayPrometheusAnimation()
    {
        cameraAnimator.Play("prometheusAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void PlayPrometheusBackAnimation()
    {
        cameraAnimator.Play("prometheusAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
    public void Monstertruck()
    {
        cameraAnimator.Play("monstertruckAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void MonstertruckBack()
    {
        cameraAnimator.Play("monstertruckAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
        public void sportCar()
    {
        cameraAnimator.Play("sportCarAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void sportCarBack()
    {
        cameraAnimator.Play("sportCarAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
            public void MicraRiccio()
    {
        cameraAnimator.Play("MicraRiccioAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void MicraRiccioBack()
    {
        cameraAnimator.Play("MicraRiccioAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
               public void Ambulance()
    {
        cameraAnimator.Play("AmbulanceAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void AmbulanceBack()
    {
        cameraAnimator.Play("AmbulanceAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
               public void PoliceMonsterTruck()
    {
        cameraAnimator.Play("PoliceMonsterTruckAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void PoliceMonstreTruckBack()
    {
        cameraAnimator.Play("PoliceMonsterTruckAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
                   public void SportRacingCar()
    {
        cameraAnimator.Play("SportRacingCarAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void SportRacingCarBack()
    {
        cameraAnimator.Play("SportRacingCarAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }

              public void Cice()
    {
        cameraAnimator.Play("CiceAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void CiceBack()
    {
        cameraAnimator.Play("CiceAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
                public void Bus()
    {
        cameraAnimator.Play("BusAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void BusBack()
    {
        cameraAnimator.Play("BusAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
                   public void Ruby()
    {
        cameraAnimator.Play("RubyAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void RubyBack()
    {
        cameraAnimator.Play("RubyAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
                     public void Jeep()
    {
        cameraAnimator.Play("JeepAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void JeepBack()
    {
        cameraAnimator.Play("JeepAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }
                         public void Firetruck()
    {
        cameraAnimator.Play("FiretruckAnimation", 0, 0f);
        cameraAnimator.speed = 1;
    }

    public void FiretruckBack()
    {
        cameraAnimator.Play("FiretruckAnimationBack", 0, 0f);
        cameraAnimator.speed = 1;
    }


public void MoveCameraRight()
{
    cameraAnimator.enabled = false; // Disabilita l'Animator
    StartCoroutine(MoveCameraRightCoroutine());
}

 IEnumerator MoveCameraRightCoroutine()
    {
        Vector3 targetPosition = transform.position + moveOffset;

        // Calcola l'offset in base al rapporto di aspetto
        if ((float)Screen.width / Screen.height < aspectRatioThreshold)
        {
            targetPosition.z -= additionalBackOffset; // Sposta la camera più indietro sull'asse Z
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator MoveCameraBackCoroutine()
    {
        Vector3 targetPosition = originalPosition;

        // Calcola l'offset in base al rapporto di aspetto
        if ((float)Screen.width / Screen.height < aspectRatioThreshold)
        {
            targetPosition.z -= additionalBackOffset; // Sposta la camera più indietro sull'asse Z
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
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
