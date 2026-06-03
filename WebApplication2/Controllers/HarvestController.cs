using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/harvest")]
    public class HarvestController : ControllerBase
    {
        private static List<Dictionary<string, object>> HarvestLots = new();
        private static int currentId = 1;

        [HttpPost]
        public IActionResult CreateHarvest([FromBody] HarvestRequestDto request)
        {
            try
            {
                // ==============================
                // 1. NULL REQUEST
                // ==============================
                if (request == null)
                    throw new ArgumentNullException(nameof(request), "Request cannot be null.");

                // ==============================
                // 2. String
                // ==============================
                if (string.IsNullOrWhiteSpace(request.LotCode))
                    throw new ArgumentException("LotCode is required.");

                if (string.IsNullOrWhiteSpace(request.Shift))
                    throw new ArgumentException("Shift is required.");

                if (string.IsNullOrWhiteSpace(request.PickerTeam))
                    throw new ArgumentException("PickerTeam is required.");

                if (string.IsNullOrWhiteSpace(request.UnitCode))
                    throw new ArgumentException("UnitCode is required.");

                // ==============================
                // 3. Quantity
                // ==============================
                if (request.Quantity <= 0)
                    throw new ArgumentOutOfRangeException(nameof(request.Quantity), "Quantity must be greater than 0.");

                // ==============================
                // 4. LOT CODE
                // ==============================
                if (HarvestLots.Any(l => l.ContainsKey("LotCode") && l["LotCode"].ToString() == request.LotCode))
                    throw new InvalidOperationException("Lot with the same LotCode already exists.");

                // ==============================
                // 5. FORMAT / PARSING PROBLEM 
                // ==============================
                decimal quantityCheck;
                if (!decimal.TryParse(request.Quantity.ToString(), out quantityCheck))
                    throw new FormatException("Invalid quantity format.");

                // ==============================
                // 6. Lot Create
                // ==============================
                var lot = new Dictionary<string, object>
                {
                    ["Id"] = currentId++,
                    ["LotCode"] = request.LotCode,
                    ["Shift"] = request.Shift,
                    ["PickerTeam"] = request.PickerTeam,
                    ["Quantity"] = request.Quantity,
                    ["UnitCode"] = request.UnitCode,
                    ["Status"] = "Created",
                    ["CreatedAt"] = DateTime.UtcNow
                };

                HarvestLots.Add(lot);

                return Ok(new
                {
                    message = "Harvest lot created",
                    data = lot
                });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"Null error: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"Validation error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict($"Conflict: {ex.Message}");
            }
            catch (FormatException ex)
            {
                return BadRequest($"Format error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error: {ex.Message}");
            }
        }

        // ==============================
        // GET ALL
        // ==============================
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(HarvestLots);
        }
    }
}