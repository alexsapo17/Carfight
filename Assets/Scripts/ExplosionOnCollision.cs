using UnityEngine;

public class ExplosionOnCollision : MonoBehaviour
{
    public GameObject explosionPrefab; // Prefab dell'esplosione
    public float explosionForce = 100f; // Forza dell'esplosione
    public float upwardForce = 50f; // Forza verso l'alto
    public float explosionRadius = 10f; // Raggio dell'esplosione
    public float destroyDelay = 2f; // Tempo prima che il prefab dell'esplosione venga distrutto

    private bool hasExploded = false; // Flag per controllare se l'esplosione è già avvenuta

    void OnCollisionEnter(Collision collision)
    {
        if (!hasExploded && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")))
        {
            // Crea l'esplosione al punto di collisione
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Trova tutti i rigidbody nell'area circostante all'esplosione
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider col in colliders)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calcola una forza combinata orizzontale e verticale
                    Vector3 direction = (col.transform.position - transform.position).normalized;
                    Vector3 force = direction * explosionForce + Vector3.up * upwardForce;

                    // Applica la forza al rigidbody
                    rb.AddForce(force, ForceMode.Impulse);
                }
            }

            // Distruggi l'esplosione dopo un certo periodo di tempo
            Destroy(explosion, destroyDelay);

            // Imposta il flag per indicare che l'esplosione è avvenuta
            hasExploded = true;

            // Distruggi questo oggetto dopo il delay
            Destroy(gameObject);
        }
    }
}
