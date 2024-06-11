using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

public class SourceLogger : MonoBehaviour
{
    private static SourceLogger instance;
    private static readonly object padlock = new object();

    private string filePath;
    private ConcurrentQueue<string> logQueue;
    private Thread logThread;
    private bool isLogging;

    public static SourceLogger Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    // Find the logger in the scene
                    instance = FindObjectOfType<SourceLogger>();

                    // If no logger is found, create a new GameObject and attach the logger
                    if (instance == null)
                    {
                        GameObject loggerObject = new GameObject("SourceLogger");
                        instance = loggerObject.AddComponent<SourceLogger>();
                    }
                }
                return instance;
            }
        }
    }

    void Awake()
    {
        // If an instance already exists and it's not this, destroy this instance
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance to this instance
        instance = this;

        // Prevent this object from being destroyed when loading a new scene
        DontDestroyOnLoad(gameObject);

        InitializeLogger();
    }

    private void InitializeLogger()
    {
        // Define the file path
        filePath = Path.Combine(Application.dataPath, "sourceLog.txt");

        // Initialize the log queue
        logQueue = new ConcurrentQueue<string>();

        // Start the logging thread
        isLogging = true;
        logThread = new Thread(ProcessLogQueue);
        logThread.Start();
    }

    void OnDestroy()
    {
        // Stop the logging thread
        isLogging = false;
        logThread?.Join();  // Wait for the logging thread to finish

        // Process any remaining logs
        ProcessLogQueue();
    }

    // Method to log a message to the file
    public void Log(string message)
    {
        string logEntry = $"{DateTime.Now:hh:mm:ss.fff}, {message}";
        logQueue.Enqueue(logEntry);
    }

    // Method to process the log queue
    private void ProcessLogQueue()
    {
        while (isLogging || !logQueue.IsEmpty)
        {
            if (logQueue.TryDequeue(out string logEntry))
            {
                // Write the log entry to the file
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(logEntry);
                }
            }

            // Sleep for a short time to prevent busy-waiting
            Thread.Sleep(50);
        }
    }
}