namespace OnlineShop.Web.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public decimal Total { get; set; }
        public string? ShipName { get; set; }
        public string? ShipPhone { get; set; }
        public string? ShipAddress { get; set; }

        public List<OrderItem>? OrderItems { get; set; }
    }
}
