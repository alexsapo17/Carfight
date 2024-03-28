using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyPunchAbility : MonoBehaviour
{
    public GameObject prefabToSpawn; 
    public float speed = 50f; 
    public float offsetDistance = 13f; 
    public float raycastLength = 10f; // Lunghezza del raycast
    public float cooldownDuration = 5f; 
    public float destroyDelay = 3f; 
public float yOffset = 1f;
    private bool isCooldown = false;
    private RaycastHit hit;

    void Update()
    {
        // Controlla se il mouse è stato cliccato
        if (!isCooldown)
        {
            // Lancia un raycast in avanti dalla posizione del gameobject
            if (Physics.Raycast(transform.position + Vector3.up * yOffset, transform.forward, out hit, raycastLength)) // Passa la lunghezza del raycast
            {
                // Controlla se il raycast ha colpito un oggetto con il tag "Player" o "Enemy"
                if (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Enemy"))
                {
                    SpawnPrefab();
                }
            }
                        Debug.DrawRay(transform.position, transform.forward * raycastLength, Color.red);

        }
    }

    void SpawnPrefab()
    {
        // Ottiene la posizione e la rotazione del gameobject a cui è attaccato lo script
        Vector3 spawnPosition = transform.position + transform.forward * offsetDistance;
        Quaternion spawnRotation = transform.rotation;

        // Istanza il prefab nella posizione e rotazione calcolate
        GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

        // Aggiungi una forza costante al prefab per farlo muovere in avanti
        Rigidbody rb = spawnedPrefab.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = transform.forward * speed;
        }

        // Distruggi il prefab dopo un certo periodo di tempo
        Destroy(spawnedPrefab, destroyDelay);

        StartCooldown();
    }

    void StartCooldown()
    {
        isCooldown = true; 
        StartCoroutine(ResetCooldown(cooldownDuration)); 
    }

    IEnumerator ResetCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);

        isCooldown = false; 
    }
}
