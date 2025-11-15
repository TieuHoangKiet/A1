using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Models;
using OnlineShop.Web.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace OnlineShop.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Trang danh sách sản phẩm
        public async Task<IActionResult> Index(int? categoryId, string? sort)
        {
            var products = await _productService.GetProductsAsync(categoryId, sort);
            var categories = await _productService.GetCategoriesAsync();

            var viewModel = new ProductIndexViewModel
            {
                Products = products ?? new List<Product>(),
                Categories = categories ?? new List<Category>(),
                CurrentCategoryId = categoryId,
                SortByPrice = sort
            };

            return View(viewModel);
        }

        // Chi tiết sản phẩm
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductAsync(id);
            if (product == null) return NotFound();

            // Build ProductDetailViewModel expected by the view
            var related = new List<Product>();
            if (product.CategoryId != 0)
            {
                var productsInCategory = await _productService.GetProductsAsync(product.CategoryId, null) ?? new List<Product>();
                related = productsInCategory.Where(p => p.Id != product.Id).Take(4).ToList();
            }

            var viewModel = new ProductDetailViewModel
            {
                Product = product,
                RelatedProducts = related,
                Reviews = new List<ReviewViewModel>()
            };

            return View(viewModel);
        }
    }
}
