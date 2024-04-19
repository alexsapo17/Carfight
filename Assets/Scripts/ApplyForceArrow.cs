using UnityEngine;

public class ApplyForceArrow : MonoBehaviour
{
    public Vector3 direzioneForzaLocale = Vector3.forward; // La direzione locale in cui applicare la forza
    public float forza = 10f; // La forza da applicare
    public string[] tagsTarget; // Array di tag dei GameObject su cui applicare la forza
public AudioSource audioSource;
    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il GameObject che ha attivato il trigger ha un Rigidbody e se il suo tag è nella lista dei target
        if (other.attachedRigidbody != null && ContieneTag(other.tag))
        {

            // Calcola la direzione della forza nel sistema di coordinate locale del GameObject
            Vector3 direzioneForzaMondo = transform.TransformDirection(direzioneForzaLocale);

            // Calcola la velocità del GameObject nel momento dell'impatto
            Vector3 velocita = other.attachedRigidbody.velocity;

            // Calcola il prodotto scalare tra la velocità del GameObject e la direzione della forza
            float dotProduct = Vector3.Dot(velocita.normalized, direzioneForzaMondo.normalized);

            // Se il prodotto scalare è positivo, significa che il GameObject si sta muovendo nella direzione in cui dovrebbe essere spinto
            if (dotProduct > 0)
            {
                // Applica la forza al Rigidbody nella direzione specificata
                other.attachedRigidbody.AddForce(direzioneForzaMondo * forza, ForceMode.Impulse);
                            audioSource.Play();

            }
        }
    }

    // Funzione per controllare se il tag del GameObject è nella lista dei target 
    private bool ContieneTag(string tag) 
    {
        foreach (string t in tagsTarget)
        {
            if (t == tag)
            {
                return true;
            }
        }
        return false;
    }
}
