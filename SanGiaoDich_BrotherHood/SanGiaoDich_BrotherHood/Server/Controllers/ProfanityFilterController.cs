using Google.Apis.Auth.OAuth2;
using Google.Cloud.Language.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Azure.AI.TextAnalytics;
using Azure;
using Microsoft.Extensions.Configuration;
using System.Text;
using Google.Cloud.Translation.V2;
using System.Net.Http;
using System.Text.Json;
using SanGiaoDich_BrotherHood.Server.Services;
using Newtonsoft.Json;

namespace SanGiaoDich_BrotherHood.Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProfanityFilterController : ControllerBase
    {
        private readonly HuggingFaceService _huggingFaceService;
        private readonly LanguageServiceClient _languageServiceClient;
        private readonly TranslationClient _translationClient;
        private readonly HttpClient _httpClient;

        private static readonly HashSet<string> ProfanityWords = new HashSet<string>
        {
            "mày", "đụ", "vãi", "cặc", "lồn" 
        };
        private readonly TextAnalyticsClient _textAnalyticsClient;
        public ProfanityFilterController(IConfiguration configuration, HuggingFaceService huggingFaceService)
        {
            _languageServiceClient = LanguageServiceClient.Create();
            var endpoint = "https://brothertrain.cognitiveservices.azure.com/";
            var apiKey = "G9zNUpudjYir1aqzZoy2rt6Nx8z0GTvWbnWP6bi6TPirUSXdBtRuJQQJ99ALACYeBjFXJ3w3AAAaACOG8LZR";
            if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException("Azure Text Analytics configuration is missing or incomplete.");
            }

            var credential = new AzureKeyCredential(apiKey);
            _textAnalyticsClient = new TextAnalyticsClient(new Uri(endpoint), credential);
            _translationClient = TranslationClient.Create();
            _huggingFaceService = huggingFaceService;

        }
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateProductDescription(string productName)
        {
            var apiUrl = "https://brotherstudent.openai.azure.com/azureml://registries/azure-openai/models/gpt-4/versions/turbo-2024-04-09";
            var apiKey = "BzVMSgMeUEgwaXpLPfDHXpvDKm5YvXgeztpKSXWNZQ8UM9M7be8mJQQJ99ALACYeBjFXJ3w3AAABACOG9CBa";  // Đảm bảo đây là API key hợp lệ

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var requestBody = new
            {
                prompt = $"Tạo mô tả cho sản phẩm: {productName}",
                max_tokens = 100,
                temperature = 0.7
            };

            try
            {
                var response = await client.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, errorResponse); 
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                return Ok(responseBody);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private async Task<string> TranslateTextToEnglish(string text)
        {
            var response = await _translationClient.TranslateTextAsync(text, "en");
            return response.TranslatedText;
        }

        private async Task<string> TranslateTextToVietnamese(string text)
        {
            var response = await _translationClient.TranslateTextAsync(text, "vi");
            return response.TranslatedText;
        }

        [HttpPost("check-profanity")]
        public async Task<IActionResult> CheckProfanity([FromBody] string text)
        {
            var document = new Document
            {
                Content = text,
                Type = Document.Types.Type.PlainText
            };
            var response = await _languageServiceClient.AnalyzeSentimentAsync(document);
            var sentiment = response.DocumentSentiment;
            if (sentiment.Score < -0.5) 
            {
                return Ok("Văn bản có khả năng chứa từ ngữ tục tiểu.");
            }
            var words = text.Split(new[] { ' ', '.', ',', ';', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (ProfanityWords.Contains(word.ToLower()))
                {
                    return Ok("Văn bản có từ ngữ tục tiểu.");
                }
            }

            return Ok("Văn bản không có từ ngữ tục tiểu.");
        }
    }
}
