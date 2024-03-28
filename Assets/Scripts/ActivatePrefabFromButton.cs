using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Collections;

public class ActivatePrefabFromButton : MonoBehaviour
{
    public Button spawnButton; // Assegna questo nell'Inspector
    public GameObject prefabToSpawn; // Assegna il prefab da istanziare nell'Inspector
    public float speed = 5f; // Velocità di movimento del prefab
    public float offsetDistance = 1f; // Distanza di offset davanti al player
        public float offsetDistancey = 1f; // Distanza di offset davanti al player

    public float cooldownDuration = 5f; // Durata del cooldown prima che il pulsante possa essere riattivato
    public float destroyDelay = 3f; // Tempo prima che il prefab venga distrutto

    private bool isCooldown = false;
    private Quaternion spawnRotation;

    void Start()
    {
        spawnButton.onClick.AddListener(SpawnPrefab);
    }

    void SpawnPrefab()
    {
        if (isCooldown) return;

        // Cerca tra tutti i PhotonView presenti nella scena
        foreach (var pv in FindObjectsOfType<PhotonView>())
        {
            // Controlla se il PhotonView appartiene al giocatore locale e ha il tag "Player"
            if (pv.IsMine && pv.gameObject.CompareTag("Player"))
            {
                // Ottieni la posizione e la rotazione della macchina
                Vector3 spawnPosition = pv.transform.position + pv.transform.forward * offsetDistance + pv.transform.up * offsetDistancey;
                Quaternion spawnRotation = pv.transform.rotation;

                // Istanza il prefab nella posizione e rotazione della macchina
                GameObject spawnedPrefab = Instantiate(prefabToSpawn, spawnPosition, spawnRotation);

                // Aggiungi una forza costante al prefab per farlo muovere in avanti
                Rigidbody rb = spawnedPrefab.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = pv.transform.forward * speed;
                }

                // Distruggi il prefab dopo un certo periodo di tempo
                Destroy(spawnedPrefab, destroyDelay);

                StartCooldown();
                break; // Interrompe il ciclo una volta trovata la macchina del giocatore locale
            }
        }
    }

    void StartCooldown()
    {
        isCooldown = true; // Attiva il flag di cooldown
        spawnButton.interactable = false; // Disabilita il pulsante
        StartCoroutine(ResetCooldown(cooldownDuration)); // Imposta un timer per resettare il cooldown
    }

    IEnumerator ResetCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);

        isCooldown = false; // Resetta il flag di cooldown
        spawnButton.interactable = true; // Riabilita il pulsante
    }
}
