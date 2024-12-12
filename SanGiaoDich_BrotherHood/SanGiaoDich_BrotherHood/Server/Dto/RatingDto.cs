namespace SanGiaoDich_BrotherHood.Server.Dto
{
    public class RatingDto
    {
        public int Star { get; set; }
        public string Comment { get; set; }
        public string Image { get; set; }
        public string UserName { get; set; } // Thông tin người gửi đánh giá
        public string FullName { get; set; }
        public string ImageAccount { get; set; }
		public string ProductName { get; set; } // Tên sản phẩm
		public string ProductImage { get; set; } // Ảnh sản phẩm
	}
}
