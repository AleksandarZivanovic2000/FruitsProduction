using System;
using System.Linq;

namespace FruitsProduction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== HARVEST MANAGEMENT SYSTEM ===");
                Console.WriteLine("1 - Insert Harvest");
                Console.WriteLine("2 - Read Harvest Lots");
                Console.WriteLine("3 - Delete Harvest Lot");
                Console.WriteLine("0 - Exit");
                Console.Write("\nSelect option: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        InsertHarvest();
                        break;

                    case "2":
                        ReadHarvestLots();
                        break;

                    case "3":
                        DeleteHarvestLot();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.ReadLine();
            }
        }

        // ==============================
        // INSERT
        // ==============================
        static void InsertHarvest()
        {
            try
            {
                Console.Write("Lot Code: ");
                string lotCode = Console.ReadLine();

                Console.Write("Shift: ");
                string shift = Console.ReadLine();

                Console.Write("Picker Team: ");
                string pickerTeam = Console.ReadLine();

                Console.Write("Quantity: ");
                decimal quantity = decimal.Parse(Console.ReadLine());

                Console.Write("Unit Code (e.g. KG): ");
                string unitCode = Console.ReadLine();

                ExecuteHarvestIntake(
                    lotCode,
                    DateTime.Today,
                    shift,
                    pickerTeam,
                    quantity,
                    unitCode,
                    "Manual entry"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nERROR: " + ex.Message);
            }
        }

        // ==============================
        // CORE BUSINESS LOGIC
        // ==============================
        static void ExecuteHarvestIntake(
            string lotCode,
            DateTime harvestDate,
            string shift,
            string pickerTeam,
            decimal grossQuantity,
            string unitCode,
            string notes)
        {
            using (var db = new ProductionProduceDbEntities())
            {
                Console.WriteLine("\nSTEP 1: VALIDATION");

                var cropCycle = db.CropCycles.FirstOrDefault();
                if (cropCycle == null)
                    throw new Exception("No CropCycle exists.");

                if (db.HarvestLots.Any(x => x.LotCode == lotCode))
                    throw new Exception("Duplicate LotCode.");

                if (grossQuantity <= 0)
                    throw new Exception("Quantity must be greater than zero.");

                if (harvestDate > DateTime.Today)
                    throw new Exception("Harvest date cannot be in future.");

                var unit = db.UnitOfMeasures
                    .FirstOrDefault(x => x.Code == unitCode);

                if (unit == null)
                    throw new Exception("Unit does not exist.");

                var status = db.LotStatus
                    .FirstOrDefault(x => x.Code == "Created");

                if (status == null)
                    throw new Exception("LotStatus 'Created' missing.");

                var movementType = db.InventoryMovementTypes
                    .FirstOrDefault(x => x.Code == "HarvestIn");

                if (movementType == null)
                    throw new Exception("MovementType 'HarvestIn' missing.");

                Console.WriteLine("Validation PASSED.");

                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("\nSTEP 2: INSERTING DATA");

                        var harvestLot = new HarvestLot
                        {
                            HarvestLotGuid = Guid.NewGuid(),
                            CropCycleId = cropCycle.CropCycleId,
                            LotCode = lotCode,
                            HarvestDate = harvestDate,
                            ShiftCode = shift,
                            PickerTeam = pickerTeam,
                            GrossQuantity = grossQuantity,
                            UnitOfMeasureId = unit.UnitOfMeasureId,
                            LotStatusId = status.LotStatusId,
                            Notes = notes
                        };

                        db.HarvestLots.Add(harvestLot);
                        db.SaveChanges();

                        var movement = new InventoryMovement
                        {
                            HarvestLotId = harvestLot.HarvestLotId,
                            Quantity = grossQuantity,
                            UnitOfMeasureId = unit.UnitOfMeasureId,
                            MovementTypeId = movementType.InventoryMovementTypeId,
                            Notes = "Initial harvest intake"
                        };

                        db.InventoryMovements.Add(movement);

                        var audit = new AuditLog
                        {
                            OccurredAt = DateTime.Now,
                            Actor = "ConsoleApp",
                            Action = "INSERT",
                            EntityName = "HarvestLot",
                            EntityKey = harvestLot.HarvestLotId.ToString()
                        };

                        db.AuditLogs.Add(audit);

                        db.SaveChanges();
                        transaction.Commit();

                        Console.WriteLine("Transaction COMMITTED.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Transaction failed: " + ex.Message);
                    }
                }

                Console.WriteLine("\nSUCCESS: Harvest intake completed.");
            }
        }

        // ==============================
        // READ
        // ==============================
        static void ReadHarvestLots()
        {
            using (var db = new ProductionProduceDbEntities())
            {
                var lots = db.HarvestLots
                    .OrderByDescending(x => x.HarvestDate)
                    .ToList();

                Console.WriteLine("\n=== HARVEST LOTS ===\n");

                foreach (var lot in lots)
                {
                    Console.WriteLine($"ID: {lot.HarvestLotId}");
                    Console.WriteLine($"LotCode: {lot.LotCode}");
                    Console.WriteLine($"Date: {lot.HarvestDate:d}");
                    Console.WriteLine($"Quantity: {lot.GrossQuantity}");
                    Console.WriteLine("----------------------------");
                }
            }
        }

        // ==============================
        // DELETE
        // ==============================
        static void DeleteHarvestLot()
        {
            Console.Write("Enter LotCode to delete: ");
            string lotCode = Console.ReadLine();

            using (var db = new ProductionProduceDbEntities())
            {
                var lot = db.HarvestLots
                    .FirstOrDefault(x => x.LotCode == lotCode);

                if (lot == null)
                {
                    Console.WriteLine("Lot not found.");
                    return;
                }

                db.HarvestLots.Remove(lot);
                db.SaveChanges();

                Console.WriteLine("Lot deleted successfully.");
            }
        }
    }
}