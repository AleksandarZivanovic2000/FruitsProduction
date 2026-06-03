using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.DTOs;
namespace WebApplication3.Api.Controllers
{
    [ApiController]
    [Route("api/storage")]
    public class StorageController : ControllerBase
    {
        // ======================================
        // IN-MEMORY "DATABASE"
        // ======================================
        private static List<HarvestLot> lots = new()
        {
            new HarvestLot { Id = 1, LotCode = "A1", Quantity = 100 },
            new HarvestLot { Id = 2, LotCode = "B2", Quantity = 200 }
        };

        private static List<InventoryMovement> movements = new();
        private static List<StorageZone> zones = new()
        {
            new StorageZone { Id = 1, Name = "Zone A", TempMin = 2, TempMax = 8, HumMin = 60, HumMax = 80 }
        };

        private static List<StorageConditionLog> logs = new();

        // ======================================
        // 1. UNALLOCATED LOTS
        // ======================================
        [HttpGet("unallocated-lots")]
        public IActionResult GetUnallocatedLots()
        {
            try
            {
                var allocatedLotIds = movements
                    .Where(m => m.ToBinId != null)
                    .Select(m => m.LotId)
                    .Distinct()
                    .ToList();

                var result = lots
                    .Where(l => !allocatedLotIds.Contains(l.Id))
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
        }

        // ======================================
        // 2. ALLOCATE LOT
        // ======================================
        [HttpPost("allocate")]
        public IActionResult Allocate([FromBody] AllocateRequest request)
        {
            try
            {
                if (request.Quantity <= 0)
                    throw new Exception("Quantity must be > 0");

                var lot = lots.FirstOrDefault(x => x.Id == request.LotId);
                if (lot == null)
                    throw new Exception("Lot not found");

                decimal used = movements
                    .Where(m => m.ToBinId == request.BinId)
                    .Sum(m => m.Quantity);

                decimal capacity = 1000; // fixed mock capacity

                if (used + request.Quantity > capacity)
                    throw new Exception("Bin capacity exceeded");

                movements.Add(new InventoryMovement
                {
                    LotId = request.LotId,
                    ToBinId = request.BinId,
                    Quantity = request.Quantity
                });

                return Ok("Allocation successful");
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR: " + ex.Message);
            }
        }

        // ======================================
        // 3. LOG CONDITION
        // ======================================
        [HttpPost("condition")]
        public IActionResult LogCondition([FromBody] ConditionRequest request)
        {
            try
            {
                logs.Add(new StorageConditionLog
                {
                    ZoneId = request.ZoneId,
                    Temperature = request.Temperature,
                    Humidity = request.Humidity,
                    LoggedAt = DateTime.Now
                });

                return Ok("Condition logged");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ======================================
        // 4. DASHBOARD
        // ======================================
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            try
            {
                var result = new List<object>();

                foreach (var zone in zones)
                {
                    var latest = logs
                        .Where(l => l.ZoneId == zone.Id)
                        .OrderByDescending(l => l.LoggedAt)
                        .FirstOrDefault();

                    if (latest == null) continue;

                    bool ok =
                        latest.Temperature >= zone.TempMin &&
                        latest.Temperature <= zone.TempMax &&
                        latest.Humidity >= zone.HumMin &&
                        latest.Humidity <= zone.HumMax;

                    result.Add(new
                    {
                        zone.Name,
                        latest.Temperature,
                        latest.Humidity,
                        Status = ok ? "OK" : "ALERT"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}






