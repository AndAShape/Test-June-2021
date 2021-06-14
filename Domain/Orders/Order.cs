using System;
using System.Collections.Generic;

namespace Domain.Orders
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DeliveryExpected { get; set; }
        public bool ContainsGift { get; set; }

        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
