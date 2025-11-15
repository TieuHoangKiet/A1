using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Filters;
using OnlineShop.Web.Services;
using OnlineShop.Web.Models;

namespace OnlineShop.Web.Controllers
{
    [AdminAuthorize]
    public class AdminHomeController : Controller
    {
        private readonly ApiService _api;

        public AdminHomeController(ApiService api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _api.GetAsync<List<OrderDto>>("orders");

            if (orders == null)
                return View(new DashboardStats());

            var stats = new DashboardStats
            {
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.Total),
                TotalProductsSold = orders.Sum(o => o.OrderItems.Sum(i => i.Quantity))
            };

            return View(stats);
        }

    }

    public class DashboardStats
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalProductsSold { get; set; }
    }
}
