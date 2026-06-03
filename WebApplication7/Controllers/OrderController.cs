using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private static List<CustomerOrder> orders = new();
        private static List<CustomerOrderItem> items = new();
        private static List<Inventory> inventory = new();
        private static List<InventoryAllocation> allocations = new();

        // ================= CREATE ORDER =================
        [HttpPost("create")]
        public IActionResult CreateOrder(CustomerOrder order)
        {
            try
            {
                order.CustomerOrderId = orders.Count + 1;
                order.OrderDate = DateTime.Now;
                order.Status = "Draft";

                orders.Add(order);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error creating order: {ex.Message}");
            }
        }

        // ================= ADD ITEM =================
        [HttpPost("add-item")]
        public IActionResult AddItem(CustomerOrderItem item)
        {
            try
            {
                item.CustomerOrderItemId = items.Count + 1;
                item.AllocatedQuantityKg = 0;

                items.Add(item);

                return Ok(item);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error adding item: {ex.Message}");
            }
        }

        // ================= CONFIRM ORDER =================
        [HttpPut("confirm/{id}")]
        public IActionResult ConfirmOrder(int id)
        {
            try
            {
                var order = orders.FirstOrDefault(o => o.CustomerOrderId == id);

                if (order == null)
                    return NotFound("Order not found");

                order.Status = "Confirmed";
                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error confirming order: {ex.Message}");
            }
        }

        // ================= ALLOCATE INVENTORY =================
        [HttpPost("allocate/{orderId}")]
        public IActionResult Allocate(int orderId)
        {
            try
            {
                var orderItems = items.Where(i => i.CustomerOrderId == orderId).ToList();

                foreach (var item in orderItems)
                {
                    double needed = item.RequestedQuantityKg;

                    var stocks = inventory
                        .Where(i => i.ProduceType == item.ProduceType)
                        .OrderBy(i => i.ExpirationDate);

                    foreach (var stock in stocks)
                    {
                        if (needed <= 0) break;

                        double alloc = Math.Min(stock.QuantityKg, needed);

                        stock.QuantityKg -= alloc;
                        needed -= alloc;

                        item.AllocatedQuantityKg += alloc;

                        allocations.Add(new InventoryAllocation
                        {
                            InventoryAllocationId = allocations.Count + 1,
                            InventoryId = stock.InventoryId,
                            CustomerOrderItemId = item.CustomerOrderItemId,
                            AllocatedQuantityKg = alloc
                        });
                    }
                }

                return Ok("Allocation completed");
            }
            catch (Exception ex)
            {
                return BadRequest($"Allocation error: {ex.Message}");
            }
        }

        // ================= GET ORDERS =================
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error fetching orders: {ex.Message}");
            }
        }

        // ================= DELETE ORDER =================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var order = orders.FirstOrDefault(o => o.CustomerOrderId == id);

                if (order == null)
                    return NotFound("Order not found");

                orders.Remove(order);

                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest($"Delete error: {ex.Message}");
            }
        }
    }
}