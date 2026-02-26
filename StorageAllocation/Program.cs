using System;
using System.Linq;
using StorageAllocation;

namespace FruitsProduction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== STORAGE MANAGEMENT SYSTEM ===");
                Console.WriteLine("1 - View Unallocated Lots");
                Console.WriteLine("2 - Allocate Lot To Bin");
                Console.WriteLine("3 - Log Storage Condition");
                Console.WriteLine("4 - View Storage Dashboard");
                Console.WriteLine("0 - Exit");
                Console.Write("\nSelect option: ");

                var choice = Console.ReadLine();

                try
                {
                    switch (choice)
                    {
                        case "1":
                            ViewUnallocatedLots();
                            break;
                        case "2":
                            AllocateLotMenu();
                            break;
                        case "3":
                            LogConditionMenu();
                            break;
                        case "4":
                            ViewStorageDashboard();
                            break;
                        case "0":
                            return;
                        default:
                            Console.WriteLine("Invalid option.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\nERROR: " + ex.Message);
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();
            }
        }

        // ======================================
        // VIEW UNALLOCATED LOTS
        // ======================================
        static void ViewUnallocatedLots()
        {
            using (var db = new ProductionProduceDbEntities())
            {
                var lots = db.HarvestLots.ToList();

                Console.WriteLine("\n=== UNALLOCATED LOTS ===\n");

                foreach (var lot in lots)
                {
                    var hasTransfer = db.InventoryMovements
                        .Any(m => m.HarvestLotId == lot.HarvestLotId &&
                                  m.ToStorageBinId != null);

                    if (!hasTransfer)
                    {
                        Console.WriteLine($"ID: {lot.HarvestLotId}");
                        Console.WriteLine($"LotCode: {lot.LotCode}");
                        Console.WriteLine($"Date: {lot.HarvestDate:d}");
                        Console.WriteLine($"Quantity: {lot.GrossQuantity}");
                        Console.WriteLine("---------------------------");
                    }
                }
            }
        }

        // ======================================
        // ALLOCATE LOT MENU
        // ======================================
        static void AllocateLotMenu()
        {
            Console.Write("HarvestLotId: ");
            int lotId = int.Parse(Console.ReadLine());

            Console.Write("ToStorageBinId: ");
            int binId = int.Parse(Console.ReadLine());

            Console.Write("Quantity: ");
            decimal quantity = decimal.Parse(Console.ReadLine());

            AllocateLot(lotId, binId, quantity);
        }

        // ======================================
        // ALLOCATION LOGIC
        // ======================================
        static void AllocateLot(int harvestLotId, int toBinId, decimal quantity)
        {
            using (var db = new ProductionProduceDbEntities())
            {
                Console.WriteLine("\nSTEP 1: VALIDATION");

                if (quantity <= 0)
                    throw new Exception("Quantity must be greater than zero.");

                var lot = db.HarvestLots.Find(harvestLotId);
                if (lot == null)
                    throw new Exception("Harvest lot not found.");

                var bin = db.StorageBins.Find(toBinId);
                if (bin == null)
                    throw new Exception("Storage bin not found.");

                decimal available = GetBinAvailableCapacityKg(toBinId);

                if (quantity > available)
                    throw new Exception("Bin capacity exceeded.");

                var transferType = db.InventoryMovementTypes
                    .FirstOrDefault(x => x.Code == "Transfer");

                if (transferType == null)
                    throw new Exception("Transfer movement type missing.");

                Console.WriteLine("Validation PASSED.");

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var movement = new InventoryMovement
                        {
                            HarvestLotId = harvestLotId,
                            MovementTypeId = transferType.InventoryMovementTypeId,
                            FromStorageBinId = null,
                            ToStorageBinId = toBinId,
                            Quantity = quantity,
                            Notes = "Manual allocation"
                        };

                        db.InventoryMovements.Add(movement);

                        var audit = new AuditLog
                        {
                            OccurredAt = DateTime.Now,
                            Actor = "ConsoleApp",
                            Action = "ALLOCATE",
                            EntityName = "HarvestLot",
                            EntityKey = harvestLotId.ToString()
                        };

                        db.AuditLogs.Add(audit);

                        db.SaveChanges();
                        transaction.Commit();

                        Console.WriteLine("Allocation successful.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Allocation failed: " + ex.Message);
                    }
                }
            }
        }

        // ======================================
        // CAPACITY CHECK (ISPRAVLJENO)
        // ======================================
        static decimal GetBinAvailableCapacityKg(int binId)
        {
            using (var db = new ProductionProduceDbEntities())
            {
                var bin = db.StorageBins.Find(binId);

                if (bin == null)
                    throw new Exception("Bin not found.");

                var totalIn = db.InventoryMovements
                    .Where(m => m.ToStorageBinId == binId)
                    .Sum(m => (decimal?)m.Quantity) ?? 0;

                var totalOut = db.InventoryMovements
                    .Where(m => m.FromStorageBinId == binId)
                    .Sum(m => (decimal?)m.Quantity) ?? 0;

                var currentStock = totalIn - totalOut;

                
                return currentStock;
            }
        }

        // ======================================
        // LOG CONDITION MENU
        // ======================================
        static void LogConditionMenu()
        {
            Console.Write("StorageZoneId: ");
            int zoneId = int.Parse(Console.ReadLine());

            Console.Write("Temperature: ");
            decimal temp = decimal.Parse(Console.ReadLine());

            Console.Write("Humidity: ");
            decimal humidity = decimal.Parse(Console.ReadLine());

            LogStorageCondition(zoneId, temp, humidity);
        }

        // ======================================
        // STORAGE CONDITION LOGIC
        // ======================================
        static void LogStorageCondition(int zoneId, decimal temp, decimal humidity)
        {
            using (var db = new ProductionProduceDbEntities())
            {
                var zone = db.StorageZones.Find(zoneId);

                if (zone == null)
                    throw new Exception("Zone not found.");

                var log = new StorageConditionLog
                {
                    StorageZoneId = zoneId,
                    LoggedAt = DateTime.Now,
                    TemperatureC = temp,
                    HumidityPct = humidity,
                    Notes = "Manual entry"
                };

                db.StorageConditionLogs.Add(log);
                db.SaveChanges();

                Console.WriteLine("Condition logged successfully.");
            }
        }

        // ======================================
        // STORAGE DASHBOARD
        // ======================================
        static void ViewStorageDashboard()
        {
            using (var db = new ProductionProduceDbEntities())
            {
                var zones = db.StorageZones.ToList();

                Console.WriteLine("\n=== STORAGE DASHBOARD ===\n");

                foreach (var zone in zones)
                {
                    var latest = db.StorageConditionLogs
                        .Where(x => x.StorageZoneId == zone.StorageZoneId)
                        .OrderByDescending(x => x.LoggedAt)
                        .FirstOrDefault();

                    if (latest == null)
                        continue;

                    bool tempOk =
                        latest.TemperatureC >= zone.TargetTempMinC &&
                        latest.TemperatureC <= zone.TargetTempMaxC;

                    bool humidityOk =
                        latest.HumidityPct >= zone.TargetHumidityMinPct &&
                        latest.HumidityPct <= zone.TargetHumidityMaxPct;

                    string status = (tempOk && humidityOk) ? "OK" : "ALERT";

                    Console.WriteLine($"Zone: {zone.Name}");
                    Console.WriteLine($"Temp: {latest.TemperatureC}");
                    Console.WriteLine($"Humidity: {latest.HumidityPct}");
                    Console.WriteLine($"STATUS: {status}");
                    Console.WriteLine("---------------------------");

                    if (status == "ALERT")
                    {
                        db.AuditLogs.Add(new AuditLog
                        {
                            OccurredAt = DateTime.Now,
                            Actor = "System",
                            Action = "ALERT",
                            EntityName = "StorageZone",
                            EntityKey = zone.StorageZoneId.ToString()
                        });

                        db.SaveChanges();
                    }
                }
            }
        }
    }
}