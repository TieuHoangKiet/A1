using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Models;
using System.Net.Http.Json;
using OnlineShop.Web.Filters;


namespace OnlineShop.Web.Controllers
   
{
    [AdminAuthorize]  // 🛡️ Thêm dòng này
    public class AdminAccountController : Controller
    {
        private readonly HttpClient _http;

        public AdminAccountController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("http://localhost:5081/api/");
        }

        // 🧱 HÀM KIỂM TRA QUYỀN ADMIN
        private bool IsAdmin()
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            return roleId == 1;
        }

        // 🟢 DANH SÁCH
        public async Task<IActionResult> Index()
        {
            // Kiểm tra đăng nhập và quyền admin
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var list = await _http.GetFromJsonAsync<List<AccountUser>>("Accounts");

            // Lọc bỏ tài khoản admin và dữ liệu test
            var filtered = list?
                .Where(u =>
                    u.UserName != "admin" && // ẩn tài khoản admin
                    !u.UserName.Contains("string", StringComparison.OrdinalIgnoreCase) && // bỏ dữ liệu test
                    (u.Role?.RoleName == "User" || u.RoleId == 2)) // chỉ hiển thị user
                .OrderByDescending(u => u.UserId)
                .ToList();

            return View(filtered ?? new List<AccountUser>());
        }

        // 🟢 GET: TẠO
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            ViewBag.Roles = await _http.GetFromJsonAsync<List<RoleUser>>("Roles");
            return View(new AccountUser());
        }

        // 🟢 POST: TẠO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountUser model)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _http.GetFromJsonAsync<List<RoleUser>>("Roles");
                return View(model);
            }

            // Chỉ gửi dữ liệu cần thiết, không gửi object Role
            var payload = new
            {
                userName = model.UserName,
                passwordHash = model.PasswordHash,
                fullName = model.FullName,
                email = model.Email,
                phone = model.Phone,
                roleId = model.RoleId
            };

            var response = await _http.PostAsJsonAsync("Accounts", payload);

            if (response.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Không thể tạo tài khoản!");
            ViewBag.Roles = await _http.GetFromJsonAsync<List<RoleUser>>("Roles");
            return View(model);
        }

        // 🟡 GET: SỬA
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var acc = await _http.GetFromJsonAsync<AccountUser>($"Accounts/{id}");
            if (acc == null) return NotFound();

            ViewBag.Roles = await _http.GetFromJsonAsync<List<RoleUser>>("Roles");
            return View(acc);
        }

        // 🟡 POST: SỬA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountUser model)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = await _http.GetFromJsonAsync<List<RoleUser>>("Roles");
                return View(model);
            }

            var res = await _http.PutAsJsonAsync($"Accounts/{model.UserId}", model);
            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "Cập nhật thất bại!");
            ViewBag.Roles = await _http.GetFromJsonAsync<List<RoleUser>>("Roles");
            return View(model);
        }

        // 🔴 XÓA
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var res = await _http.DeleteAsync($"Accounts/{id}");
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (model.PasswordHash != model.ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp!";
                return View(model);
            }

            var payload = new
            {
                userName = model.UserName,
                passwordHash = model.PasswordHash,
                email = model.Email,
                fullName = model.FullName,
                phone = model.Phone
            };

            var res = await _http.PostAsJsonAsync("accounts/register", payload);

            if (!res.IsSuccessStatusCode)
            {
                var msg = await res.Content.ReadAsStringAsync();
                ViewBag.Error = "Lỗi đăng ký: " + msg;
                return View(model);
            }

            // Auto login
            return RedirectToAction("Login", "Account");
        }

    }
}
