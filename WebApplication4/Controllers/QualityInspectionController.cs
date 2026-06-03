using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QualityInspectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        // ====== FAKE DATABASE ======
        private static List<Dictionary<string, object>> HarvestLots = new();
        private static List<Dictionary<string, object>> SalesOrders = new();
        private static List<Dictionary<string, object>> SalesOrderItems = new();
        private static List<Dictionary<string, object>> Inventory = new();
        private static List<Dictionary<string, object>> Invoices = new();
        private static List<Dictionary<string, object>> Payments = new();

        // ===================== 1 =====================
        // GET: api/system/lots
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

        // ===================== 2 =====================
        // POST: api/system/create-order
        [HttpPost("create-order")]
        public IActionResult CreateOrder(string customer, string produce, double weight)
        {
            try
            {
                int id = SalesOrders.Count + 1;

                SalesOrders.Add(new Dictionary<string, object>
                {
                    ["SalesOrderId"] = id,
                    ["CustomerName"] = customer,
                    ["Status"] = "Created"
                });

                SalesOrderItems.Add(new Dictionary<string, object>
                {
                    ["SalesOrderId"] = id,
                    ["ProduceType"] = produce,
                    ["RequestedWeight"] = weight,
                    ["ReservedWeight"] = 0.0
                });

                return Ok("Order created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating order: {ex.Message}");
            }
        }

        // ===================== 3 =====================
        // POST: api/system/reserve/{id}
        [HttpPost("reserve/{id}")]
        public IActionResult Reserve(int id)
        {
            try
            {
                var item = SalesOrderItems.FirstOrDefault(i => (int)i["SalesOrderId"] == id);
                if (item == null)
                    return NotFound("Order item not found");

                double remaining = (double)item["RequestedWeight"];

                foreach (var lot in HarvestLots.Where(l => l["Status"].ToString() == "Approved"))
                {
                    if (remaining <= 0) break;

                    double available = (double)lot["GrossQuantity"];
                    double allocated = Math.Min(available, remaining);

                    lot["GrossQuantity"] = available - allocated;
                    remaining -= allocated;
                    item["ReservedWeight"] = (double)item["ReservedWeight"] + allocated;
                }

                return Ok("Reservation completed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error during reservation: {ex.Message}");
            }
        }

        // ===================== 4 =====================
        // POST: api/system/add-inventory/{lotId}
        [HttpPost("add-inventory/{lotId}")]
        public IActionResult AddInventory(int lotId)
        {
            try
            {
                var lot = HarvestLots.FirstOrDefault(l => (int)l["Id"] == lotId);
                if (lot == null)
                    return NotFound("Lot not found");

                int inventoryId = Inventory.Count + 1;

                Inventory.Add(new Dictionary<string, object>
                {
                    ["InventoryId"] = inventoryId,
                    ["ProduceType"] = lot["ProduceType"],
                    ["QuantityKg"] = lot["GrossQuantity"],
                    ["EntryDate"] = DateTime.Now
                });

                return Ok("Inventory added");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Inventory error: {ex.Message}");
            }
        }

        // ===================== 5 =====================
        // POST: api/system/invoice/{orderId}
        [HttpPost("invoice/{orderId}")]
        public IActionResult GenerateInvoice(int orderId)
        {
            try
            {
                var item = SalesOrderItems.FirstOrDefault(i => (int)i["SalesOrderId"] == orderId);
                if (item == null)
                    return NotFound("Order item not found");

                decimal weight = Convert.ToDecimal(item["ReservedWeight"]);
                decimal pricePerKg = 2.5m;

                decimal subtotal = weight * pricePerKg;
                decimal vat = subtotal * 0.2m;
                decimal total = subtotal + vat;

                Invoices.Add(new Dictionary<string, object>
                {
                    ["InvoiceId"] = Invoices.Count + 1,
                    ["Total"] = total,
                    ["Status"] = "Issued"
                });

                return Ok($"Invoice generated. Total: {total}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Invoice error: {ex.Message}");
            }
        }

        // ===================== 6 =====================
        // POST: api/system/payment/{invoiceId}
        [HttpPost("payment/{invoiceId}")]
        public IActionResult RegisterPayment(int invoiceId, decimal amount)
        {
            try
            {
                var invoice = Invoices.FirstOrDefault(i => (int)i["InvoiceId"] == invoiceId);
                if (invoice == null)
                    return NotFound("Invoice not found");

                Payments.Add(new Dictionary<string, object>
                {
                    ["InvoiceId"] = invoiceId,
                    ["Amount"] = amount,
                    ["Date"] = DateTime.Now
                });

                invoice["Status"] = "Paid";

                return Ok("Payment successful");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Payment error: {ex.Message}");
            }
        }
    }
}