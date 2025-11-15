using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Models;
using OnlineShop.Web.Services;
using OnlineShop.Web.Filters;

namespace OnlineShop.Web.Controllers
{
    [AdminAuthorize]
    public class AdminOrderController : Controller
    {
        private readonly ApiService _api;

        public AdminOrderController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _api.GetAsync<List<Order>>("orders");
            return View(list ?? new List<Order>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _api.GetAsync<Order>($"orders/{id}");
            if (order == null) return NotFound();

            ViewBag.Items = order.OrderItems ?? new List<OrderItem>();
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var ok = await _api.PutAsync($"orders/status/{id}", status);
            TempData[ok ? "Success" : "Error"] = ok ? "Đã cập nhật!" : "Cập nhật thất bại!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _api.DeleteAsync($"orders/{id}");
            TempData[ok ? "ok" : "error"] = ok ? "deleted" : "delete-failed";
            return RedirectToAction(nameof(Index));
        }

    }
}
