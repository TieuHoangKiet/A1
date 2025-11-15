using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OnlineShop.Api.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OnlineShop.Api.Models;

namespace OnlineShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ShopDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ShopDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // ⭐ DÙNG ĐÚNG TÊN DbSet: Account_Users
            var user = _context.Account_Users
                .FirstOrDefault(x => x.UserName == request.UserName
                                     && x.PasswordHash == request.Password);

            if (user == null)
                return Unauthorized("Sai tài khoản hoặc mật khẩu");

            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User")
                },
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                user
            });
        }
    }

    // Nếu chưa có file riêng thì tạm để class request ở đây cũng được
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
