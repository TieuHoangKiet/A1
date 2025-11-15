namespace OnlineShop.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }       // ✅ thêm
        public string? ImageUrl { get; set; } // ✅ thêm
        public string? Description { get; set; } // ✅ thêm
        public bool? IsActive { get; set; }  // ✅ thêm
        public DateTime CreatedAt { get; set; } // ✅ thêm
        public string? ThumbnailUrl { get; set; }
        public string? ShortDescription { get; set; }   // Mô tả ngắn
        public Category? Category { get; set; }

    }
}
