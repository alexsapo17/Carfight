using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
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
}
