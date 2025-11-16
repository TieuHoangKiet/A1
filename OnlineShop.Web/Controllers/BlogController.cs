using Microsoft.AspNetCore.Mvc;
using OnlineShop.Web.Services;
using OnlineShop.Web.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace OnlineShop.Web.Controllers
{
    public class BlogController : Controller
    {
        private readonly PortService _portService;
        private readonly IWebHostEnvironment _env;

        public BlogController(PortService portService, IWebHostEnvironment env)
        {
            _portService = portService;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _portService.GetAllAsync() ?? new List<PortViewModel>();

            // Diagnostic: confirm physical view file exists
            var viewPath = Path.Combine(_env.ContentRootPath, "Views", "Blog", "Index.cshtml");
            if (!System.IO.File.Exists(viewPath))
            {
                var blogDir = Path.Combine(_env.ContentRootPath, "Views", "Blog");
                var files = new List<string>();
                if (Directory.Exists(blogDir))
                {
                    files = Directory.GetFiles(blogDir).Select(Path.GetFileName).ToList();
                }

                var msg = $"View not found at: {viewPath}\nContentRoot: {_env.ContentRootPath}\nBlog folder exists: {Directory.Exists(blogDir)}\nFiles in Blog folder: {string.Join(", ", files)}";
                return Content(msg);
            }

            return View(list);
        }

        public async Task<IActionResult> Details(int id)
        {
            var post = await _portService.GetAsync(id);
            if (post == null) return NotFound();
            return View(post);
        }
    }
}
