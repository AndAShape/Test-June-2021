using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Orders;

namespace Domain.Abstract
{
    public interface IOrderRepository
    {
        Task<Order> FetchLatestOrder(string customerId);
        Task<IList<OrderItem>> FetchOrderItems(int orderId);
    }
}
