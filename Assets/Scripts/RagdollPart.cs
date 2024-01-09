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
        mainController?.CollisionDetected(this, collision);
    }
}

