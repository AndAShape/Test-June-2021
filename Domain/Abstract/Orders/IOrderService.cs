using System.Threading.Tasks;
using Domain.Orders;

namespace Domain.Abstract.Orders
{
    public interface IOrderService
    {
        Task<(Customer, Order)> GetLastestOrder(string customerEmail, string customerId);
    }
}
