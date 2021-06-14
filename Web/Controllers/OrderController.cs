using System;
using System.Linq;
using System.Threading.Tasks;
using Domain.Abstract.Orders;
using Domain.Orders;
using Microsoft.AspNetCore.Mvc;
using MMT_Test.ViewModels;

namespace MMT_Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        readonly IOrderService orderService;

        public OrdersController(IOrderService orderService) => this.orderService = orderService;

        [HttpPost("recent")]
        public async Task<ActionResult> Get(CustomerParameters customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var email = customer.User.Trim();
            var customerId = customer.CustomerId.Trim();

            (Domain.Customer customer, Order order) response;

            try
            {
                response = await orderService.GetLastestOrder(email, customerId);
            }
            catch (ArgumentException e) when (e.Message.Contains("Customer not found"))
            {
                return NotFound();
            }
            catch (ArgumentException e) when (e.Message.Contains("Invalid customer Id"))
            {
                return BadRequest();
            }

            return Ok(CreateResponse(response.customer, response.order));
        }

        RecentOrderViewModel CreateResponse(Domain.Customer c, Order o)
        { 
            var customer = new CustomerViewModel
            {
                FirstName = c.FirstName,
                LastName = c.LastName
            };

            OrderViewModel order;

            if (o == null)
            {
                order = null;
            }
            else
            {
                var orderItems = o.Items.Select(i => new OrderItemViewModel
                {
                    Product = o.ContainsGift ? "Gift" : i.ProductName,
                    Quantity = i.Quantity,
                    PriceEach = i.Price

                }).ToList();

                static string FormatDate(DateTime date) => string.Format("{0:dd-MMM-yyyy}", date);

                var deliveryAddress = $"{c.HouseNumber} {c.Street}, {c.Town}, {c.Postcode}";

                order = new OrderViewModel
                {
                    OrderNumber = o.OrderId,
                    OrderDate = FormatDate(o.OrderDate),
                    DeliveryAddress = deliveryAddress,
                    OrderItems = orderItems,
                    DeliveryExpected = FormatDate(o.DeliveryExpected),
                };
            }

            return new RecentOrderViewModel
            {
                Customer = customer,
                Order = order
            };
        }
    }
}
