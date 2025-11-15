namespace OnlineShop.Web.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public Order? Order { get; set; }
        public Product? Product { get; set; }
        public decimal UnitPrice => Price;

        // Tổng tiền dòng = Price * Quantity
        public decimal LineTotal => Price * Quantity;

    }
}
