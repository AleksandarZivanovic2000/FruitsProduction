using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication8;

[ApiController]
[Route("api/[controller]")]
public class HarvestController : ControllerBase
{
    // "Fake database" 
    private static List<HarvestLot> HarvestLots = new List<HarvestLot>()
    {
        new HarvestLot { Id = 1, LotCode = "A001", ProduceType = "Apple", Status = "Approved" }
    };

    // 1. GET 
    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(HarvestLots);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška: {ex.Message}");
        }
    }

    // 2. GET by ID
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        try
        {
            var lot = HarvestLots.FirstOrDefault(x => x.Id == id);

            if (lot == null)
                return NotFound("Lot ne postoji");

            return Ok(lot);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška: {ex.Message}");
        }
    }

    // 3. POST 
    [HttpPost]
    public IActionResult Create([FromBody] HarvestLot lot)
    {
        try
        {
            lot.Id = HarvestLots.Count > 0 ? HarvestLots.Max(x => x.Id) + 1 : 1;
            HarvestLots.Add(lot);

            return Ok(lot);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška: {ex.Message}");
        }
    }

    // 4. PUT 
    [HttpPut("inspect/{id}")]
    public IActionResult Inspect(int id, [FromBody] int score)
    {
        try
        {
            var lot = HarvestLots.FirstOrDefault(x => x.Id == id);

            if (lot == null)
                return NotFound("Lot ne postoji");

            lot.Status = score > 70 ? "Approved" : "Quarantine";

            return Ok(lot);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška: {ex.Message}");
        }
    }

    // 5. DELETE
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var lot = HarvestLots.FirstOrDefault(x => x.Id == id);

            if (lot == null)
                return NotFound("Lot ne postoji");

            HarvestLots.Remove(lot);

            return Ok("Uspešno obrisano");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška: {ex.Message}");
        }
    }

    // 6. Dashboard
    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        try
        {
            var result = HarvestLots.Select(x => new
            {
                x.LotCode,
                x.Status
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Greška: {ex.Message}");
        }
    }
}