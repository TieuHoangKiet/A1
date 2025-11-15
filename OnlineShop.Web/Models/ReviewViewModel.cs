namespace OnlineShop.Web.Models
{
    public class ReviewViewModel
    {
        public string UserName { get; set; } = string.Empty;   // Tên người đánh giá
        public int Rating { get; set; }                        // Số sao (1–5)
        public string Comment { get; set; } = string.Empty;    // Nội dung đánh giá
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Thời gian đánh giá
    }
}
