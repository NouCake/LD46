using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayLog : MonoBehaviour {

    private TextMeshProUGUI text;

    void Start() {
        text = GetComponent<TextMeshProUGUI>();

        Application.logMessageReceived += LogMessage;
    }

    // Update is called once per frame
    void LogMessage(string message, string stackTrace, LogType type) {
        text.text += message + "\n";
    }
}
