using System.Collections.Generic;

namespace OnlineShop.Web.Models
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
