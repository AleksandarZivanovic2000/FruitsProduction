using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QualityInspectionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QualityController : ControllerBase
    {
        // Fake storage 


        private static List<Dictionary<string, object>> HarvestLots = new();
        private static List<Dictionary<string, object>> SalesOrders = new();
        private static List<Dictionary<string, object>> Invoices = new();
        private static List<Dictionary<string, object>> Payments = new();

        // ================= 1. GET LOTS =================
        [HttpGet("lots")]
        public IActionResult GetLots()
        {
            try
            {
                return Ok(HarvestLots);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ================= 2. INSPECT LOT =================
        [HttpPost("inspect")]
        public IActionResult InspectLot(int id, int score, double defect)
        {
            try
            {
                var lot = HarvestLots.FirstOrDefault(l => (int)l["Id"] == id);
                if (lot == null)
                    return NotFound("Lot not found");

                string status = (score < 70 || defect > 15) ? "Quarantine" : "Approved";

                lot["Status"] = status;

                return Ok(new { Message = "Inspection completed", Status = status });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // ================= 3. CREATE SALES ORDER =================
        [HttpPost("sales-order")]
        public IActionResult CreateSalesOrder(string customer, string produce, double weight)
        {
            try
            {
                int id = SalesOrders.Count + 1;

                SalesOrders.Add(new Dictionary<string, object>
                {
                    ["SalesOrderId"] = id,
                    ["CustomerName"] = customer,
                    ["ProduceType"] = produce,
                    ["Weight"] = weight,
                    ["Status"] = "Created"
                });

                return Ok("Order created");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // ================= 4. CREATE INVOICE =================
        [HttpPost("invoice")]
        public IActionResult GenerateInvoice(int orderId)
        {
            try
            {
                var order = SalesOrders.FirstOrDefault(o => (int)o["SalesOrderId"] == orderId);
                if (order == null)
                    return NotFound("Order not found");

                decimal pricePerKg = 2.5m;
                decimal weight = Convert.ToDecimal(order["Weight"]);

                decimal subtotal = pricePerKg * weight;
                decimal vat = subtotal * 0.2m;
                decimal total = subtotal + vat;

                int invoiceId = Invoices.Count + 1;

                Invoices.Add(new Dictionary<string, object>
                {
                    ["InvoiceId"] = invoiceId,
                    ["Total"] = total,
                    ["Status"] = "Issued"
                });

                return Ok(new { InvoiceId = invoiceId, Total = total });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        // ================= 5. REGISTER PAYMENT =================
        [HttpPost("payment")]
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
                    ["Amount"] = amount
                });

                invoice["Status"] = "Paid";

                return Ok("Payment successful");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // ================= 6. DASHBOARD =================
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            try
            {
                decimal totalRevenue = Invoices.Sum(i => (decimal)i["Total"]);
                int paidInvoices = Invoices.Count(i => i["Status"].ToString() == "Paid");

                return Ok(new
                {
                    Revenue = totalRevenue,
                    PaidInvoices = paidInvoices
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}