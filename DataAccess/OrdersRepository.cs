using System.Threading.Tasks;
using Domain.Abstract;
using Domain.Orders;
using Dapper;
using System.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;

namespace DataAccess
{
    public class OrderRepository : IOrderRepository
    {
        readonly string connectionString;

        public OrderRepository(string connectionString) => this.connectionString = connectionString;

        public async Task<Order> FetchLatestOrder(string customerId)
        {
            string query = $@"SELECT TOP 1 ORDERID, ORDERDATE, DELIVERYEXPECTED, CONTAINSGIFT
                                FROM ORDERS  
                                WHERE CUSTOMERID = @customerId
                                ORDER BY ORDERDATE DESC";

            using var conn = new SqlConnection(connectionString);

            var result = await conn.QueryAsync<Order>(query, new { customerId });

            return result.FirstOrDefault();
        }

        public async Task<IList<OrderItem>> FetchOrderItems(int orderId)
        {
            string query = $@"SELECT p.PRODUCTNAME, oi.QUANTITY, oi.PRICE 
                                FROM ORDERITEMS oi
                                INNER JOIN PRODUCTS p
                                ON p.PRODUCTID = oi.PRODUCTID
                                WHERE oi.ORDERID = @orderId
                                ORDER BY ORDERITEMID";

            using var conn = new SqlConnection(connectionString);

            var result = await conn.QueryAsync<OrderItem>(query, new { orderId });

            return result.ToList();
        }
    }
}