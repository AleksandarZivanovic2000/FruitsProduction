using System;

class ToDoList
{
    // Step 2: Creating the Task List
    static string[] tasks = new string[10];
    static int taskCount = 0;

    // Step 3: Adding a Task
    static void AddTask()
    {
        if (taskCount >= tasks.Length)
        {
            Console.WriteLine("Your task list is full.");
            return;
        }

        Console.Write("Enter a new task: ");
        string task = Console.ReadLine();

        tasks[taskCount] = task;
        taskCount++;

        Console.WriteLine("Task added successfully!");
    }

    // Step 4: Viewing Tasks
    static void ViewTasks()
    {
        if (taskCount == 0)
        {
            Console.WriteLine("No tasks have been added yet.");
            return;
        }

        Console.WriteLine("\nYour Tasks:");

        for (int i = 0; i < taskCount; i++)
        {
            Console.WriteLine($"{i + 1}. {tasks[i]}");
        }
    }

    // Step 5 and 7: Marking a Task as Completed + Debugging
    static void CompleteTask()
    {
        ViewTasks();

        Console.Write("Enter the task number to mark as completed: ");
        int taskNumber = Convert.ToInt32(Console.ReadLine());

        // Debugging line
        Console.WriteLine($"Debug: taskNumber = {taskNumber}");

        if (taskNumber < 1 || taskNumber > taskCount)
        {
            Console.WriteLine("Error: That task number is out of range.");
            return;
        }

        tasks[taskNumber - 1] += " (Completed)";

        Console.WriteLine("Task marked as completed!");
    }

    // Step 6: Main Method
    static void Main(string[] args)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\nTo-Do List Menu");
            Console.WriteLine("1. Add Task");
            Console.WriteLine("2. View Tasks");
            Console.WriteLine("3. Complete Task");
            Console.WriteLine("4. Exit");

            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddTask();
                    break;

                case "2":
                    ViewTasks();
                    break;

                case "3":
                    CompleteTask();
                    break;

                case "4":
                    running = false;
                    Console.WriteLine("Goodbye!");
                    break;

                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }
}
