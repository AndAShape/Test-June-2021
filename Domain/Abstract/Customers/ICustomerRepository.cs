using System.Threading.Tasks;

namespace Domain.Abstract.Customers
{
    public interface ICustomerRepository
    {
        Task<Customer> FetchCustomer(string email);
    }
}