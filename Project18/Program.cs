using System;
using System.Collections.Generic;

class LibraryManagementSystem
{
    // Dictionary to store books and their checked-out status
    private Dictionary<string, bool> books = new Dictionary<string, bool>();
    private List<string> borrowedBooks = new List<string>();
    private const int borrowLimit = 3;

    public LibraryManagementSystem()
    {
        // Initialize with some books
        books["The Hobbit"] = false;
        books["1984"] = false;
        books["To Kill a Mockingbird"] = false;
        books["The Great Gatsby"] = false;
    }

    // Search feature
    public void SearchBook()
    {
        Console.Write("Enter book title to search: ");
        string title = Console.ReadLine();

        if (books.ContainsKey(title))
        {
            if (!books[title])
                Console.WriteLine($"{title} is available.");
            else
                Console.WriteLine($"{title} is currently checked out.");
        }
        else
        {
            Console.WriteLine($"{title} is not in the collection.");
        }
    }

    // Borrow feature with limit
    public void BorrowBook()
    {
        if (borrowedBooks.Count >= borrowLimit)
        {
            Console.WriteLine("Borrowing limit reached. Return a book first.");
            return;
        }

        Console.Write("Enter book title to borrow: ");
        string title = Console.ReadLine();

        if (books.ContainsKey(title))
        {
            if (!books[title])
            {
                books[title] = true; // mark as checked out
                borrowedBooks.Add(title);
                Console.WriteLine($"{title} has been borrowed.");
            }
            else
            {
                Console.WriteLine($"{title} is already checked out.");
            }
        }
        else
        {
            Console.WriteLine($"{title} is not in the collection.");
        }
    }

    // Return feature (remove checked-out flag)
    public void ReturnBook()
    {
        Console.Write("Enter book title to return: ");
        string title = Console.ReadLine();

        if (borrowedBooks.Contains(title))
        {
            books[title] = false; // mark as available
            borrowedBooks.Remove(title);
            Console.WriteLine($"{title} has been returned.");
        }
        else
        {
            Console.WriteLine($"You have not borrowed {title}.");
        }
    }

    // Menu
    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\nLibrary Menu:");
            Console.WriteLine("1. Search for a book");
            Console.WriteLine("2. Borrow a book");
            Console.WriteLine("3. Return a book");
            Console.WriteLine("4. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    SearchBook();
                    break;
                case "2":
                    BorrowBook();
                    break;
                case "3":
                    ReturnBook();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static void Main(string[] args)
    {
        LibraryManagementSystem library = new LibraryManagementSystem();
        library.Run();
    }
}
