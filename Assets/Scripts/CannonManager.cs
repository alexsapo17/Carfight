using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CannonData
{
    public Cannon cannon;
    public float minInterval = 3f;
    public float maxInterval = 7f;
    private float nextFireTime;

    public float GetNextFireTime()
    {
        return nextFireTime;
    }

    public void SetNextFireTime(float currentTime)
    {
        nextFireTime = currentTime + Random.Range(minInterval, maxInterval);
    }
}

public class CannonManager : MonoBehaviour
{
    public List<CannonData> cannons = new List<CannonData>();
    private bool isFiring = true;

    private void Start()
    {
        // Inizializza i tempi di fuoco iniziali per tutti i cannoni
        float currentTime = Time.time;
        foreach (var cannonData in cannons)
        {
            cannonData.SetNextFireTime(currentTime);
        }
    }

    private void Update()
    {
        if (!isFiring)
            return;

        // Controlla se è il momento di sparare per ciascun cannone
        float currentTime = Time.time;
        foreach (var cannonData in cannons)
        {
            if (currentTime >= cannonData.GetNextFireTime())
            {
                // Chiama il metodo FireCannon dello script Cannon del cannone
                cannonData.cannon.FireCannon();

                // Attiva il trigger "Shoot" dell'animatore del cannone
                Animator cannonAnimator = cannonData.cannon.GetComponent<Animator>();
                if (cannonAnimator != null)
                {
                    cannonAnimator.SetTrigger("Shoot");
                }

                // Imposta il prossimo tempo di fuoco per il cannone
                cannonData.SetNextFireTime(currentTime);
            }
        }
    }

    // Metodo per fermare gli spari dei cannoni
    public void StopCannonFire()
    {
        isFiring = false;
    }
}
