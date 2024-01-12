using UnityEngine;

public class EliminatedLine : MonoBehaviour
{
    private LevelManager levelManager;

   void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assicurati che il tag del giocatore sia "Player"
        {
                        levelManager.EliminatedPlayer();

        }
    }
}
