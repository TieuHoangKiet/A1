namespace OnlineShop.Web.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        // Dùng luôn AccountUser ở Web
        public AccountUser User { get; set; } = new AccountUser();
    }
}
