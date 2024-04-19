using UnityEngine;
using System.Collections; // Necessario per usare StartCoroutine

public class EnemyCollision : MonoBehaviour
{
    // Cambia questa variabile per utilizzare un GameObject direttamente dall'Inspector
    public GameObject prefab; // Ora puoi trascinare il tuo prefab direttamente qui dall'Editor di Unity
    public AudioSource audioSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Istanza il prefab al punto di collisione
            // Non c'è più bisogno di caricarlo, dato che ora lo assegni dall'Inspector
            if (prefab != null)
            {
                Instantiate(prefab, collision.contacts[0].point, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Prefab non assegnato nell'Inspector.");
            }
               // Ottieni il componente AudioSource
        audioSource = GetComponent<AudioSource>();

        // Se l'AudioSource non è stato trovato, stampa un messaggio di avviso
        if (audioSource == null)
        {
            Debug.LogWarning("AudioSource non trovato sull'oggetto " + gameObject.name);
        }
        // Attiva l'AudioSource se presente
        if (audioSource != null)
        {
            audioSource.Play();
        }

            StartCoroutine(SlowMotionEffect());
        }
    }

    private IEnumerator SlowMotionEffect()
    {
        Time.timeScale = 0.2f; // Imposta lo slow-motion
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Mantiene la fisica fluida durante lo slow-motion

        yield return new WaitForSecondsRealtime(3f); // Aspetta 1 secondo in tempo reale

        Time.timeScale = 1f; // Ripristina la velocità normale del gioco
        Time.fixedDeltaTime = 0.02f; // Ripristina il valore predefinito di fixedDeltaTime

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.FinishSurvivalGame();
        }
        else
        {
            Debug.LogError("LevelManager non trovato nella scena.");
        }
    }
}
