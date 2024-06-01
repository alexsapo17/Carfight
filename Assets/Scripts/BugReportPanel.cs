using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using System;
using System.Collections;
using Firebase.Auth;

public class BugReportPanel : MonoBehaviour
{
    public GameObject bugReportPanel;
    public InputField bugDescriptionInputField;
    public GameObject nextPanel;
    public GameObject reportLimitText; // Testo per visualizzare il limite dei report giornalieri
    private DatabaseReference databaseReference;
    private FirebaseAuth auth;
    private int dailyReportLimit = 3; // Limite giornaliero dei report
    private int reportsSentToday = 0; // Numero di report inviati oggi
    private string lastReportDate = ""; // Data dell'ultimo report inviato

    private void Start()
    {
        // Ottieni il riferimento al database Firebase
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Ottieni l'istanza di FirebaseAuth
        auth = FirebaseAuth.DefaultInstance;

        bugReportPanel.SetActive(false);

        // Carica il conteggio e la data dell'ultimo report inviato
        LoadReportData();
    }

    private void LoadReportData()
    {
        // Controlla se l'utente è autenticato
        if (auth.CurrentUser != null)
        {
            // Ottieni l'ID dell'utente corrente
            string userId = auth.CurrentUser.UserId;

            // Carica i dati del report dall'archivio
            FirebaseDatabase.DefaultInstance.GetReference("reportData").Child(userId).GetValueAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot != null && snapshot.Exists)
                    {
                        object reportDateObj = snapshot.Child("lastReportDate").Value;
                        if (reportDateObj != null)
                        {
                            lastReportDate = reportDateObj.ToString();
                        }

                        object reportCountObj = snapshot.Child("reportsSentToday").Value;
                        if (reportCountObj != null)
                        {
                            reportsSentToday = int.Parse(reportCountObj.ToString());
                        }
                    }
                }
            });
        }
    }

    private void SaveReportData()
    {
        // Controlla se l'utente è autenticato
        if (auth.CurrentUser != null)
        {
            // Ottieni l'ID dell'utente corrente
            string userId = auth.CurrentUser.UserId;

            // Salva i dati del report nell'archivio
            databaseReference.Child("reportData").Child(userId).Child("lastReportDate").SetValueAsync(lastReportDate);
            databaseReference.Child("reportData").Child(userId).Child("reportsSentToday").SetValueAsync(reportsSentToday);
        }
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



        // Controlla se l'utente ha già inviato un report oggi
        if (DateTime.Today.ToString("dd/MM/yyyy") != lastReportDate)
        {
            // Se è una nuova giornata, azzera il conteggio dei report inviati oggi e aggiorna la data dell'ultimo report
            reportsSentToday = 0;
            lastReportDate = DateTime.Today.ToString("dd/MM/yyyy");
        }
        // Controlla se l'utente ha superato il limite giornaliero
        if (reportsSentToday >= dailyReportLimit)
        {
            // Mostra un messaggio all'utente per informarlo che ha superato il limite giornaliero
            reportLimitText.SetActive(true);
            return;
        }
        // Invia il report
        if (auth.CurrentUser != null)
        {
            string userId = auth.CurrentUser.UserId;
            string reportId = databaseReference.Child("reports").Child(userId).Push().Key;
            databaseReference.Child("reports").Child(userId).Child(reportId).SetValueAsync(bugDescription)
                .ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        Debug.Log("Bug report inviato con successo a Firebase.");

                        // Aggiorna il conteggio dei report inviati oggi
                        reportsSentToday++;

                        // Salva i dati del report
                        SaveReportData();
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
