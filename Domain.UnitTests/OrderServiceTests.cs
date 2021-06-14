using System;
using System.Threading.Tasks;
using Domain.Abstract.Customers;
using Domain.Orders;
using Xunit;

namespace Domain.UnitTests
{
    public class MockCustomerRepository : ICustomerRepository
    {
        public Task<Customer> FetchCustomer(string email)
        {
            return Task.FromResult(new Customer { CustomerId = "hello" });
        }
    }

    public class OrderServiceTests
    {
        [Fact]
        public async Task Incorrect_Id_Throws_Argument_Exception()
        {
            var orderService = new OrderService(new MockCustomerRepository(), null);

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => orderService.GetLastestOrder("john@smith.com", "123"));

            Assert.Equal("Invalid customer Id", exception.Message);
        }
    }
}
