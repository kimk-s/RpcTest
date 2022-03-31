using System;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private readonly ConcurrentQueue<string> _messagesForUI = new();

    public TMP_Text text;

    private void Update()
    {
        if (_messagesForUI.TryDequeue(out var message))
        {
            text.text += Environment.NewLine + message;
        }
    }

    public void Log(string message)
    {
        Debug.Log(message);

        _messagesForUI.Enqueue(message);
    }
}
