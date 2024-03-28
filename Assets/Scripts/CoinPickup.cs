using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinPickup : MonoBehaviour
{
    public int value = 10; // Valore della moneta
    public GameObject pickupEffect; // Assegna il prefab del sistema di particelle nell'Editor

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SceneManager.GetActiveScene().name == "SinglePlayerScene")
            {
                SurvivalPickupsManager survivalPickupsManager = FindObjectOfType<SurvivalPickupsManager>();
                if (survivalPickupsManager != null)
                {
                    // Notifica SurvivalPickupsManager della raccolta
                    survivalPickupsManager.CollectItem("coin", value); // Assicurati che la stringa corrisponda esattamente a quella usata nel tuo script manager
                }
            }
            else
            {
                CurrencyManager currencyManager = FindObjectOfType<CurrencyManager>();
                if (currencyManager != null)
                {
                    // Modifica le monete del player
                    currencyManager.ModifyCoins(value);
                }
            }

            if (pickupEffect != null)
            {
                // Istanza l'effetto e memorizza il riferimento
                GameObject effectInstance = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                // Modifica la scala dell'effetto istanziato
                effectInstance.transform.localScale = new Vector3(10f, 10f, 10f); // Modifica questo valore per ottenere la scala desiderata
            }

             // Distruggi la moneta dopo la raccolta
                         Destroy(gameObject);

        }
                if (other.CompareTag("Enemy"))
        {
                        if (pickupEffect != null)
            {
                // Istanza l'effetto e memorizza il riferimento
                GameObject effectInstance = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                // Modifica la scala dell'effetto istanziato
                effectInstance.transform.localScale = new Vector3(10f, 10f, 10f); // Modifica questo valore per ottenere la scala desiderata
            }
            Destroy(gameObject);
            }
    }
}
