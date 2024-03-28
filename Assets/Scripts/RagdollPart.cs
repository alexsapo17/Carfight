using UnityEngine;


public class RagdollPart : MonoBehaviour
{
    public RagdollController mainController;

    void Start()
    {
        if (mainController == null)
        {
            mainController = GetComponentInParent<RagdollController>();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
                    if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
{
        mainController?.CollisionDetected(this, collision);
    }
    }
}

