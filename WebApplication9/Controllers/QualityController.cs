using Microsoft.AspNetCore.Mvc;
using WebApplication1;

[ApiController]
[Route("api/quality")]
public class QualityController : ControllerBase
{
    private static List<Dictionary<string, object>> HarvestLots = new();
    private static List<Dictionary<string, object>> Inspections = new();

    [HttpPost("inspect")]
    public IActionResult InspectLot([FromBody] InspectLotRequest request)
    {
        var lot = HarvestLots.FirstOrDefault(l => (int)l["Id"] == request.LotId);
        if (lot == null)
            return NotFound("Lot not found");

        string status = (request.Score < 70 || request.Defect > 15) ? "Failed" : "Passed";

        Inspections.Add(new Dictionary<string, object>
        {
            ["HarvestLotId"] = request.LotId,
            ["VisualScore"] = request.Score,
            ["DefectPct"] = request.Defect,
            ["Status"] = status
        });

        if (status == "Failed")
        {
            lot["Status"] = "Quarantine";
        }

        return Ok("Inspection saved");
    }
}