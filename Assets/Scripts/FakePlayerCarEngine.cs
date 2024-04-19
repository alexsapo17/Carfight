using UnityEngine;

public class FakePlayerCarEngine : MonoBehaviour
{
    public AudioSource audioSource;
    public Rigidbody carRigidbody;
    public float pitchMultiplier = 2f;
    public float maxSpeed = 10f;

    private void Update()
    {
        if (carRigidbody != null && audioSource != null)
        {
            // Calcola la velocità del gameobject
            float speed = carRigidbody.velocity.magnitude;

            // Normalizza la velocità tra 0 e 1 in base alla velocità massima
            float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

            // Calcola il pitch in base alla velocità normalizzata e al moltiplicatore di pitch
            float pitch = 1f + normalizedSpeed * pitchMultiplier;

            // Imposta il pitch dell'AudioSource
            audioSource.pitch = pitch;
        }
    }
}
