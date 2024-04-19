using UnityEngine;

public class ActivateAudioOnEnable : MonoBehaviour
{
    public AudioSource audioSource;
 private bool hasBeenEnabled = false; // Flag per controllare se l'oggetto è stato abilitato almeno una volta

    void Start()
    {
        // Ottieni il componente AudioSource
        audioSource = GetComponent<AudioSource>();

        // Se l'AudioSource non è stato trovato, stampa un messaggio di avviso
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource non trovato sull'oggetto " + gameObject.name);
        }
    }

    // Chiamato quando l'oggetto diventa attivo
    void OnEnable()
    {
        // Verifica se l'oggetto è stato abilitato almeno una volta
        if (hasBeenEnabled)
        {
            // Attiva l'AudioSource se presente
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
        else
        {
            // Imposta il flag a true dopo la prima attivazione
            hasBeenEnabled = true;
        }
    }

    // Chiamato quando l'oggetto diventa inattivo
    void OnDisable()
    {
        // Disattiva l'AudioSource se presente
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}