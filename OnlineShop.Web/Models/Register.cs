namespace OnlineShop.Web.Models
{
	public class RegisterModel
	{
		public string UserName { get; set; }
		public string PasswordHash { get; set; }
		public string ConfirmPassword { get; set; }   // 👈 THÊM DÒNG NÀY
		public string Email { get; set; }
		public string FullName { get; set; }
		public string Phone { get; set; }
	}
}
