using UnityEngine;
using UnityEngine.UI;

public class OnScreenDebug : MonoBehaviour
{
    public Text debugText; // Assign this in the Inspector with a UI Text element.
    private string logMessages = "";

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    // Append log messages to our string and update the Text element.
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        logMessages = logString + "\n" + logMessages;

        // Limit the number of characters to prevent performance issues.
        if (logMessages.Length > 1000)
        {
            logMessages = logMessages.Substring(0, 1000);
        }

        if (debugText != null)
        {
            debugText.text = logMessages;
        }
    }
}
