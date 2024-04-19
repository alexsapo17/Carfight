using UnityEngine;

public class AudioFreezeBall : MonoBehaviour
{
    public AudioSource audioSource;
    public float maxVolumeDistance = 100f;
    public float minVolume = 0.1f;
    public float maxVolume = 1f;

    private void Update()
    {
        // Trova il gameobject più vicino con il tag "FreezeBall"
        GameObject nearestObject = FindNearestGameObjectWithTag("FreezeBall");

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
        }
        else
        {
            // Se non viene trovato nessun gameobject con il tag "FreezeBall", imposta il volume a zero
            audioSource.volume = 0f;
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
