using UnityEngine;

public class AudioIntensity : MonoBehaviour
{
    public AudioSource audioSource;
    public float maxDistance = 10f;

    private void Update()
    {
        // Trova tutti gli oggetti nell'area circostante
        Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance);

        // Calcola l'intensità del suono in base alla distanza dagli oggetti circostanti
        foreach (Collider collider in colliders)
        {
            // Ignora se stesso
            if (collider.gameObject == gameObject)
                continue;

            // Calcola la distanza tra questo oggetto e l'oggetto circostante
            float distance = Vector3.Distance(transform.position, collider.transform.position);

            // Calcola l'intensità del suono in base alla distanza (più vicino è l'oggetto, più alta è l'intensità)
            float intensity = 1f - Mathf.Clamp01(distance / maxDistance);

            // Regola il volume dell'AudioSource in base all'intensità
            if (audioSource != null)
            {
                audioSource.volume = intensity;
            }
        }
    }
}
