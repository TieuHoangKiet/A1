using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace OnlineShop.Web.Controllers
{
    public class DebugController : Controller
    {
        // ⭐ 1) Hiển thị token thô
        public IActionResult Token()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return Content("⚠ NO TOKEN FOUND");

            return Content("🔐 JWT TOKEN:\n\n" + token);
        }

        // ⭐ 2) Decode token → JSON đẹp
        public IActionResult Decode()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return Content("⚠ NO TOKEN FOUND");

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var payload = new
            {
                UserName = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                UserId = jwt.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value,
                Role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                ExpireAt = jwt.ValidTo
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return Content(json, "application/json");
        }

        // ⭐ 3) Đẩy token vào Swagger
        public IActionResult Swagger()
        {
            var token = HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrEmpty(token))
                return Content("⚠ NO TOKEN FOUND");

            string script = @$"
                <script>
                    localStorage.setItem('swagger_token', 'Bearer {token}');
                    window.location.href = 'http://localhost:5081/swagger';
                </script>
            ";

            return Content(script, "text/html");
        }
    }
}
