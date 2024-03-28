using UnityEngine;
using System.Collections;
public class FreezeObject : MonoBehaviour
{
    public GameObject prefabToSpawn; // Prefab da istanziare
    public GameObject prefabToSpawnCollision; // Prefab da istanziare

    public float freezeDuration = 3f; // Durata della trasformazione in kinematic
    public float destroyDelay = 3f; // Tempo prima che il prefab venga distrutto
    private bool isDestroyed = false; // Flag per controllare se il gameobject è stato già distrutto

    void OnCollisionEnter(Collision collision)
    {
        // Controlla se il gameobject che ha attaccato lo script entra in collisione con un altro gameobject
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // Ottiene il rigidbody dell'altro gameobject
            Rigidbody otherRb = collision.collider.attachedRigidbody;

            // Se l'altro gameobject ha un rigidbody
            if (otherRb != null)
            {
                // Trasforma temporaneamente il rigidbody dell'altro gameobject in kinematic
                otherRb.isKinematic = true;

                // Avvia la coroutine per ripristinare il rigidbody dopo la durata specificata
                StartCoroutine(ReleaseKinematic(otherRb));
                
                // Ottiene la posizione della collisione
                Vector3 collisionPoint = collision.contacts[0].point;

                // Ottiene il centro del gameobject colpito
                Vector3 hitCenter = collision.gameObject.GetComponent<Collider>().bounds.center;

                // Istanza il prefab nel punto di collisione
                GameObject spawnedPrefab1 = Instantiate(prefabToSpawnCollision, collisionPoint, Quaternion.identity);
                
                // Istanza il prefab al centro del gameobject colpito
                GameObject spawnedPrefab2 = Instantiate(prefabToSpawn, hitCenter, Quaternion.identity);

                // Distrugge entrambi i prefab dopo un certo periodo di tempo
                Destroy(spawnedPrefab1, destroyDelay);
                Destroy(spawnedPrefab2, destroyDelay);
                
                // Avvia la coroutine per distruggere il gameobject a cui è attaccato lo script
                StartCoroutine(DestroyThisObjectAfterDelay());
            }
        }
    }

    IEnumerator ReleaseKinematic(Rigidbody rb)
    {
        yield return new WaitForSeconds(freezeDuration);

        // Ripristina il rigidbody alla sua forma originale
        rb.isKinematic = false;
    }
    
    IEnumerator DestroyThisObjectAfterDelay()
    {
        // Aspetta finché non sono stati distrutti entrambi i prefab
        yield return new WaitForSeconds(destroyDelay);
        
        // Distrugge il gameobject a cui è attaccato lo script
        Destroy(gameObject);
    }
}
