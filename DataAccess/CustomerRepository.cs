using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain;
using Domain.Abstract.Customers;
using Newtonsoft.Json;

namespace DataAccess
{
    public class CustomerRepository : ICustomerRepository
    {
        readonly string apiKey;
        readonly Uri baseUrl;

        public CustomerRepository(string apiKey, string baseUrl)
        {
            this.apiKey = apiKey;
            this.baseUrl = new Uri(baseUrl);
        }

        public async Task<Customer> FetchCustomer(string email)
        {
            var client = new HttpClient { BaseAddress = baseUrl };

            string response = null;

            try
            {
                response = await client.GetStringAsync($"GetUserDetails?code={apiKey}&email={email}");
            }
            catch (HttpRequestException e) when (e.Message.Contains("404"))
            {
                throw new ArgumentException("Customer not found");
            }

            var item = await Task.Run(() => JsonConvert.DeserializeObject<Customer>(response));

            return item;
        }
    }
}