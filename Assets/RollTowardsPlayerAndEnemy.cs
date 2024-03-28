using UnityEngine;

public class RollTowardsPlayerAndEnemy : MonoBehaviour
{
    public float rollForce = 10f; // Forza di rotolamento

    private GameObject target; // Gameobject verso cui la palla deve rotolare
    private Rigidbody rb; // RigidBody della palla

    void Start()
    {
        // Ottieni il RigidBody della palla
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Trova il gameobject più vicino contrassegnato come "Player" o "Enemy"
        FindNearestTarget();

        // Controlla se il gameobject target è valido
        if (target != null)
        {
            // Calcola la direzione verso il target
            Vector3 direction = (target.transform.position - transform.position).normalized;

            // Calcola la forza di rotolamento nella direzione del target
            Vector3 rollForceVector = direction * rollForce;

            // Applica la forza di rotolamento alla palla
            rb.AddForce(rollForceVector);
        }
    }

    void FindNearestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        float minDistance = Mathf.Infinity;
        GameObject nearestTarget = null;

        // Cerca il giocatore più vicino
        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = player;
            }
        }

        // Cerca il nemico più vicino
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = enemy;
            }
        }

        // Imposta il target più vicino trovato
        target = nearestTarget;
    }
}
