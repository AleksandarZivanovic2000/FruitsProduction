using System;

class Program
{
    static void Main()
    {
        // Step 1: Create variables for books
        string book1 = "";
        string book2 = "";
        string book3 = "";
        string book4 = "";
        string book5 = "";

        bool running = true;

        while (running)
        {
            Console.WriteLine("\nLibrary Management System");
            Console.WriteLine("Choose an action: add, remove, display, exit");
            string action = Console.ReadLine()?.ToLower();

            if (action == "add")
            {
                // Step 2: Add a book
                Console.Write("Enter the title of the book to add: ");
                string newBook = Console.ReadLine();

                if (string.IsNullOrEmpty(book1)) book1 = newBook;
                else if (string.IsNullOrEmpty(book2)) book2 = newBook;
                else if (string.IsNullOrEmpty(book3)) book3 = newBook;
                else if (string.IsNullOrEmpty(book4)) book4 = newBook;
                else if (string.IsNullOrEmpty(book5)) book5 = newBook;
                else Console.WriteLine("No more space to add books. Library is full.");
            }
            else if (action == "remove")
            {
                // Step 3: Remove a book
                Console.Write("Enter the title of the book to remove: ");
                string removeBook = Console.ReadLine();

                if (book1 == removeBook) book1 = "";
                else if (book2 == removeBook) book2 = "";
                else if (book3 == removeBook) book3 = "";
                else if (book4 == removeBook) book4 = "";
                else if (book5 == removeBook) book5 = "";
                else Console.WriteLine("Book not found in the library.");
            }
            else if (action == "display")
            {
                // Step 4: Display the list of books
                Console.WriteLine("\nBooks currently in the library:");
                if (!string.IsNullOrEmpty(book1)) Console.WriteLine(book1);
                if (!string.IsNullOrEmpty(book2)) Console.WriteLine(book2);
                if (!string.IsNullOrEmpty(book3)) Console.WriteLine(book3);
                if (!string.IsNullOrEmpty(book4)) Console.WriteLine(book4);
                if (!string.IsNullOrEmpty(book5)) Console.WriteLine(book5);

                if (string.IsNullOrEmpty(book1) && string.IsNullOrEmpty(book2) &&
                    string.IsNullOrEmpty(book3) && string.IsNullOrEmpty(book4) &&
                    string.IsNullOrEmpty(book5))
                {
                    Console.WriteLine("No books available.");
                }
            }
            else if (action == "exit")
            {
                // Step 5: Exit the program
                running = false;
                Console.WriteLine("Exiting the library system. Goodbye!");
            }
            else
            {
                // Step 6: Handle invalid inputs
                Console.WriteLine("Invalid action. Please choose add, remove, display, or exit.");
            }
        }
    }
}
