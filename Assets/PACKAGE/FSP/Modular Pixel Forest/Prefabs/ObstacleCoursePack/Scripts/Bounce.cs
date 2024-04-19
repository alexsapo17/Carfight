using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float force = 10f; // Forza del rimbalzo

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            // Calcola la direzione del rimbalzo
            Vector3 hitDir = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                hitDir += contact.normal;
            }
            hitDir /= collision.contacts.Length;

            // Applica l'effetto di rimbalzo al GameObject colpito
            Rigidbody otherRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            if (otherRigidbody != null)
            {
                otherRigidbody.AddForce(hitDir * force, ForceMode.Impulse);
            }
        }
    }
}
