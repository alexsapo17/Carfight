using UnityEngine;

public class FinishLevelTrigger : MonoBehaviour
{
    private LevelManager levelManager;
    private float lastRaceFinishTime = 0f;
    public float cooldownTime = 1f; // Secondi di cooldown prima di poter chiamare di nuovo FinishRace

    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Time.time - lastRaceFinishTime >= cooldownTime)
            {
                lastRaceFinishTime = Time.time;
                levelManager.FinishRace();
            }
        }
    }
}
