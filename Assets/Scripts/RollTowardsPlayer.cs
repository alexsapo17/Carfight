using UnityEngine;

public class RollTowardsPlayer : MonoBehaviour
{
    public string targetTag = "Player"; // Tag del gameobject verso cui la palla deve rotolare
    public float rollForce = 5f; // Forza di rotolamento

    private GameObject target; // Gameobject con il tag "Player" verso cui la palla deve rotolare
    private Rigidbody rb; // RigidBody della palla

    void Start()
    {
        // Trova il gameobject con il tag "Player"
        target = GameObject.FindGameObjectWithTag(targetTag);

        // Controlla se è stato trovato un gameobject con il tag "Player"
        if (target == null)
        {
            Debug.LogError("Impossibile trovare il gameobject con il tag 'Player'. Assicurati che il tag sia corretto.");
        }

        // Ottieni il RigidBody della palla
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
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
}
