using System.Collections.Generic;

namespace OnlineShop.Web.Models
{
    // ✅ Model này chứa tất cả dữ liệu cần thiết cho trang Index
    public class ProductIndexViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        // Dùng để biết đang lọc theo category nào
        public int? CurrentCategoryId { get; set; }

        // Dùng để biết đang sắp xếp theo giá nào
        public string? SortByPrice { get; set; }

        public ProductIndexViewModel()
        {
            Products = new List<Product>();
            Categories = new List<Category>();
        }
    }
}