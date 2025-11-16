using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Services;
using System.Threading.Tasks;

namespace OnlineShop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ProductService _productService;

        public HomeController(ProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetProductsAsync();
            return View(products);
        }

        // ======================================================
        // THÊM PHƯƠNG THỨC NÀY VÀO
        // ======================================================
        public IActionResult About()
        {
            return View();
        }
        // ======================================================
    }
}