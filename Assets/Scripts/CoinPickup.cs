using UnityEngine;


public class CoinPickup : MonoBehaviour
{
    public int value = 10; // Valore della moneta
    public GameObject pickupEffect; // Assegna il prefab del sistema di particelle nell'Editor

  
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SurvivalPickupsManager SurvivalPickupsManager = FindObjectOfType<SurvivalPickupsManager>();
            // Notifica SurvivalPickupsManager della raccolta
            SurvivalPickupsManager.CollectItem("coin", value); // Assicurati che la stringa corrisponda esattamente a quella usata nel tuo script manager
            
            if (pickupEffect != null)
            {
                // Istanza l'effetto e memorizza il riferimento
                GameObject effectInstance = Instantiate(pickupEffect, transform.position, Quaternion.identity);
                // Modifica la scala dell'effetto istanziato
                effectInstance.transform.localScale = new Vector3(10f, 10f, 10f); // Modifica questo valore per ottenere la scala desiderata
            }
            
            Destroy(gameObject); // Distruggi la moneta dopo la raccolta
        }
    }
}
