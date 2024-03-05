using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TreeCollision : MonoBehaviour
{
    private Rigidbody rb;
    private Collider[] treeColliders;
    private Collider terrainCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Disattiva la gravità inizialmente
        rb.isKinematic = true; // Imposta l'albero come kinematico inizialmente

        terrainCollider = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Collider>();
        if (terrainCollider != null)
        {
            treeColliders = GetComponentsInChildren<Collider>();
            foreach (var col in treeColliders)
            {
                Physics.IgnoreCollision(col, terrainCollider, true);
            }
        }
        else
        {
            Debug.LogError("Terrain collider not found. Make sure your terrain has the 'Terrain' tag.");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            rb.useGravity = true; // Attiva la gravità
            rb.isKinematic = false; // Imposta l'albero come non kinematico

            if (terrainCollider != null)
            {
                foreach (var col in treeColliders)
                {
                    Physics.IgnoreCollision(col, terrainCollider, false);
                }
            }

            Destroy(gameObject, 2f); // Distruggi l'oggetto dopo un ritardo
        }
    }
}
