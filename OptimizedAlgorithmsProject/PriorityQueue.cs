using System;
using System.Collections.Generic;
using System.Linq;

public class TaskItem
{
    public string Name { get; set; }
    public int Priority { get; set; }

    public TaskItem(string name, int priority)
    {
        Name = name;
        Priority = priority;
    }
}

public class TaskScheduler
{
    private readonly List<TaskItem> tasks = new List<TaskItem>();

    public void AddTask(string name, int priority)
    {
        tasks.Add(new TaskItem(name, priority));
    }

    public void ExecuteTasks()
    {
        // Sort by priority before execution
        tasks.Sort((a, b) => a.Priority.CompareTo(b.Priority));

        while (tasks.Count > 0)
        {
            TaskItem task = tasks[0];
            tasks.RemoveAt(0);

            try
            {
                Console.WriteLine(
                    $"Executing: {task.Name} | Priority: {task.Priority}"
                );

                Console.WriteLine(
                    $"Completed: {task.Name}"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Error executing {task.Name}: {ex.Message}"
                );
            }
        }
    }
}