using System;

namespace SimpleCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Set up the console window appearance
            Console.Title = "Simple C# Calculator";
            Console.WriteLine("=== C# Console Calculator ===");
            Console.WriteLine("-----------------------------\n");

            // Request and validate the first numeric entry
            Console.Write("Enter the first number: ");
            if (!double.TryParse(Console.ReadLine(), out double num1))
            {
                Console.WriteLine("Invalid entry. Please restart and type a valid number.");
                return;
            }

            // Request the operational sign
            Console.Write("Enter an operator (+, -, *, /): ");
            string op = Console.ReadLine();

            // Request and validate the second numeric entry
            Console.Write("Enter the second number: ");
            if (!double.TryParse(Console.ReadLine(), out double num2))
            {
                Console.WriteLine("Invalid entry. Please restart and type a valid number.");
                return;
            }

            double result = 0;
            bool validOperation = true;

            // Process calculation using a switch block
            switch (op)
            {
                case "+":
                    result = num1 + num2;
                    break;
                case "-":
                    result = num1 - num2;
                    break;
                case "*":
                    result = num1 * num2;
                    break;
                case "/":
                    // Check for division by zero
                    if (num2 == 0)
                    {
                        Console.WriteLine("\nError: Cannot divide by zero.");
                        validOperation = false;
                    }
                    else
                    {
                        result = num1 / num2;
                    }
                    break;
                default:
                    Console.WriteLine("\nError: Unrecognized operation sign.");
                    validOperation = false;
                    break;
            }

            // Output the successful calculation matrix
            if (validOperation)
            {
                Console.WriteLine($"\nYour result: {num1} {op} {num2} = {result}");
            }

            // Prevent immediate console window termination
            Console.WriteLine("\nPress any key to close the app...");
            Console.ReadKey();
        }
    }
}

