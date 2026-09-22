using System;
using System.IO;
using System.Text.Json;

public class Person
{
    public string UserName { get; set; }
    public int UserAge { get; set; }
}

class Program
{
    static void Main()
    {
        Person samplePerson = new Person { UserName = "Alice", UserAge = 30 };
        string jsonString = JsonSerializer.Serialize(samplePerson);

        File.WriteAllText("person.json", jsonString);

        Console.WriteLine("JSON serialization complete.");
    }
}