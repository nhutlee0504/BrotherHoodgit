namespace SanGiaoDich_BrotherHood.Server.Configurations
{
    public class OpenAIConfig
    {
        public string ApiKey { get; set; }
        public string ModerationEndpoint { get; set; }  // Endpoint kiểm duyệt văn bản
        public string ImageModerationEndpoint { get; set; }  // Endpoint kiểm duyệt hình ảnh
    }
}
