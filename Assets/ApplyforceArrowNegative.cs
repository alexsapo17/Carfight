using UnityEngine;

public class ApplyForceArrowNegative : MonoBehaviour
{
    public float velocitaRallentamento = 5f; // La velocità a cui rallentare il gameobject
    public string[] tagsTarget; // Array di tag dei GameObject su cui applicare la forza
public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        // Controlla se il GameObject che ha attivato il trigger ha un Rigidbody e se il suo tag è nella lista dei target
        if (ContieneTag(other.tag))
        {
            Rigidbody rb = other.attachedRigidbody;
            if (rb != null)
            {
                // Calcola la velocità del GameObject nel momento dell'impatto
                Vector3 velocita = rb.velocity;
            audioSource.Play();

                // Calcola la direzione opposta alla velocità attuale per applicare una forza di rallentamento
                Vector3 direzioneOpposta = -velocita.normalized;

                // Calcola la forza di rallentamento basata sulla velocità di rallentamento
                Vector3 forzaRallentamento = direzioneOpposta * velocitaRallentamento;

                // Applica la forza di rallentamento al Rigidbody
                rb.AddForce(forzaRallentamento, ForceMode.Impulse);
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
