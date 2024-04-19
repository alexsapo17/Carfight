using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using System.Collections;
using Firebase.Auth;
public class BugReportPanel : MonoBehaviour
{
    public GameObject bugReportPanel;
    public InputField bugDescriptionInputField;
    public GameObject nextPanel;
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;

    private void Start()
    {
        // Ottieni il riferimento al database Firebase
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Ottieni l'istanza di FirebaseAuth
        auth = FirebaseAuth.DefaultInstance;

        bugReportPanel.SetActive(false);
    }

    public void OpenBugReportPanel()
    {
        bugReportPanel.SetActive(true);
    }

    public void CloseBugReportPanel()
    {
        bugReportPanel.SetActive(false);
    }

    public void SendBugReport()
    {
        string bugDescription = bugDescriptionInputField.text;

        // Controlla se l'utente è autenticato
        if (auth.CurrentUser != null)
        {
            // Ottieni l'ID dell'utente corrente
            string userId = auth.CurrentUser.UserId;

            // Crea un nuovo nodo nel database per il report del bug, associato all'ID dell'utente
            string reportId = databaseReference.Child("reports").Child(userId).Push().Key;
            databaseReference.Child("reports").Child(userId).Child(reportId).SetValueAsync(bugDescription)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Bug report inviato con successo a Firebase.");
                    }
                    else
                    {
                        Debug.LogError("Errore durante l'invio del bug report a Firebase: " + task.Exception);
                    }
                });
        }
        else
        {
            Debug.LogError("Utente non autenticato. Impossibile inviare il report.");
        }

        // Chiudi il pannello del report
        CloseBugReportPanel();

        // Avvia una coroutine per attivare il prossimo pannello dopo 2 secondi
        StartCoroutine(ActivateNextPanelAfterDelay(2f));

        Debug.Log("Bug Report: " + bugDescription);
    }

    IEnumerator ActivateNextPanelAfterDelay(float delay)
    {
        nextPanel.SetActive(true);
        yield return new WaitForSeconds(delay);
        nextPanel.SetActive(false);
    }
}
