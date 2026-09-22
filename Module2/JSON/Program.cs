using Newtonsoft.Json;
using System;
using System.Xml;

public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

class Program
{
    static void Main()
    {
        Person person = new Person { Name = "Sanja", Age = 25 };

        // Serialize object to JSON string
        string json = JsonConvert.SerializeObject(person);

        Console.WriteLine(json);
    }
}