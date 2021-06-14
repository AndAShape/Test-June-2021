using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Abstract.Customers;
using Domain.Abstract.Orders;
using System;

namespace Domain.Orders
{
    public class OrderService : IOrderService
    {
        readonly ICustomerRepository customerRepository;
        readonly IOrderRepository orderRepository;

        public OrderService(ICustomerRepository customerRepository, IOrderRepository orderRepository)
        {
            this.customerRepository = customerRepository;
            this.orderRepository = orderRepository;
        }

        public async Task<(Customer, Order)> GetLastestOrder(string customerEmail, string customerId)
        {
            var customer = await customerRepository.FetchCustomer(customerEmail);

            if (customerId != customer.CustomerId)
            {
                throw new ArgumentException("Invalid customer Id");
            }

            var order = await GetLatestOrderWithItems(customer.CustomerId);

            return (customer, order);
        }

        async Task<Order> GetLatestOrderWithItems(string customerId)
        {
            var order = await orderRepository.FetchLatestOrder(customerId);

            if (order != null)
            {
                order.Items = await orderRepository.FetchOrderItems(order.OrderId);
            }

            return order;
        }
    }
}