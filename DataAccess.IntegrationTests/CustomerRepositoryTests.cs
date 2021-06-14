using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DataAccess.IntegrationTests
{
    public class CustomerRepositoryTests
    {
        [Fact]
        public async Task Can_Fetch_Customer()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            configBuilder.AddJsonFile("config.Development.json");

            var c = configBuilder.Build();

            var repository = new CustomerRepository(c.GetValue<string>("ApiKey"),
                c.GetValue<string>("BaseUrl"));

            var customer = await repository.FetchCustomer("cat.owner@mmtdigital.co.uk");

            Assert.Equal("C34454", customer.CustomerId);
            Assert.Equal("cat.owner@mmtdigital.co.uk", customer.Email);
            Assert.Equal("Charlie", customer.FirstName);
            Assert.Equal("Cat", customer.LastName);

            Assert.Equal("1a", customer.HouseNumber);
            Assert.Equal("Uppingham Gate", customer.Street);
            Assert.Equal("Uppingham", customer.Town);
            Assert.Equal("LE15 9NY", customer.Postcode);
        }

        [Fact]
        public async Task Customer_Not_Found_Throws_Argument_Exception()
        {
            var configBuilder = new ConfigurationBuilder();

            configBuilder.SetBasePath(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
            configBuilder.AddJsonFile("config.Development.json");

            var c = configBuilder.Build();

            var repository = new CustomerRepository(c.GetValue<string>("ApiKey"),
                c.GetValue<string>("BaseUrl"));

            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => repository.FetchCustomer("dontexist@hello.world"));

            Assert.Equal("Customer not found", exception.Message);
        }
    }
}