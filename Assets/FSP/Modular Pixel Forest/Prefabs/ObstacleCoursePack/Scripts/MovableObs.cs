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
    if (enableTapisRoulantEffect)
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
}

}
