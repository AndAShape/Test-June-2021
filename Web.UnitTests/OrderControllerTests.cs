using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract.Orders;
using Domain.Orders;
using Microsoft.AspNetCore.Mvc;
using MMT_Test;
using MMT_Test.Controllers;
using MMT_Test.ViewModels;
using Moq;
using Xunit;

namespace Web.UnitTests
{
    public class OrderControllerTests
    {
        RecentOrderViewModel GetResult(ActionResult actionResult) =>
            (RecentOrderViewModel)(actionResult as OkObjectResult).Value;

        [Fact]
        public async Task Unknown_Customer_Returns_404()
        {
            var mock = new Mock<IOrderService>();

            mock.Setup(s => s.GetLastestOrder(It.IsAny<string>(), It.IsAny<string>())).
                ThrowsAsync(new ArgumentException("Customer not found"));

            var controller = new OrdersController(mock.Object);

            var resp = await controller.Get(new CustomerParameters { CustomerId = "1", User = "test@email.com" });

            Assert.IsType<NotFoundResult>(resp);
        }

        [Fact]
        public async Task Incorrect_CustomerId_Returns_BadRequest()
        {
            var mock = new Mock<IOrderService>();

            mock.Setup(s => s.GetLastestOrder(It.IsAny<string>(), It.IsAny<string>())).
                ThrowsAsync(new ArgumentException("Invalid customer Id"));

            var controller = new OrdersController(mock.Object);

            var resp = await controller.Get(new CustomerParameters { CustomerId = "1", User = "test@email.com" });

            Assert.IsType<BadRequestResult>(resp);
        }

        [Fact]
        public async Task Order_Containing_Gifts_Hides_Product_Name()
        {
            var mock = new Mock<IOrderService>();

            mock.Setup(s => s.GetLastestOrder(It.IsAny<string>(), It.IsAny<string>())).
                ReturnsAsync(() => (new Customer(), new Order
                {
                    ContainsGift = true,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductName = "1" },
                        new OrderItem { ProductName = "2" },
                    }
                }));

            var controller = new OrdersController(mock.Object);

            var resp = await controller.Get(new CustomerParameters { CustomerId = "1", User = "test@email.com" });

            var orderItems = GetResult(resp).Order.OrderItems;

            Assert.True(orderItems.All(oi => oi.Product == "Gift"));
        }

        [Fact]
        public async Task Order_Not_Containing_Gifts_Shows_Product_Name()
        {
            var mock = new Mock<IOrderService>();

            mock.Setup(s => s.GetLastestOrder(It.IsAny<string>(), It.IsAny<string>())).
                ReturnsAsync(() => (new Customer(), new Order
                {
                    ContainsGift = false,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductName = "1" },
                        new OrderItem { ProductName = "2" },
                    }
                }));

            var controller = new OrdersController(mock.Object);

            var resp = await controller.Get(new CustomerParameters { CustomerId = "1", User = "test@email.com" });

            var orderItems = GetResult(resp).Order.OrderItems;

            Assert.True(orderItems[0].Product == "1");
            Assert.True(orderItems[1].Product == "2");
        }

        [Fact]
        public async Task No_Order_Results_In_Null()
        {
            var mock = new Mock<IOrderService>();

            mock.Setup(s => s.GetLastestOrder(It.IsAny<string>(), It.IsAny<string>())).
                ReturnsAsync(() => (new Customer(), null));

            var controller = new OrdersController(mock.Object);

            var resp = await controller.Get(new CustomerParameters { CustomerId = "1", User = "test@email.com" });

            Assert.Null(GetResult(resp).Order);
        }

        [Fact]
        public async Task Correct_Mapping()
        {
            var mock = new Mock<IOrderService>();

            mock.Setup(s => s.GetLastestOrder(It.IsAny<string>(), It.IsAny<string>())).
                ReturnsAsync(() => (new Customer
                {
                    FirstName = "John",
                    LastName = "Smith",
                    HouseNumber = "1A",
                    Street = "Oxford Street",
                    Town = "London",
                    Postcode = "W1"

                }, new Order
                {
                    OrderId = 1,
                    OrderDate = new DateTime(2022, 1, 10),
                    DeliveryExpected = new DateTime(2021, 2, 2),
                    ContainsGift = false,
                    Items = new List<OrderItem>
                    {
                        new OrderItem { ProductName = "1", Price = 2.99M, Quantity = 2 },
                        new OrderItem { ProductName = "2", Price = 5M, Quantity = 8 },
                    }
                }));

            var controller = new OrdersController(mock.Object);

            var resp = GetResult(await controller.Get(new CustomerParameters { CustomerId = "1", User = "test@email.com" }));

            var c = resp.Customer;

            Assert.Equal("John", c.FirstName);
            Assert.Equal("Smith", c.LastName);

            var o = resp.Order;

            Assert.Equal(1, o.OrderNumber);
            Assert.Equal("10-Jan-2022", o.OrderDate);
            Assert.Equal("02-Feb-2021", o.DeliveryExpected);
            Assert.Equal("1A Oxford Street, London, W1", o.DeliveryAddress);

            var item = o.OrderItems[0];

            Assert.Equal("1", item.Product);
            Assert.Equal(2.99M, item.PriceEach);
            Assert.Equal(2, item.Quantity);

            item = o.OrderItems[1];

            Assert.Equal("2", item.Product);
            Assert.Equal(5M, item.PriceEach);
            Assert.Equal(8, item.Quantity);
        }
    }
}