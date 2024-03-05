using UnityEngine;
using System.Collections; // Necessario per usare StartCoroutine

public class EnemyCollision : MonoBehaviour
{
    public string prefabName = "CFXR4 Firework 1 Cyan-Purple (HDR)"; // Sostituisci con il nome esatto del tuo prefab

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Istanza il prefab al punto di collisione
            GameObject prefab = Resources.Load<GameObject>(prefabName);
            if (prefab != null)
            {
                Instantiate(prefab, collision.contacts[0].point, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Prefab non trovato: " + prefabName);
            }

            StartCoroutine(SlowMotionEffect());
        }
    }

    private IEnumerator SlowMotionEffect()
    {
        Time.timeScale = 0.2f; // Imposta lo slow-motion
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Mantiene la fisica fluida durante lo slow-motion

        yield return new WaitForSecondsRealtime(1f); // Aspetta 1 secondo in tempo reale

        Time.timeScale = 1f; // Ripristina la velocità normale del gioco
        Time.fixedDeltaTime = 0.02f; // Ripristina il valore predefinito di fixedDeltaTime

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.FinishSurvivalGame();
        }
        else
        {
            Debug.LogError("LevelManager non trovato nella scena.");
        }
    }
}
