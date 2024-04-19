using UnityEngine;
using UnityEngine.SceneManagement;

public class GemPickup : MonoBehaviour
{
    public int value = 5; // Valore della gemma
    public GameObject pickupEffect; // Assegna il prefab del sistema di particelle nell'Editor

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
                        // Disattiva tutti i figli del gameobject
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
             // Disattiva il renderer e il collider
            Renderer renderer = GetComponent<Renderer>();
            Collider collider = GetComponent<Collider>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
            if (collider != null)
            {
                collider.enabled = false;
            }

            // Accedi all'AudioSource e riproduci il suono
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
                Debug.Log("Audio attivato!"); // Messaggio di debug
            }
            
            if (SceneManager.GetActiveScene().name == "SinglePlayerScene")
            {
                SurvivalPickupsManager survivalPickupsManager = FindObjectOfType<SurvivalPickupsManager>();
                if (survivalPickupsManager != null)
                {
                    // Notifica SurvivalPickupsManager della raccolta
                    survivalPickupsManager.CollectItem("gem", value); // Assicurati che la stringa corrisponda esattamente a quella usata nel tuo script manager
                }
            }
            else
            {
                CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
                if (currencyManager != null)
                {
                    // Modifica le gemme del player
                    currencyManager.ModifyGems(value);
                }
            }

            if (pickupEffect != null)
            {
                // Istanza l'effetto e memorizza il riferimento
                GameObject effectInstance = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                // Modifica la scala dell'effetto istanziato
                effectInstance.transform.localScale = new Vector3(10f, 10f, 10f); // Modifica questo valore per ottenere la scala desiderata
            }

            Destroy(gameObject, 1f); // Distruggi la gemma dopo la raccolta
        }
        if (other.CompareTag("Enemy"))
        {
                        // Disattiva tutti i figli del gameobject
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
             // Disattiva il renderer e il collider
            Renderer renderer = GetComponent<Renderer>();
            Collider collider = GetComponent<Collider>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
            if (collider != null)
            {
                collider.enabled = false;
            }

            // Accedi all'AudioSource e riproduci il suono
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
                Debug.Log("Audio attivato!"); // Messaggio di debug
            }
                  if (pickupEffect != null)
            {
                // Istanza l'effetto e memorizza il riferimento
                GameObject effectInstance = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                // Modifica la scala dell'effetto istanziato
                effectInstance.transform.localScale = new Vector3(10f, 10f, 10f); // Modifica questo valore per ottenere la scala desiderata
            }
            Destroy(gameObject, 1f);
        }
    }
}
