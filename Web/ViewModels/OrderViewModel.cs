using System.Collections.Generic;

namespace MMT_Test.ViewModels
{
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string DeliveryAddress { get; set; }

        public IList<OrderItemViewModel> OrderItems { get; set; } = new List<OrderItemViewModel>();

        public string DeliveryExpected { get; set; }
    }
}
