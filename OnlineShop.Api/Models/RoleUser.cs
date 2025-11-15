using System.Text.Json.Serialization;

namespace OnlineShop.Api.Models
{
    public class RoleUser
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }

        [JsonIgnore]  // 🧩 bỏ qua khi serialize/deseralize JSON
        public List<AccountUser>? Accounts { get; set; }
    }
}
