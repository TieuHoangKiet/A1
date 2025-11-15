namespace OnlineShop.Web.Models
{
    public class AccountUser
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string PasswordHash { get; set; } = "";   // 🟢 THÊM DÒNG NÀY
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public int RoleId { get; set; }
        public RoleUser? Role { get; set; }

        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
