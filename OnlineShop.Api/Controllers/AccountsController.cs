using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Api.Data;
using OnlineShop.Api.Models;

namespace OnlineShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public AccountsController(ShopDbContext context)
        {
            _context = context;
        }

        // 🔓 Ai cũng xem tất cả user (tùy bạn, nhưng nên bắt Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountUser>>> GetAll()
        {
            return await _context.Account_Users
                .Include(a => a.Role)
                .OrderByDescending(a => a.UserId)
                .ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountUser>> GetById(int id)
        {
            var user = await _context.Account_Users
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.UserId == id);

            if (user == null) return NotFound();
            return user;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AccountUser model)
        {
            if (model == null) return BadRequest("Dữ liệu không hợp lệ.");
            if (string.IsNullOrWhiteSpace(model.PasswordHash))
                return BadRequest("Mật khẩu không được để trống.");

            model.CreatedAt = DateTime.Now;
            _context.Account_Users.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AccountUser model)
        {
            var user = await _context.Account_Users.FindAsync(id);
            if (user == null) return NotFound();

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Phone = model.Phone;
            user.RoleId = model.RoleId;

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Account_Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Account_Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // 🔓 Đăng ký cho User — không cần token
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountUser model)
        {
            if (string.IsNullOrWhiteSpace(model.UserName) || string.IsNullOrWhiteSpace(model.PasswordHash))
                return BadRequest("Thiếu thông tin đăng ký");

            bool exists = await _context.Account_Users.AnyAsync(x => x.UserName == model.UserName);
            if (exists)
                return BadRequest("Tên đăng nhập đã tồn tại!");

            model.RoleId = 2;
            model.IsActive = true;
            model.CreatedAt = DateTime.Now;

            _context.Account_Users.Add(model);
            await _context.SaveChangesAsync();

            return Ok(model);
        }
    }
}
