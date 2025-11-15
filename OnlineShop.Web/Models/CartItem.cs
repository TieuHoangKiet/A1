namespace OnlineShop.Web.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;
        public string? ThumbnailUrl { get; set; }

        public decimal Total => Price * Quantity;
    }
}
