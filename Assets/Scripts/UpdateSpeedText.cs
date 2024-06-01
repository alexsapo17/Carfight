using UnityEngine;
using UnityEngine.UI;

public class UpdateSpeedText : MonoBehaviour
{
    private Rigidbody rigidbodyToTrack;
    public Text speedText;

    void Start()
    {
        // Ottieni il Rigidbody dall'oggetto corrente
        rigidbodyToTrack = GetComponent<Rigidbody>();

        // Assicurati che il Rigidbody sia stato assegnato
        if (rigidbodyToTrack == null)
        {
            Debug.LogError("Rigidbody da tracciare non assegnato!");
            enabled = false; // Disabilita questo script
            return;
        }

        // Se il testo non è stato assegnato, cerca un GameObject con un componente Text nella scena
        if (speedText == null)
        {
            GameObject speedTextGO = GameObject.Find("SpeedText");
            if (speedTextGO != null)
            {
                speedText = speedTextGO.GetComponent<Text>();
            }
            else
            {
                enabled = false; // Disabilita questo script
                return;
            }
        }

        // Avvia l'aggiornamento del testo ogni 0.2 secondi
        InvokeRepeating("UpdateSpeed", 0f, 0.1f);
    }

    void UpdateSpeed()
    {
        // Assicurati che il Rigidbody e il testo siano ancora assegnati
        if (rigidbodyToTrack == null || speedText == null)
            return;

        // Ottieni la velocità dal Rigidbody e convertila in chilometri all'ora
        float speedKMH = rigidbodyToTrack.velocity.magnitude * 2.8f;

        // Aggiorna il testo con la velocità in chilometri all'ora, senza decimali
        speedText.text = "" + Mathf.Round(speedKMH) + "";
    }
}
