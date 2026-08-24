using System;
using System.IO;

public class TaskExecutor
{
    private readonly string logFile = "task_execution.log";

    public void Execute(string taskName, Action action)
    {
        if (string.IsNullOrWhiteSpace(taskName))
        {
            Log("Task execution failed: task name is empty.");
            return;
        }

        if (action == null)
        {
            Log($"Task execution failed: {taskName} has no action.");
            return;
        }

        try
        {
            Log($"Starting task: {taskName}");

            action();

            Log($"Task completed successfully: {taskName}");
        }
        catch (ArgumentException ex)
        {
            Log($"Argument error in {taskName}: {ex.Message}");
        }
        catch (InvalidOperationException ex)
        {
            Log($"Invalid operation in {taskName}: {ex.Message}");
        }
        catch (Exception ex)
        {
            Log($"Unexpected error in {taskName}: {ex.Message}");
        }
    }

    private void Log(string message)
    {
        string entry =
            $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

        Console.WriteLine(entry);

        try
        {
            File.AppendAllText(
                logFile,
                entry + Environment.NewLine
            );
        }
        catch (IOException)
        {
            Console.WriteLine(
                "Warning: Unable to write to log file."
            );
        }
    }
}