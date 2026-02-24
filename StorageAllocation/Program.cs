using System;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString =
        "Server=.;Database=StorageDB;Trusted_Connection=True;TrustServerCertificate=True;";

    static void Main()
    {
        Console.WriteLine("Testing connection...");

        TestConnection();

        while (true)
        {
            Console.WriteLine("\n1 - Show Harvest Lots");
            Console.WriteLine("2 - Check Bin Capacity");
            Console.WriteLine("0 - Exit");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ShowHarvestLots();
                    break;

                case "2":
                    CheckCapacity();
                    break;

                case "0":
                    return;
            }
        }
    }

    // ===============================
    // TEST CONNECTION
    // ===============================
    static void TestConnection()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();
            Console.WriteLine("Database connected successfully.");
        }
    }

    // ===============================
    // SHOW HARVEST LOTS
    // ===============================
    static void ShowHarvestLots()
    {
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            string query = "SELECT Id, LotCode, GrossQuantity FROM HarvestLots";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Console.WriteLine("\n--- HARVEST LOTS ---");

                    while (reader.Read())
                    {
                        Console.WriteLine(
                            "ID: " + reader["Id"] +
                            " | Code: " + reader["LotCode"] +
                            " | Qty: " + reader["GrossQuantity"]);
                    }
                }
            }
        }
    }

    // ===============================
    // CHECK BIN CAPACITY
    // ===============================
    static void CheckCapacity()
    {
        Console.Write("Enter Bin Id: ");
        int binId = int.Parse(Console.ReadLine());

        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            conn.Open();

            decimal capacity = 0;

            string capQuery = "SELECT CapacityKg FROM StorageBins WHERE Id = @BinId";

            using (SqlCommand cmd = new SqlCommand(capQuery, conn))
            {
                cmd.Parameters.AddWithValue("@BinId", binId);
                capacity = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            decimal currentStock = 0;

            string stockQuery =
                "SELECT ISNULL(SUM(CASE WHEN ToStorageBinId = @BinId THEN Quantity ELSE 0 END),0) -" +
                "ISNULL(SUM(CASE WHEN FromStorageBinId = @BinId THEN Quantity ELSE 0 END),0) " +
                "FROM InventoryMovements";

            using (SqlCommand cmd = new SqlCommand(stockQuery, conn))
            {
                cmd.Parameters.AddWithValue("@BinId", binId);
                currentStock = Convert.ToDecimal(cmd.ExecuteScalar());
            }

            decimal available = capacity - currentStock;

            Console.WriteLine("Capacity: " + capacity);
            Console.WriteLine("Current stock: " + currentStock);
            Console.WriteLine("Available: " + available);
        }
    }
}