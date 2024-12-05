namespace SanGiaoDich_BrotherHood.Shared.Dto
{
    public class ProductDto
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string ProrityLevel { get; set; }
        public int CategoryId { get; set;}
        public decimal PriceUp { get; set; }
    }
}
