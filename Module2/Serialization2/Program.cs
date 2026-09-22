using System;
using System.IO;
using System.Xml.Serialization;   // Step 1: Add this

class Program
{
    static void Main(string[] args)
    {
        // Create Person object
        Person person = new Person
        {
            FirstName = "John",
            LastName = "Doe",
            Age = 30
        };

        // Step 2: Instantiate XmlSerializer for Person class
        XmlSerializer serializer = new XmlSerializer(typeof(Person));

        // Step 3: Serialize and save to XML file
        using (FileStream fs = new FileStream("person.xml", FileMode.Create))
        {
            serializer.Serialize(fs, person);
        }

        // Step 4: Confirmation message
        Console.WriteLine("Person object successfully serialized to person.xml");
    }
}
