namespace API.Dto
{
    public class RatingDto
    {
        public int Star { get; set; }
        public string Comment { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; } // Thông tin người gửi đánh giá
    }
}
