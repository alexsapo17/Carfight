using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TreeCollision : MonoBehaviour
{
    private Rigidbody rb;
    private Collider[] treeColliders;
    private Collider terrainCollider;
    public GameObject player; // Riferimento al player, impostalo dall'Editor o trovalo dinamicamente
    public float activationDistance = 10f; // Distanza alla quale l'albero diventa non kinematico

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

        // Trova il player se non è stato impostato
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
    }

    void Update()
    {
        if(player != null && rb.isKinematic && Vector3.Distance(transform.position, player.transform.position) < activationDistance)
        {
            rb.isKinematic = false; // Rendi l'albero non kinematico quando il player si avvicina
            rb.useGravity = true; // Attiva la gravità
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")) && !rb.isKinematic)
        {
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
