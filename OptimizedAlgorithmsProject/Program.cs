using System;

class Program
{
    static void Main()
    {
        Console.WriteLine("=================================");
        Console.WriteLine("OPTIMIZED ALGORITHMS PROJECT");
        Console.WriteLine("=================================\n");

        // -----------------------------------------
        // 1. AVL TREE
        // -----------------------------------------

        Console.WriteLine("1. OPTIMIZED BINARY TREE");
        Console.WriteLine("---------------------------------");

        AvlTree tree = new AvlTree();

        int[] values =
        {
            50, 30, 70, 20, 40, 60, 80,
            10, 25, 35, 45
        };

        foreach (int value in values)
            tree.Insert(value);

        Console.Write("In-order traversal: ");
        tree.InOrder();

        Console.WriteLine(
            $"Search 40: {tree.Search(40)}"
        );

        Console.WriteLine(
            $"Search 100: {tree.Search(100)}"
        );

        // -----------------------------------------
        // 2. TASK SCHEDULING
        // -----------------------------------------

        Console.WriteLine("\n2. OPTIMIZED TASK SCHEDULING");
        Console.WriteLine("---------------------------------");

        TaskScheduler scheduler = new TaskScheduler();

        scheduler.AddTask("Backup Database", 3);
        scheduler.AddTask("Security Check", 1);
        scheduler.AddTask("Generate Report", 4);
        scheduler.AddTask("Update Server", 2);

        scheduler.ExecuteTasks();

        // -----------------------------------------
        // 3. OPTIMIZED SORTING
        // -----------------------------------------

        Console.WriteLine("\n3. OPTIMIZED SORTING");
        Console.WriteLine("---------------------------------");

        int[] numbers =
        {
            64, 34, 25, 12, 22, 11, 90, 5
        };

        Console.Write("Before sorting: ");

        foreach (int number in numbers)
            Console.Write(number + " ");

        Console.WriteLine();

        OptimizedSorter.MergeSort(numbers);

        Console.Write("After sorting:  ");

        foreach (int number in numbers)
            Console.Write(number + " ");

        Console.WriteLine();

        // -----------------------------------------
        // 4. DEBUGGED TASK EXECUTION
        // -----------------------------------------

        Console.WriteLine("\n4. DEBUGGED TASK EXECUTION");
        Console.WriteLine("---------------------------------");

        TaskExecutor executor = new TaskExecutor();

        executor.Execute(
            "Successful Task",
            () =>
            {
                Console.WriteLine("Task is running...");
            }
        );

        executor.Execute(
            "Calculation Task",
            () =>
            {
                int result = 100 / 10;

                Console.WriteLine(
                    $"Calculation result: {result}"
                );
            }
        );

        executor.Execute(
            "Failing Task",
            () =>
            {
                throw new InvalidOperationException(
                    "Simulated task failure."
                );
            }
        );

        Console.WriteLine("\n=================================");
        Console.WriteLine("PROJECT COMPLETED");
        Console.WriteLine("=================================");
    }
}