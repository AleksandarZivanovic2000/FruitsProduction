using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QualityFulfillmentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityFulfillmentController : ControllerBase
    {
        private static List<Dictionary<string, object>> HarvestLots = new();
        private static List<Dictionary<string, object>> SalesOrders = new();
        private static List<Dictionary<string, object>> Shipments = new();

        // ================== 1. GET LOTS ==================
        [HttpGet("lots")]
        public IActionResult GetLots()
        {
            try
            {
                return Ok(HarvestLots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching lots: {ex.Message}");
            }
        }

        // ================== 2. ADD LOT ==================
        [HttpPost("lots")]
        public IActionResult AddLot([FromBody] Dictionary<string, object> lot)
        {
            try
            {
                lot["Id"] = HarvestLots.Count + 1;
                HarvestLots.Add(lot);

                return Ok(new { message = "Lot added", data = lot });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding lot: {ex.Message}");
            }
        }

        // ================== 3. INSPECT LOT ==================
        [HttpPost("inspect/{id}")]
        public IActionResult InspectLot(int id, [FromBody] dynamic request)
        {
            try
            {
                var lot = HarvestLots.FirstOrDefault(l => (int)l["Id"] == id);
                if (lot == null)
                    return NotFound("Lot not found");

                int score = request.visualScore;
                double defect = request.defectPct;

                string status = (score < 70 || defect > 15) ? "Failed" : "Passed";

                lot["Status"] = status;

                return Ok(new { lotId = id, status });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Inspection error: {ex.Message}");
            }
        }

        // ================== 4. CREATE ORDER ==================
        [HttpPost("orders")]
        public IActionResult CreateOrder([FromBody] dynamic request)
        {
            try
            {
                int id = SalesOrders.Count + 1;

                var order = new Dictionary<string, object>
                {
                    ["SalesOrderId"] = id,
                    ["Customer"] = request.customer.ToString(),
                    ["Status"] = "Created"
                };

                SalesOrders.Add(order);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Order creation failed: {ex.Message}");
            }
        }

        // ================== 5. RESERVE ORDER ==================
        [HttpPost("orders/{id}/reserve")]
        public IActionResult ReserveOrder(int id)
        {
            try
            {
                var order = SalesOrders.FirstOrDefault(o => (int)o["SalesOrderId"] == id);

                if (order == null)
                    return NotFound("Order not found");

                order["Status"] = "Reserved";

                return Ok(new { message = "Order reserved", order });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Reservation error: {ex.Message}");
            }
        }

        // ================== 6. CREATE SHIPMENT ==================
        [HttpPost("shipments/{orderId}")]
        public IActionResult CreateShipment(int orderId)
        {
            try
            {
                var order = SalesOrders.FirstOrDefault(o => (int)o["SalesOrderId"] == orderId);

                if (order == null)
                    return NotFound("Order not found");

                if (order["Status"].ToString() != "Reserved")
                    return BadRequest("Order not ready for shipment");

                var shipment = new Dictionary<string, object>
                {
                    ["ShipmentId"] = Shipments.Count + 1,
                    ["SalesOrderId"] = orderId,
                    ["Status"] = "Prepared"
                };

                Shipments.Add(shipment);

                order["Status"] = "Shipped";

                return Ok(shipment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Shipment error: {ex.Message}");
            }
        }
    }
}