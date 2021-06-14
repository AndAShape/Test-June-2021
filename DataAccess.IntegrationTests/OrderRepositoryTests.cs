using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DataAccess.IntegrationTests
{
    public class OrderRepositoryTests
    {
        [Fact]
        public async Task Can_Fetch_Most_Recent_Order()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            configBuilder.AddJsonFile("config.Development.json");

            var c = configBuilder.Build();

            var repository = new OrderRepository(c.GetValue<string>("DbConnectionString"));

            var order = await repository.FetchLatestOrder("C34454");

            Assert.Equal(4, order.OrderId);
            Assert.Equal(new DateTime(2020, 9, 11), order.OrderDate);
            Assert.Equal(new DateTime(2021, 5, 7), order.DeliveryExpected);
            Assert.False(order.ContainsGift);

            order = await repository.FetchLatestOrder("R34788");

            Assert.Equal(6, order.OrderId);
            Assert.Equal(new DateTime(2020, 10, 18), order.OrderDate);
            Assert.Equal(new DateTime(2021, 6, 25), order.DeliveryExpected);
            Assert.False(order.ContainsGift);

            order = await repository.FetchLatestOrder("A99001");

            Assert.Equal(9, order.OrderId);
            Assert.Equal(new DateTime(2020, 12, 3), order.OrderDate);
            Assert.Equal(new DateTime(2021, 5, 27), order.DeliveryExpected);
            Assert.False(order.ContainsGift);

            order = await repository.FetchLatestOrder("XM45001");

            Assert.Equal(15, order.OrderId);
            Assert.Equal(new DateTime(2020, 12, 24), order.OrderDate);
            Assert.Equal(new DateTime(2021, 12, 24), order.DeliveryExpected);
            Assert.True(order.ContainsGift);
        }

        [Fact]
        public async Task No_Order_Returns_Null()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            configBuilder.AddJsonFile("config.Development.json");

            var c = configBuilder.Build();

            var repository = new OrderRepository(c.GetValue<string>("DbConnectionString"));

            var order = await repository.FetchLatestOrder("X");

            Assert.Null(order);
        }

        [Fact]
        public async Task Can_Fetch_Order_Items()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            configBuilder.AddJsonFile("config.Development.json");

            var c = configBuilder.Build();

            var repository = new OrderRepository(c.GetValue<string>("DbConnectionString"));

            var items = await repository.FetchOrderItems(15);

            Assert.Equal(5, items.Count);

            var item = items.First();

            Assert.Equal("Cat Climbing Tower", item.ProductName);
            Assert.Equal(1, item.Quantity);
            Assert.Equal(8.99M, item.Price);

            item = items.Last();

            Assert.Equal("Superman costume for pet", item.ProductName);
            Assert.Equal(2, item.Quantity);
            Assert.Equal(13.99M, item.Price);
        }
    }
}