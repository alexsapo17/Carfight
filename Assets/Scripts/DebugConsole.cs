using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
public class DebugConsole : MonoBehaviour
{
    public Text logText; // Assicurati di assegnare un oggetto Text UI attraverso l'Inspector
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
        
        // Keep the console log limited to a certain number of lines
        while (myLogQueue.Count > 20)
        {
            myLogQueue.Dequeue();
        }

        if (logText != null)
        {
            logText.text = myLog;
        }
    }

    void Update()
    {
        // Update the UI Text object with the current log string.
        if (logText != null)
        {
            logText.text = myLog;
        }
    }
}
