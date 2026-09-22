using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


class Program
{
    static void Main()
    {
        Person person = new Person
        {
            Id = 1,
            Name = "Aleksandar",
            Age = 25
        };

        // Binary Serialization
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream fs = new FileStream("person.bin", FileMode.Create))
        {
            formatter.Serialize(fs, person);
        }

        
    }
}