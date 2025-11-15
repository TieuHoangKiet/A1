namespace OnlineShop.Api.Models
{
    public class AccountUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // chỉ lưu RoleId, không gửi object Role
        public int? RoleId { get; set; }
        public RoleUser? Role { get; set; }
    }
}
