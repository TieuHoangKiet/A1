using System.Net.Http.Json;
using OnlineShop.Web.Models;

namespace OnlineShop.Web.Services
{
    public class ProductService
    {
        private readonly ApiService _api;

        public ProductService(ApiService api)
        {
            _api = api;
        }

        // ============================
        // 🔹 1. Lấy tất cả sản phẩm
        // ============================
        public Task<List<Product>> GetProductsAsync()
            => _api.GetAsync<List<Product>>("products");

        // ============================
        // 🔹 2. Lấy sản phẩm có lọc & sắp xếp
        //     GET /api/products?categoryId=1&sort=price_asc
        // ============================
        public Task<List<Product>> GetProductsAsync(int? categoryId, string? sort)
        {
            string url = "products";
            List<string> query = new();

            if (categoryId.HasValue)
                query.Add($"categoryId={categoryId.Value}");

            if (!string.IsNullOrEmpty(sort))
                query.Add($"sort={sort}");

            if (query.Count > 0)
                url += "?" + string.Join("&", query);

            return _api.GetAsync<List<Product>>(url);
        }

        // ============================
        // 🔹 3. Lấy 1 sản phẩm theo ID
        // ============================
        public Task<Product?> GetProductAsync(int id)
            => _api.GetAsync<Product>($"products/{id}");

        // ============================
        // 🔹 4. Lấy danh sách category
        // ============================
        public Task<List<Category>> GetCategoriesAsync()
            => _api.GetAsync<List<Category>>("categories");

        // ============================
        // 🔹 5. Tạo sản phẩm mới
        // ============================
        public Task<bool> CreateProductAsync(Product product)
            => _api.PostAsync<bool, Product>("products", product);

        // ============================
        // 🔹 6. Cập nhật sản phẩm
        // ============================
        public Task<bool> UpdateProductAsync(Product product)
            => _api.PutAsync($"products/{product.Id}", product);

        // ============================
        // 🔹 7. Xóa sản phẩm
        // ============================
        public Task<bool> DeleteProductAsync(int id)
            => _api.DeleteAsync($"products/{id}");
    }
}
