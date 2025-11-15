using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Models;
using OnlineShop.Web.Services;
using System.Security.Claims;

namespace OnlineShop.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _api;

        public AccountController(ApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var request = new LoginRequest
            {
                UserName = username,
                Password = password
            };

            // gọi API login
            LoginResponse? result;
            try
            {
                result = await _api.PostAsync<LoginResponse, LoginRequest>("auth/login", request);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không gọi được API: " + ex.Message;
                return View();
            }

            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu!";
                return View();
            }

            // ================================
            // 1️⃣ LƯU TOKEN vào Session
            // ================================
            HttpContext.Session.SetString("JwtToken", result.Token);

            // Lưu thông tin user
            HttpContext.Session.SetInt32("UserId", result.User.UserId);
            HttpContext.Session.SetString("UserName", result.User.UserName);
            HttpContext.Session.SetInt32("RoleId", result.User.RoleId);

            // ================================
            // 2️⃣ TẠO COOKIE LOGIN (MVC)
            // ================================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.User.UserName),
                new Claim("UserId", result.User.UserId.ToString()),
                new Claim(ClaimTypes.Role, result.User.RoleId == 1 ? "Admin" : "User")
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(
                "Cookies",
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddHours(1)
                });

            // ================================
            // 3️⃣ CHUYỂN HƯỚNG THEO ROLE
            // ================================
            if (result.User.RoleId == 1)
                return RedirectToAction("Index", "AdminHome");

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            // Xóa session
            HttpContext.Session.Clear();

            // Xóa cookie login
            await HttpContext.SignOutAsync("Cookies");

            return RedirectToAction("Login");
        }
    }
}
