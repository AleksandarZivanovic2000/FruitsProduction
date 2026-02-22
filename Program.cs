using System;
using System.Linq;

namespace FruitsProduction
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== HARVEST INTAKE PROCESS ===\n");

            try
            {
                ExecuteHarvestIntake(
                    lotCode: "HL-2026-001",
                    harvestDate: DateTime.Today,
                    shift: "Day",
                    pickerTeam: "Team A",
                    grossQuantity: 1000m,
                    unitCode: "KG",
                    notes: "Morning harvest"
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nPROCESS FAILED:");
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

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
                Console.WriteLine("STEP 1: VALIDATION\n");

                // =========================
                // 1️⃣ CropCycle (uzima prvi aktivni)
                // =========================
                var cropCycle = db.CropCycles.FirstOrDefault();

                if (cropCycle == null)
                    throw new Exception("No CropCycle exists in database.");

                Console.WriteLine($"Using CropCycle ID: {cropCycle.CropCycleId}");

                // =========================
                // 2️⃣ Duplicate LotCode
                // =========================
                if (db.HarvestLots.Any(x => x.LotCode == lotCode))
                    throw new Exception("Duplicate LotCode.");

                // =========================
                // 3️⃣ Quantity validation
                // =========================
                if (grossQuantity <= 0)
                    throw new Exception("Quantity must be greater than zero.");

                // =========================
                // 4️⃣ Date validation
                // =========================
                if (harvestDate > DateTime.Today)
                    throw new Exception("Harvest date cannot be in the future.");

                // =========================
                // 5️⃣ Unit validation
                // =========================
                var unit = db.UnitOfMeasures
                    .FirstOrDefault(x => x.Code == unitCode);

                if (unit == null)
                    throw new Exception("Unit of measure does not exist.");

                // =========================
                // 6️⃣ Status validation
                // =========================
                var status = db.LotStatus
                    .FirstOrDefault(x => x.Code == "Created");

                if (status == null)
                    throw new Exception("LotStatus 'Created' not configured.");

                // =========================
                // 7️⃣ MovementType validation
                // =========================
                var movementType = db.InventoryMovementTypes
                    .FirstOrDefault(x => x.Code == "HarvestIn");

                if (movementType == null)
                    throw new Exception("InventoryMovementType 'HarvestIn' not configured.");

                Console.WriteLine("Validation PASSED.\n");

                // =========================
                // TRANSACTION
                // =========================
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Console.WriteLine("STEP 2: BEGIN TRANSACTION\n");

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
                            Notes = notes,
                            CropCycle = cropCycle,
                        };

                        db.HarvestLots.Add(harvestLot);
                        db.SaveChanges();

                        Console.WriteLine($"HarvestLot created with ID: {harvestLot.HarvestLotId}");

                        var movement = new InventoryMovement
                        {
                            InventoryMovementId = movementType.InventoryMovementTypeId,
                            HarvestLotId = harvestLot.HarvestLotId,
                            FromStorageBinId = null,
                            ToStorageBinId = null,
                            Quantity = grossQuantity,
                            UnitOfMeasureId = unit.UnitOfMeasureId,
                            MovementTypeId = 1,
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

                        Console.WriteLine("Transaction COMMITTED.\n");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Transaction ROLLED BACK.");
                        throw new Exception("Transaction failed: " + ex.Message);
                    }
                }

                // =========================
                // VERIFY STOCK
                // =========================
                Console.WriteLine("STEP 3: VERIFY STOCK\n");

                var balance = db.InventoryMovements
                    .Where(x => x.HarvestLot.LotCode == lotCode)
                    .Sum(x => (decimal?)x.Quantity) ?? 0;

                Console.WriteLine($"Calculated Balance: {balance} {unitCode}");

                if (balance != grossQuantity)
                    throw new Exception("Balance mismatch detected.");

                Console.WriteLine("\nSUCCESS: Harvest intake completed correctly.");
            }
        }
    }
}