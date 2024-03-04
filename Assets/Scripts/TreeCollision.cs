using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Assicura che ci sia un Rigidbody
public class TreeCollision : MonoBehaviour
{
    private Rigidbody rb;
    private Collider[] treeColliders;
    private Collider terrainCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Inizialmente disattiva la gravità

        // Ottiene tutti i collider dell'albero, nel caso in cui ci siano più collider (es. mesh collider + collider di trigger)
        treeColliders = GetComponentsInChildren<Collider>();

        // Trova il collider del terreno
        terrainCollider = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>(); // Assicurati che il terreno abbia il tag "Terrain"

        // Ignora le collisioni tra l'albero e il terreno
        foreach (var col in treeColliders)
        {
            Physics.IgnoreCollision(col, terrainCollider, true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Controlla se l'oggetto collidente ha il tag "Player" o "Enemy"
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            rb.useGravity = true; // Attiva la gravità

            // Riabilita le collisioni tra l'albero e il terreno
            foreach (var col in treeColliders)
            {
                Physics.IgnoreCollision(col, terrainCollider, false);
            }

            // Distruggi l'oggetto dopo un certo ritardo
            Destroy(gameObject, 2f);
        }
    }
}
