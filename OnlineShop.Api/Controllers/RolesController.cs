using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Api.Data;
using OnlineShop.Api.Models;

namespace OnlineShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]  // 🔒 FULL controller
    public class RolesController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public RolesController(ShopDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleUser>>> GetRoles()
        {
            return await _context.Roles_Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleUser>> GetRole(int id)
        {
            var role = await _context.Roles_Users.FindAsync(id);
            if (role == null) return NotFound();
            return role;
        }

        [HttpPost]
        public async Task<ActionResult<RoleUser>> CreateRole(RoleUser model)
        {
            _context.Roles_Users.Add(model);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRole), new { id = model.RoleId }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleUser model)
        {
            if (id != model.RoleId) return BadRequest();

            _context.Entry(model).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles_Users.FindAsync(id);
            if (role == null) return NotFound();

            _context.Roles_Users.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
