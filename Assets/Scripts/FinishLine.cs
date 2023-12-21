using UnityEngine;
using Photon.Pun;

public class FinishLine : MonoBehaviour
{
    private GameManager gameManager;

 void Start()
{
    gameManager = FindObjectOfType<GameManager>();
    if (gameManager != null)
    {
        Debug.Log("GameManager trovato.");
    }
    else
    {
        Debug.LogError("GameManager non trovato.");
    }
}

void OnTriggerEnter(Collider other)
{
    Debug.Log("OnTriggerEnter chiamato con: " + other.gameObject.name);
    if (other.gameObject.tag == "Player")
    {
gameManager.PlayerEliminated(other.gameObject);
    }
}

}
