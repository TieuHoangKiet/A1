using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Api.Data;
using OnlineShop.Web.Models;
using System.Globalization;
using System.Text;
using ApiProduct = OnlineShop.Api.Models.Product; // Alias cho Product trong API Models
using OnlineShop.Web.Filters;


namespace OnlineShop.Web.Controllers
   
{
    [AdminAuthorize]  // 🛡️ Thêm dòng này
    public class AdminProductController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminProductController(ShopDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // 🟢 Danh sách sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(products);
        }

        // 🟡 GET: Tạo sản phẩm
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        // 🟠 POST: Tạo sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken] // chỉ 1 lần duy nhất
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            ViewBag.Categories = _context.Categories.ToList();

            if (!ModelState.IsValid)
                return View(model);

            // 🧩 Tạo slug chuẩn SEO (loại bỏ dấu tiếng Việt và trùng slug)
            string baseSlug = GenerateSlug(model.Name);
            string slug = baseSlug;
            int count = 1;
            while (await _context.Products.AnyAsync(p => p.Slug == slug))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            // 🧩 Upload ảnh nếu có
            string? imageUrl = null;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                string folder = Path.Combine(_env.WebRootPath, "images/products");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                imageUrl = "/images/products/" + fileName;
            }

            // 🧩 Gán sang entity để lưu DB
            var product = new ApiProduct
            {
                Name = model.Name,
                Slug = slug,
                Price = model.Price,
                Stock = model.Stock,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.Now,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 🔵 GET: Sửa sản phẩm
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();

            var model = new ProductViewModel
            {
                Id = product.Id, // thêm dòng này để fix lỗi asp-for="Id"
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                Description = product.Description,
                CategoryId = product.CategoryId
            };

            return View(model);
        }

        // 🔴 POST: Sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken] // chỉ 1 lần duy nhất
        public async Task<IActionResult> Edit(int id, ProductViewModel model)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = _context.Categories.ToList();
                return View(model);
            }

            // 🧩 Cập nhật thông tin cơ bản
            product.Name = model.Name;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.Description = model.Description;
            product.CategoryId = model.CategoryId;

            // 🧩 Tạo slug chuẩn SEO (loại bỏ dấu tiếng Việt và trùng slug)
            string baseSlug = GenerateSlug(model.Name);
            string slug = baseSlug;
            int count = 1;

            while (await _context.Products.AnyAsync(p => p.Slug == slug && p.Id != id))
            {
                slug = $"{baseSlug}-{count}";
                count++;
            }

            product.Slug = slug;

            // 🧩 Upload ảnh mới nếu có
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                string folder = Path.Combine(_env.WebRootPath, "images/products");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(folder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                product.ImageUrl = "/images/products/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Loại bỏ dấu tiếng Việt và tạo slug thân thiện
        /// </summary>
        private string GenerateSlug(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            input = input.Trim().ToLowerInvariant();

            // bỏ dấu tiếng Việt
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            // thay ký tự đặc biệt bằng -
            string slug = sb.ToString()
                .Normalize(NormalizationForm.FormC)
                .Replace("đ", "d")
                .Replace(" ", "-")
                .Replace("/", "-")
                .Replace("\\", "-");

            // loại bỏ ký tự không hợp lệ
            slug = new string(slug.Where(ch => char.IsLetterOrDigit(ch) || ch == '-').ToArray());

            // tránh trùng nhiều dấu '-'
            while (slug.Contains("--"))
                slug = slug.Replace("--", "-");

            return slug.Trim('-');
        }

        // ⚫ Xóa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken] // thêm để an toàn form
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
