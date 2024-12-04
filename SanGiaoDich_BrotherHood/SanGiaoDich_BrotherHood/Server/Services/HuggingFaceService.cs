using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace SanGiaoDich_BrotherHood.Server.Services
{
    public class HuggingFaceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken;

        public HuggingFaceService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            var apiUrl = "https://api-inference.huggingface.co/models/EleutherAI/gpt-neo-2.7B";  // Sử dụng mô hình GPT-2 hoặc mô hình bạn muốn
            if (string.IsNullOrWhiteSpace(apiUrl))
            {
                throw new ArgumentNullException("HuggingFace API URL is not configured properly.");
            }

            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(apiUrl);
            _apiToken = "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";

            if (string.IsNullOrWhiteSpace(_apiToken))
            {
                throw new ArgumentNullException("HuggingFace API Token is not configured properly.");
            }

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiToken}");
        }

        public async Task<string> GenerateProductDescriptionAsync(string productName)
        {
            var payload = new
            {
                inputs = $"{productName}",
                parameters = new { max_length = 300, temperature = 0.9 }
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync("", content);

                if (!response.IsSuccessStatusCode)
                {
                    return $"API Error: {response.StatusCode}";
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + jsonResponse);  // In để kiểm tra kết quả JSON

                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                // Kiểm tra nếu kết quả có thuộc tính 'generated_text'
                if (result.ValueKind == JsonValueKind.Array && result.GetArrayLength() > 0)
                {
                    var generatedText = result[0].GetProperty("generated_text").GetString();
                    return generatedText ?? "No description available.";
                }

                return "No description available.";
            }
            catch (Exception ex)
            {
                return $"Error calling API: {ex.Message}";
            }
        }
    }
}
