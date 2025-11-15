using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Models;
using System.Net.Http.Json;

namespace OnlineShop.Web.Controllers
{
    public class CartController : Controller
    {
        private const string CART_KEY = "CART_SESSION";

        // 🧠 Lấy giỏ hàng từ Session
        private List<CartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
            return cart ?? new List<CartItem>();
        }

        // 💾 Lưu giỏ hàng vào Session
        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
        }

        // 🛒 Trang giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // ➕ Thêm sản phẩm vào giỏ
        [HttpPost]
        public IActionResult AddToCart(int id, string name, decimal price, string? thumbnailUrl)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
                item.Quantity++;
            else
                cart.Add(new CartItem { ProductId = id, Name = name, Price = price, ThumbnailUrl = thumbnailUrl });

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ❌ Xóa sản phẩm khỏi giỏ
        public IActionResult Remove(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ======================================================
        // ⭐ HÀM MỚI: Tăng số lượng
        // ======================================================
        [HttpPost]
        public IActionResult IncreaseQuantity(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                item.Quantity++;
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // ======================================================
        // ⭐ HÀM MỚI: Giảm số lượng
        // ======================================================
        [HttpPost]
        public IActionResult DecreaseQuantity(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                }
                else
                {
                    // Nếu số lượng là 1 mà còn giảm thì xóa luôn
                    cart.Remove(item);
                }
            }
            SaveCart(cart);
            return RedirectToAction("Index");
        }


        // 🧾 Hiển thị form đặt hàng
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (!cart.Any()) return RedirectToAction("Index");

            var order = new Order
            {
                OrderItems = cart.Select(x => new OrderItem
                {
                    ProductId = x.ProductId,
                    ProductName = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity
                }).ToList(),
                Total = cart.Sum(x => x.Total)
            };

            return View(order);
        }

        // ✅ Gửi đơn hàng sang API để lưu DB
        [HttpPost]
        public async Task<IActionResult> Checkout(Order order)
        {
            var cart = GetCart();
            if (!cart.Any())
                return RedirectToAction("Index");

            order.OrderItems = cart.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                ProductName = x.Name,
                Price = x.Price,
                Quantity = x.Quantity
            }).ToList();

            order.Total = cart.Sum(x => x.Total);
            order.OrderDate = DateTime.Now;
            order.Status = "Pending";

            // 🧩 Thêm dòng fix lỗi
            order.UserEmail = "guest@example.com"; // hoặc user email thật

            //
            // ⭐⭐⭐ BẮT ĐẦU GIẢI PHÁP 1 ⭐⭐⭐
            //
            foreach (var item in order.OrderItems)
            {
                item.Order = null;   // ✅ Ngắt tham chiếu vòng
                item.Product = null; // ✅ Ngắt tham chiếu vòng
            }
            //
            // ⭐⭐⭐ KẾT THÚC GIẢI PHÁP 1 ⭐⭐⭐
            //


            try
            {
                using var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5081/api/");

                var response = await client.PostAsJsonAsync("Orders", order);
                var content = await response.Content.ReadAsStringAsync();

                Console.WriteLine("==== API RESPONSE ====");
                Console.WriteLine($"Status: {response.StatusCode}");
                Console.WriteLine(content);

                if (response.IsSuccessStatusCode)
                {
                    SaveCart(new List<CartItem>());
                    HttpContext.Session.SetObjectAsJson("LAST_ORDER", order);
                    return RedirectToAction("Success");
                }
                else
                {
                    ViewBag.ApiError = $"API trả về lỗi: {response.StatusCode}. Nội dung: {content}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ApiError = "Lỗi khi gọi API: " + ex.Message;
            }

            return View(order);
        }

        // 🎉 Trang thông báo sau khi đặt hàng
        public IActionResult Success()
        {
            var order = HttpContext.Session.GetObjectFromJson<Order>("LAST_ORDER");
            if (order == null)
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(order);
        }

    }
}