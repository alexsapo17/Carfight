using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAbilities : MonoBehaviour
{
    public string botName; // Nome del bot per identificare l'abilità da assegnare
    public PlayerEffects playerEffects; // Riferimento allo script dei player effects
    public float activationDistance = 10f; // Distanza per attivare l'abilità
    public float cooldownDuration = 5f; // Durata del cooldown
    public GameObject shockwavePrefab; // Prefab da istanziare quando viene attivata l'abilità

    private bool abilityReady = true; // Flag per controllare se l'abilità è pronta
    private Coroutine cooldownCoroutine; // Coroutine per il cooldown

    void Start()
    {
        // Trova lo script PlayerEffects nell'oggetto del bot
        playerEffects = GetComponent<PlayerEffects>();
    }

    void Update()
    {
        // Controlla se l'abilità è pronta e se un giocatore o nemico è vicino
        if (abilityReady && IsPlayerOrEnemyNearby())
        {
            ActivateAbility();
        }
    }

    bool IsPlayerOrEnemyNearby()
    {
        // Trova tutti i giocatori e nemici vicino al bot
        Collider[] colliders = Physics.OverlapSphere(transform.position, activationDistance);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player") || collider.CompareTag("Enemy"))
            {
                // Controlla se il giocatore o nemico è abbastanza vicino
                if (Vector3.Distance(transform.position, collider.transform.position) <= activationDistance)
                {
                    // Controlla se il collider non è lo stesso oggetto del bot
                    if (collider.gameObject != gameObject)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    void ActivateAbility()
    {
        // Controlla il nome del bot per assegnare l'abilità corrispondente
        switch (botName)
        {
            case "FakePlayerSportRacingCar":
                playerEffects.StartShockwaveEffect(); // Attiva l'abilità dello shockwave
                        // Istanza il prefab dell'abilità
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }
                break;

            case "FakePlayerJeep":
                playerEffects.StartShockwaveEffect(); // Attiva l'abilità dello shockwave
                        // Istanza il prefab dell'abilità
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }
                break;   

            case "FakePlayersportCar":
                playerEffects.StartShockwaveEffect(); // Attiva l'abilità dello shockwave
                        // Istanza il prefab dell'abilità
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }
                break;

            case "FakePlayerCice":
                playerEffects.StartShockwaveEffect(); // Attiva l'abilità dello shockwave
                        // Istanza il prefab dell'abilità
        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
        }
                break;

            case "FakePlayerAmbulance":
                playerEffects.StartFlashbangEffect(); // Attiva l'abilità dello shockwave
                break;

            case "FakePlayerFiretruck":
                playerEffects.StartFlashbangEffect(); // Attiva l'abilità dello shockwave
                break;

        }



        // Avvia il cooldown
        StartCooldown();
    }

    void StartCooldown()
    {
        // Imposta l'abilità come non pronta
        abilityReady = false;

        // Avvia il cooldown
        cooldownCoroutine = StartCoroutine(Cooldown());
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownDuration);

        // Alla fine del cooldown, l'abilità è pronta di nuovo
        abilityReady = true;
    }
}
