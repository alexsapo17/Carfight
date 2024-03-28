using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObs : MonoBehaviour
{
    public float distance = 5f; // Distance that moves the object
    public bool horizontal = true; // If the movement is horizontal or vertical
    public float speed = 3f; // Speed of movement or upward force
    public float offset = 0f; // If you want to modify the position at the start
    public bool enableTapisRoulantEffect = false; // Controllo per l'effetto tapis roulant
public float upwardForce = 5f; 
    private bool isForward = true; // If the movement is out
    private Vector3 startPos;

    void Awake()
    {
        startPos = transform.position;
        if (horizontal)
            transform.position += Vector3.right * offset;
        else
            transform.position += Vector3.forward * offset;
    }
void Start()
{
    // Ottieni tutti i collider presenti nella scena
    Collider[] allColliders = FindObjectsOfType<Collider>();

    // Ottieni tutti i collider del gameobject a cui è attaccato lo script
    Collider[] thisColliders = GetComponents<Collider>();

    // Cicla su tutti i collider presenti nella scena
    foreach (Collider col in allColliders)
    {
        // Verifica se il collider non appartiene al gameobject a cui è attaccato lo script
        if (!IsColliderInArray(col, thisColliders))
        {
            // Ignora le collisioni con i collider che non hanno il tag "Player" o "Enemy"
            if (!col.gameObject.CompareTag("Player") && !col.gameObject.CompareTag("Enemy"))
            {
                foreach (Collider thisCol in thisColliders)
                {
                    // Ignora la collisione tra il collider del gameobject e il collider dell'oggetto senza il tag desiderato
                    Physics.IgnoreCollision(thisCol, col, true);
                }
            }
        }
    }
}

bool IsColliderInArray(Collider collider, Collider[] array)
{
    // Cicla su tutti i collider nell'array
    foreach (Collider col in array)
    {
        // Se il collider è presente nell'array, restituisci true
        if (col == collider)
        {
            return true;
        }
    }
    // Se il collider non è presente nell'array, restituisci false
    return false;
}


    void Update()
    {
        MoveObject();
    }

    void MoveObject()
    {
        if (horizontal)
        {
            if (isForward)
            {
                if (transform.position.x < startPos.x + distance)
                {
                    transform.position += Vector3.right * Time.deltaTime * speed;
                }
                else
                    isForward = false;
            }
            else
            {
                if (transform.position.x > startPos.x)
                {
                    transform.position -= Vector3.right * Time.deltaTime * speed;
                }
                else
                    isForward = true;
            }
        }
        else
        {
            if (isForward)
            {
                if (transform.position.z < startPos.z + distance)
                {
                    transform.position += Vector3.forward * Time.deltaTime * speed;
                }
                else
                    isForward = false;
            }
            else
            {
                if (transform.position.z > startPos.z)
                {
                    transform.position -= Vector3.forward * Time.deltaTime * speed;
                }
                else
                    isForward = true;
            }
        }
    }
void OnCollisionStay(Collision collision)
{
    if (enableTapisRoulantEffect && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy")))
    {
        Rigidbody rb = collision.collider.attachedRigidbody;
        if (rb != null)
        {
            // Calcola la direzione orizzontale basata sulla posizione relativa al MovableObs
            Vector3 horizontalDirection = (collision.transform.position - transform.position).normalized;
            // Ignora la componente verticale della direzione per spingere orizzontalmente
            horizontalDirection.y = 0;

            // Combina la spinta verso l'alto con la spinta orizzontale
            Vector3 forceDirection = Vector3.up * upwardForce + horizontalDirection * speed;
            rb.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
    else
    {
        // Ignora la collisione con gli oggetti che non hanno il tag "Player" o "Enemy"
        Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
    }
}


}
