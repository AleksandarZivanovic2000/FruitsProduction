using System;
using System.Data.Entity;
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
                    cropCycleId: 1,
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
                Console.WriteLine("ERROR: " + ex.Message);
            }

            Console.ReadLine();
        }

        static void ExecuteHarvestIntake(
            int cropCycleId,
            string lotCode,
            DateTime harvestDate,
            string shift,
            string pickerTeam,
            decimal grossQuantity,
            string unitCode,
            string notes)
        {
            using (var db = new ProductionProduceDbEntities())
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    // ===== 1️⃣ VALIDATION =====
                    var cropCycle = db.CropCycles.FirstOrDefault(x => x.CropCycleId == cropCycleId);
                    if (cropCycle == null)
                        throw new Exception($"CropCycle {cropCycleId} does not exist.");

                    if (db.HarvestLots.Any(x => x.LotCode == lotCode))
                        throw new Exception($"Duplicate LotCode: {lotCode}");

                    if (grossQuantity <= 0)
                        throw new Exception("Quantity must be greater than zero.");

                    if (harvestDate > DateTime.Today)
                        throw new Exception("Harvest date cannot be in the future.");

                    
                    

                    var status = db.LotStatus.FirstOrDefault(x => x.Code == "Created");
                    if (status == null)
                        throw new Exception("Lot status 'Created' not configured.");

                    

                    // ===== 2️⃣ CREATE HARVEST LOT =====
                    var harvestLot = new HarvestLot
                    {
                        CropCycleId = cropCycleId,
                        LotCode = lotCode,
                        HarvestDate = harvestDate,
                        ShiftCode = shift,
                        PickerTeam = pickerTeam,
                        GrossQuantity = grossQuantity,
                        Notes = notes,
                        
                        LotStatusId = status.LotStatusId
                    };
                    db.HarvestLots.Add(harvestLot);
                    db.SaveChanges();

                    Console.WriteLine($"HarvestLot created. ID={harvestLot.HarvestLotId}");

                    // ===== 3️⃣ CREATE INVENTORY MOVEMENT =====
                    var movement = new InventoryMovement
                    {
                        HarvestLotId = harvestLot.HarvestLotId,
                        Quantity = grossQuantity
                        
                    };
                    db.InventoryMovements.Add(movement);

                    // ===== 4️⃣ CREATE AUDIT LOG =====
                    var audit = new AuditLog
                    {
                        
                        Actor = "ConsoleApp",
                        
                        EntityName = "HarvestLot",
                        EntityKey = harvestLot.HarvestLotId.ToString(),
                        Details = $"Lot {lotCode} created with {grossQuantity} {unitCode}"
                    };
                    db.AuditLogs.Add(audit);

                    // ===== 5️⃣ COMMIT EVERYTHING =====
                    db.SaveChanges();
                    transaction.Commit();

                    // ===== 6️⃣ VERIFY BALANCE =====
                    var balance = db.InventoryMovements
                        .Where(x => x.HarvestLotId == harvestLot.HarvestLotId)
                        .Sum(x => (decimal?)x.Quantity) ?? 0;

                    Console.WriteLine($"SUCCESS: Balance={balance} {unitCode}");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine("Transaction rolled back.");
                    throw new Exception("Harvest intake failed: " + ex.Message);
                }
            }
        }
    }
}