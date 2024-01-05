using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LogToUI : MonoBehaviour
{
public TMP_Text logText;
    private string myLog;
    private Queue myLogQueue = new Queue();

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLog = logString;
        string newString = "\n [" + type + "] : " + myLog;
        myLogQueue.Enqueue(newString);
        if (type == LogType.Exception)
        {
            newString = "\n" + stackTrace;
            myLogQueue.Enqueue(newString);
        }

        myLog = string.Empty;
        foreach (string mylog in myLogQueue)
        {
            myLog += mylog;
        }

        logText.text = myLog;
    }

    // Limita il numero di log visualizzati per evitare problemi di performance
    void Update()
    {
        if (myLogQueue.Count > 50)
        {
            myLogQueue.Dequeue();
        }
    }
}
