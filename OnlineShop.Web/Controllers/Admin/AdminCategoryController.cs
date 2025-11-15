using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Models;
using System.Globalization;
using System.Text;
using System.Net.Http.Json;
using OnlineShop.Web.Filters;


namespace OnlineShop.Web.Controllers
   
{
    [AdminAuthorize]  // 🛡️ Thêm dòng này
    public class AdminCategoryController : Controller
    {
        private readonly HttpClient _http;

        public AdminCategoryController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient();
            _http.BaseAddress = new Uri("http://localhost:5081/api/"); // URL API
        }

        // 🟢 DANH SÁCH
        public async Task<IActionResult> Index()
        {
            try
            {
                var list = await _http.GetFromJsonAsync<List<Category>>("Categories");
                return View(list ?? new List<Category>());
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi tải danh mục: " + ex.Message);
                return View(new List<Category>());
            }
        }

        // 🟢 GET: CREATE
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        // 🟢 POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            model.Slug = Slugify(model.Name);
            model.IsActive = true;
            model.CreatedAt = DateTime.Now;

            var response = await _http.PostAsJsonAsync("Categories", model);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("🟢 API POST => " + response.StatusCode + " | " + content);

            if (response.IsSuccessStatusCode)
            {
                TempData["ok"] = "created";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Không thể thêm danh mục. API trả về lỗi.");
            return View(model);
        }

        // 🟡 GET: EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var cat = await _http.GetFromJsonAsync<Category>($"Categories/{id}");
                if (cat == null) return NotFound();
                return View(cat);
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ Lỗi khi tải danh mục: " + ex.Message);
                return NotFound();
            }
        }

        // 🟡 POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category model)
        {
            if (!ModelState.IsValid) return View(model);

            model.Slug = Slugify(model.Name);

            var response = await _http.PutAsJsonAsync($"Categories/{model.Id}", model);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("🟡 API PUT => " + response.StatusCode + " | " + content);

            if (response.IsSuccessStatusCode)
            {
                TempData["ok"] = "updated";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Cập nhật thất bại! Vui lòng kiểm tra lại.");
            return View(model);
        }

        // 🔴 DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _http.DeleteAsync($"Categories/{id}");
            Console.WriteLine("🔴 API DELETE => " + response.StatusCode);

            if (response.IsSuccessStatusCode)
                TempData["ok"] = "deleted";

            return RedirectToAction(nameof(Index));
        }

        // 🔠 Slugify helper
        private static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            input = input.Trim().ToLowerInvariant();
            string normalized = input.Normalize(NormalizationForm.FormD);

            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            var slug = sb.ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace("đ", "d")
                .Replace(" ", "-")
                .Replace("/", "-")
                .Replace("\\", "-");

            slug = new string(slug.Where(ch => char.IsLetterOrDigit(ch) || ch == '-').ToArray());
            while (slug.Contains("--")) slug = slug.Replace("--", "-");
            return slug.Trim('-');
        }
    }
}
