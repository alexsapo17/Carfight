using UnityEngine;

public class AudioCarEngine : MonoBehaviour
{
    public AudioSource audioSource;
    public float maxVolumeDistance = 100f;
    public float minVolume = 0.1f;
    public float maxVolume = 1f;
    public float maxSpeed = 10f;
    public float maxPitchMultiplier = 2f;

    private void Update()
    {
        // Trova il gameobject più vicino con il tag "Player" o "Enemy"
        GameObject nearestObject = FindNearestGameObjectWithTag("Player");
        if (nearestObject == null)
            nearestObject = FindNearestGameObjectWithTag("Enemy");

        if (nearestObject != null)
        {
            // Calcola la distanza tra il gameobject corrente e il gameobject più vicino
            float distance = Vector3.Distance(transform.position, nearestObject.transform.position);

            // Normalizza la distanza tra 0 e 1 rispetto alla distanza massima
            float normalizedDistance = Mathf.Clamp01(distance / maxVolumeDistance);

            // Calcola il volume in base alla distanza normalizzata
            float volume = Mathf.Lerp(maxVolume, minVolume, normalizedDistance);

            // Imposta il volume dell'AudioSource
            audioSource.volume = volume;

            // Calcola la velocità del gameobject più vicino
            Rigidbody nearestObjectRigidbody = nearestObject.GetComponent<Rigidbody>();
            if (nearestObjectRigidbody != null)
            {
                float speed = nearestObjectRigidbody.velocity.magnitude;

                // Normalizza la velocità tra 0 e 1 rispetto alla velocità massima
                float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);

                // Calcola il pitch multiplier in base alla velocità normalizzata
                float pitchMultiplier = Mathf.Lerp(1f, maxPitchMultiplier, normalizedSpeed);

                // Imposta il pitch dell'AudioSource
                audioSource.pitch = pitchMultiplier;
            }
        }
    }

    private GameObject FindNearestGameObjectWithTag(string tag)
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);
        float minDistance = float.MaxValue;
        GameObject nearestObject = null;
        
        foreach (GameObject go in gameObjects)
        {
            if (go != gameObject) // Escludi se stesso
            {
                float distance = Vector3.Distance(transform.position, go.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestObject = go;
                }
            }
        }

        return nearestObject;
    }
}
